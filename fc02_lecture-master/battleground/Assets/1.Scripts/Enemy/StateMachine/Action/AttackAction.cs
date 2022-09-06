using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 총 4단계에 걸쳐 사격을 하게 된다
/// 1. 조준 중이고 조준 유효 각도 안에 타겟이 있거나 가깝다면
/// 2. 발사 간격 딜레이가 충분히 되었다면 애니메이션을 재생
/// 3. 충돌 검출을 하는데 사격시 약간의 충격파를 더해준다(오발률)
/// 4. 총구 이펙트 및 총알 이펙트를 생성해준다.
/// </summary>
/// 
[CreateAssetMenu(menuName = "PluggableAI/Actions/Attack")]
public class AttackAction : Action
{
    private readonly float startShootDelay = 0.2f;
    private readonly float aimAngleGap = 30f;

    public override void OnReadyAction(StateController controller)
    {
        controller.variables.shotsInRounds = Random.Range(controller.maxiumBurst / 2, controller.maxiumBurst);
        controller.variables.currentShots = 0;
        controller.variables.startShootTimer = 0f;
        controller.enemyAnimation.anim.ResetTrigger(KJY.AnimatorKey.Shooting);
        controller.enemyAnimation.anim.SetBool(KJY.AnimatorKey.Crouch, false);
        controller.variables.waitInCoverTime = 0;
        controller.enemyAnimation.ActivatePendingAim();//조준 대기 , 시야에 들어오면 조준가능

    }

    private void DoShot(StateController controller, Vector3 direction, Vector3 hitPoint, Vector3 hitNoraml = default, bool organic = false,Transform target = null)
    {
        GameObject muzzleFlash = EffectManager.Instance.EffectOneShot((int)EffectList.flash, Vector3.zero);
        muzzleFlash.transform.SetParent(controller.enemyAnimation.gunMuzzle);
        muzzleFlash.transform.localPosition = Vector3.zero;
        muzzleFlash.transform.localEulerAngles = Vector3.left * 90f;
        DestroyDelayed destroyDelayed = muzzleFlash.AddComponent<DestroyDelayed>();
        destroyDelayed.DelayTime = 0.5f;//자동 삭제

        GameObject shotTracer = EffectManager.Instance.EffectOneShot((int)EffectList.tracer, Vector3.zero);
        shotTracer.transform.SetParent(controller.enemyAnimation.gunMuzzle);
        Vector3 origin = controller.enemyAnimation.gunMuzzle.position;
        shotTracer.transform.position = origin;
        shotTracer.transform.rotation = Quaternion.LookRotation(direction);
        
        if(target && !organic)
        {
            GameObject bulletHole = EffectManager.Instance.EffectOneShot((int)EffectList.bulletHole, hitPoint + 0.01f * hitNoraml);
            bulletHole.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitNoraml);

            GameObject instantSpark = EffectManager.Instance.EffectOneShot((int)EffectList.sparks, hitPoint);

        }
        else if(target && organic)//플레이어를 맞췄을경우
        {
            HealthBase targetHealth = target.GetComponent<HealthBase>();
            if(targetHealth)
            {
                targetHealth.TakdDamage(hitPoint, direction, controller.classStats.BulletDamage, target.GetComponent<Collider>(), controller.gameObject);

            }
        }
        SoundManager.Instance.PlayShotSound(controller.classID, controller.enemyAnimation.gunMuzzle.position, 2f);

    }

    private void CastShot(StateController controller)
    {
        Vector3 imprecision = Random.Range(-controller.classStats.ShotErrorRate, controller.classStats.ShotErrorRate) * controller.transform.right;
        imprecision += Random.Range(-controller.classStats.ShotErrorRate, controller.classStats.ShotErrorRate) * controller.transform.up;
        Vector3 shotDirection = controller.personalTarget - controller.enemyAnimation.gunMuzzle.position;
        shotDirection = shotDirection.normalized + imprecision;
        Ray ray = new Ray(controller.enemyAnimation.gunMuzzle.position, shotDirection); //총구에서 나가는 레이를 생성(오발률을 적용한)
        if (Physics.Raycast(ray, out RaycastHit hit, controller.viewRadius, controller.generalStates.shotMask.value))
        {
            bool isOranic = ((1 << hit.transform.root.gameObject.layer) & controller.generalStates.targetMask) != 0;
            DoShot(controller, ray.direction, hit.point, hit.normal, isOranic, hit.transform);

        }
        else
        {
            DoShot(controller, ray.direction, ray.origin + (ray.direction * 500f));//허공에 쏘는 것
        }
    }

    private bool CanShoot(StateController controller)
    {
        float distance = (controller.personalTarget - controller.enemyAnimation.gunMuzzle.position).magnitude;//거리
        if(controller.Aiming && (controller.enemyAnimation.currentAimingAngleGap < aimAngleGap || distance <= 5.0f))//에임 앵글 갭보다 작고 에임중이고 거리가 너무 가깝다면
        {
            if(controller.variables.startShootTimer >= startShootDelay)
            {
                return true;
            }
            else
            {
                controller.variables.startShootTimer += Time.deltaTime;
            }
        }
        return false;
        
    }
    private void Shoot(StateController controller)
    {
        if(Time.timeScale > 0 && controller.variables.shotTimer == 0f)
        {
            controller.enemyAnimation.anim.SetTrigger(KJY.AnimatorKey.Shooting);
            CastShot(controller);
        }
        else if(controller.variables.shotTimer >= (0.1f + 2f * Time.deltaTime))
        {
            controller.bullets = Mathf.Max(--controller.bullets, 0);
            controller.variables.currentShots++;
            controller.variables.shotTimer = 0;
            return;
        }
        controller.variables.shotTimer += controller.classStats.ShotRateFactor * Time.deltaTime;
    }
    public override void Act(StateController controller)
    {
        controller.focusSight = true;
        if(CanShoot(controller))
        {
            Shoot(controller);
        }
        controller.variables.blindEngageTimer += Time.deltaTime;
    }

}
