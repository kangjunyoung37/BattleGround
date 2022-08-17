using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 사격 기능 : 사격이 가능한지 여부를 체크하는 기능
/// 발사 키 입력받아서 애니메이션 재생, 이펙트 생성, 충돌 체크 기능, 
/// UI 관련해서 십자선 표시 기능
/// 발사 속도 조정
/// 캐릭터 상체를 IK를 이용해서 조준 시점에 맞춰서 회전
/// 벽이나 충돌체에 총알이 피격되었을경우 피탄이펙트를 생성.
/// 이벤토리 역할, 무기를 소지하고 있는지 확인용.
/// 재장전과 무기 교체 기능 포함
/// </summary>
public class ShootBehaviour : GenericBehaviour
{
    public Texture2D aimCrossHair, shootCrossHair;
    public GameObject muzzleFlash, shot, sparks;
    public Material bulletHole;
    public int MaxBulletHoels = 50;
    public float shootErrorRate = 0.01f; //오발률
    public float shootRateFactor = 1f;//발사 속도

    public float armRotation = 8f;//팔 회전
    public LayerMask shotMask = ~(FC.TagAndLayer.LayerMasking.IgnoreRayCast | FC.TagAndLayer.LayerMasking.IgnoreShot |
        FC.TagAndLayer.LayerMasking.CoverInvisible | FC.TagAndLayer.LayerMasking.Player);
    public LayerMask organicMask = FC.TagAndLayer.LayerMasking.Player | FC.TagAndLayer.LayerMasking.Enemy;//생명체
    public Vector3 leftArmShortAim = new Vector3(-4.0f, 0.0f, 2.0f);//짧은 총을 들었을 때 조준시 왼팔의 위치 보정

    private int activeWeapon = 0;//0이 아니면 활성화 되어 있다
    
    //애니메이터용 인덱스
    private int weaponType;
    private int changeWeaponTrigger;
    private int shootingTrigger;
    private int aimBool, blockedAimBool, reloadBool;
    
    private List<InteractiveWeapon> weapons;//소지하고 있는 무기들
    private bool isAiming , isAimBlocked;

    private Transform gunMuzzle;
    private float distToHand;
    private Vector3 castRelativeOrigin;

    private Dictionary<InteractiveWeapon.WeaponType, int> slotMap;
    private Transform hips, spine, chest, rightHand, leftArm;
    private Vector3 initailRootRotation;
    private Vector3 initailHipsRotation;
    private Vector3 initailSpineRotation;
    private Vector3 initailChestRotation;

    private float shotInterval, originalShotInterval = 0.5f;
    private List<GameObject> bulletHoles;//피탄구멍
    private int bulletHoleSlot = 0;
    private int burstShotCount = 0;
    private AimBehaviour aimBehaviour;
    private Texture2D originalCrossHair;
    private bool isShooting = false;
    private bool isChangingWeapon = false;
    private bool isShotAlive = false;

    private void Start()
    {
        weaponType = Animator.StringToHash(FC.AnimatorKey.Weapon);
        aimBool = Animator.StringToHash(FC.AnimatorKey.Aim);
        blockedAimBool = Animator.StringToHash(FC.AnimatorKey.BlockedAim);
        changeWeaponTrigger = Animator.StringToHash(FC.AnimatorKey.ChangeWeapon);
        shootingTrigger = Animator.StringToHash(FC.AnimatorKey.Shooting);
        reloadBool = Animator.StringToHash(FC.AnimatorKey.Reload);
        weapons = new List<InteractiveWeapon>(new InteractiveWeapon[3]);
        aimBehaviour = GetComponent<AimBehaviour>();
        bulletHoles = new List<GameObject>();

        muzzleFlash.SetActive(false);
        shot.SetActive(false);
        sparks.SetActive(false);

        slotMap = new Dictionary<InteractiveWeapon.WeaponType, int>
        {
            {InteractiveWeapon.WeaponType.SHORT,1 },
            {InteractiveWeapon.WeaponType.LONG,2}
        };

        Transform neck = this.behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Neck);
        if(!neck)
        {
            neck = this.behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Head).parent;

        }
        hips = this.behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Hips);
        spine = this.behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Spine);
        chest = this.behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Chest);
        rightHand = this.behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.RightHand);
        leftArm = this.behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.LeftUpperArm);

        initailRootRotation = (hips.parent == transform) ? Vector3.zero : hips.parent.localEulerAngles;
        initailHipsRotation = hips.localEulerAngles;
        initailSpineRotation = spine.localEulerAngles;
        initailChestRotation = chest.localEulerAngles;
        originalCrossHair = aimBehaviour.crossHair;
        shotInterval = originalShotInterval;
        castRelativeOrigin = neck.position - transform.position;
        distToHand = (rightHand.position - neck.position).magnitude * 1.5f;



    }
    //발사 비주얼 담당
    private void DrawShoot(GameObject weapon, Vector3 destination, Vector3 targetNormal ,Transform parent, bool placeSparks = true, bool placeBulletBool = true)
    {
        Vector3 origin = gunMuzzle.position - gunMuzzle.right * 0.5f;
        muzzleFlash.SetActive(true);
        muzzleFlash.transform.SetParent(gunMuzzle);
        muzzleFlash.transform.localPosition = Vector3.zero;
        muzzleFlash.transform.localEulerAngles = Vector3.back * 90f;

        GameObject intantShot = EffectManager.Instance.EffectOneShot((int)EffectList.tracer, origin);
        intantShot.SetActive(true);
        intantShot.transform.rotation = Quaternion.LookRotation(destination - origin);
        intantShot.transform.parent = shot.transform.parent;

        if(placeSparks)
        {
            GameObject intantSparks = EffectManager.Instance.EffectOneShot((int)EffectList.sparks, destination);
            intantSparks.SetActive(true);
            intantSparks.transform.parent = sparks.transform.parent;
        }
        if(placeBulletBool)
        {
            Quaternion hitRotation = Quaternion.FromToRotation(Vector3.back, targetNormal);
            GameObject bullet = null;
            if(bulletHoles.Count < MaxBulletHoels)
            {
                bullet = GameObject.CreatePrimitive(PrimitiveType.Quad);
                bullet.GetComponent<MeshRenderer>().material = bulletHole;
                bullet.GetComponent<Collider>().enabled = false;
                bullet.transform.localScale = Vector3.one * 0.07f;
                bullet.name = "BulletHole";
                bulletHoles.Add(bullet);
            }
            else
            {
                bullet = bulletHoles[bulletHoleSlot];
                bulletHoleSlot++;
                bulletHoleSlot %= MaxBulletHoels;
            }
            bullet.transform.position = destination + 0.01f * targetNormal;
            bullet.transform.rotation = hitRotation;
            bullet.transform.SetParent(parent);
        }

    }
    private void ShootWeapon(int weapon,bool firstShot = true)
    {
        if(!isAiming || isAimBlocked || behaviourController.GetAnimator.GetBool(reloadBool)|| !weapons[weapon].Shoot(firstShot))
        {
            return;
        }
        else
        {
            this.burstShotCount++;
            behaviourController.GetAnimator.SetTrigger(shootingTrigger);
            aimBehaviour.crossHair = shootCrossHair;
            behaviourController.GetCamScript.BounceVertical(weapons[weapon].recoilAngle);

            Vector3 imprecision = Random.Range(-shootErrorRate, shootErrorRate) * behaviourController.playerCamera.forward;
            Ray ray = new Ray(behaviourController.playerCamera.position, behaviourController.playerCamera.forward + imprecision);
            RaycastHit hit = default(RaycastHit);
            if (Physics.Raycast(ray, out hit, 500f, shotMask))
            {
                if(hit.collider.transform != transform)
                {
                    bool isOrganic = (organicMask == (organicMask | (1 << hit.transform.root.gameObject.layer)));
                    DrawShoot(weapons[weapon].gameObject, hit.point, hit.normal, hit.collider.transform, !isOrganic, !isOrganic);
                    if(hit.collider)
                    {
                        hit.collider.SendMessageUpwards("HitCallBack", new HealthBase.DamageInfo(hit.point, ray.direction, weapons[weapon].bulletDamage, hit.collider), SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
            else
            {
                Vector3 destination = (ray.direction * 500f) - ray.origin;
                DrawShoot(weapons[weapon].gameObject, destination, Vector3.up, null, false, false);

            }
            SoundManager.Instance.PlayOndShotEffect((int)weapons[weapon].shotSound, gunMuzzle.position, 5f);
            GameObject gameController = GameObject.FindGameObjectWithTag(FC.TagAndLayer.TagName.GameController);
            gameController.SendMessage("RootAlertNearBy", ray.origin, SendMessageOptions.DontRequireReceiver);
            shotInterval = originalShotInterval;
            isShotAlive = true;

        }
    }
    public void EndReloadWeapon()
    {
        behaviourController.GetAnimator.SetBool(reloadBool, false);
        weapons[activeWeapon].EndReload();
    }

    private void SetWeaponCroosHair(bool armed)
    {
        if(armed)
        {
            aimBehaviour.crossHair = aimCrossHair;
        }
        else
        {
            aimBehaviour.crossHair = originalCrossHair;
        }
    }

    private void ShotProgress()
    {
        if(shotInterval > 0.2f)
        {
            shotInterval -= shootRateFactor * Time.deltaTime;
            if(shotInterval <= 0.4f)
            {
                SetWeaponCroosHair(activeWeapon > 0);
                muzzleFlash.SetActive(false);
                if(activeWeapon > 0)
                {
                    behaviourController.GetCamScript.BounceVertical(-weapons[activeWeapon].recoilAngle * 0.1f);
                    if(shotInterval <= (0.4f - 2f * Time.deltaTime))
                    {
                        if (weapons[activeWeapon].weaponMode == InteractiveWeapon.WeaponMode.AUTO && Input.GetAxisRaw(ButtonName.Shoot) != 0)
                        {
                            ShootWeapon(activeWeapon, false);
                        }
                        else if (weapons[activeWeapon].weaponMode == InteractiveWeapon.WeaponMode.BURST && burstShotCount < weapons[activeWeapon].burstSize)
                        {
                            ShootWeapon(activeWeapon, false);
                        }
                        else if (weapons[activeWeapon].weaponMode != InteractiveWeapon.WeaponMode.BURST)
                        {
                            burstShotCount = 0;
                        }
                    }
                }
            }
        }
        else
        {
            isShotAlive = false;
            behaviourController.GetCamScript.BounceVertical(0);
            burstShotCount = 0;
        }
    }
    private void ChangeWeapon(int oldWeapon, int newWeapon)
    {
        if(oldWeapon > 0)
        {
            weapons[oldWeapon].gameObject.SetActive(false);
            gunMuzzle = null;
            weapons[oldWeapon].Toggle(false);
        }
        while (weapons[newWeapon] == null && newWeapon > 0)
        {
            newWeapon = (newWeapon + 1) % weapons.Count;//빈슬롯찾기
        }
        if(newWeapon > 0)
        {
            weapons[newWeapon].gameObject.SetActive(true);
            gunMuzzle = weapons[newWeapon].transform.Find("muzzle");
            weapons[newWeapon].Toggle(true);
        }
        activeWeapon = newWeapon;
        if(oldWeapon != newWeapon)
        {
            behaviourController.GetAnimator.SetTrigger(changeWeaponTrigger);
            behaviourController.GetAnimator.SetInteger(weaponType, weapons[newWeapon] ? (int)weapons[newWeapon].weaponType : 0);
        }
        SetWeaponCroosHair(newWeapon > 0);
    }

    private void Update()
    {
        float shootTrigger = Mathf.Abs(Input.GetAxisRaw(ButtonName.Shoot));
        if(shootTrigger > Mathf.Epsilon && !isShooting && activeWeapon >0 && burstShotCount == 0)
        {
            isShooting = true;
            ShootWeapon(activeWeapon);
        }
        else if (isShooting && shootTrigger < Mathf.Epsilon)
        {
            isShooting = false;
        }
        else if(Input.GetButtonUp(ButtonName.Reload) && activeWeapon> 0)
        {
            if (weapons[activeWeapon].StartReload())
            {
                SoundManager.Instance.PlayOndShotEffect((int)weapons[activeWeapon].reloadSound, gunMuzzle.position, 0.5f);
                behaviourController.GetAnimator.SetBool(reloadBool, true);
            }
        }
        else if(Input.GetButtonDown(ButtonName.Drop) && activeWeapon > 0)
        {
            EndReloadWeapon();
            int weaponToDrop = activeWeapon;
            ChangeWeapon(activeWeapon, 0); //빈 무기랑 바꾸는것
            weapons[weaponToDrop].Drop();
            weapons[weaponToDrop] = null;

        }
        else
        {
            if((Mathf.Abs(Input.GetAxisRaw(ButtonName.Change))> Mathf.Epsilon && !isChangingWeapon))
            {
                isChangingWeapon = true;
                int nextWeapon = activeWeapon + 1;
                ChangeWeapon(activeWeapon, nextWeapon % weapons.Count);
            }
            else if(Mathf.Abs(Input.GetAxisRaw(ButtonName.Change)) <  Mathf.Epsilon)
            {
                isChangingWeapon = false;
            }
        }
        if(isShotAlive)
        {
            ShotProgress();
        }
        isAiming = behaviourController.GetAnimator.GetBool(aimBool);
    }







    /// <summary>
    /// 인벤토리역할을 하게 될 함수
    /// </summary>
    /// <param name="weapon"></param>
    public void AddWeapon(InteractiveWeapon newWeapon)
    {
        newWeapon.gameObject.transform.SetParent(rightHand);
        newWeapon.transform.localPosition = newWeapon.ringtHandPosition;
        newWeapon.transform.localRotation = Quaternion.Euler(newWeapon.relativeRotation);

        if (weapons[slotMap[newWeapon.weaponType]])
        {
            if (weapons[slotMap[newWeapon.weaponType]].label_weaponName == newWeapon.label_weaponName)
            {
                weapons[slotMap[newWeapon.weaponType]].ResetBullet();
                ChangeWeapon(activeWeapon, slotMap[newWeapon.weaponType]);
                Destroy(newWeapon.gameObject);
                return;
            }
            else
            {
                weapons[slotMap[newWeapon.weaponType]].Drop();
            }
        }
        weapons[slotMap[newWeapon.weaponType]] = newWeapon;
        ChangeWeapon(activeWeapon, slotMap[newWeapon.weaponType]);
    }
    private bool CheckforBlockedAim()
    {
        isAimBlocked = Physics.SphereCast(transform.position + castRelativeOrigin, 0.1f, behaviourController.GetCamScript.transform.forward, out RaycastHit hit, distToHand - 0.1f);
        isAimBlocked = isAimBlocked && hit.collider.transform != transform;
        behaviourController.GetAnimator.SetBool(blockedAimBool, isAimBlocked);
        Debug.DrawRay(transform.position + castRelativeOrigin, behaviourController.GetCamScript.transform.forward * distToHand, isAimBlocked ? Color.red : Color.cyan);
        return isAimBlocked;
    }
    public void OnAnimatorIK(int layerIndex)
    {
        if(isAiming && activeWeapon > 0)
        {
            if(CheckforBlockedAim())
            {
                return;
            }
            Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            targetRotation *= Quaternion.Euler(initailRootRotation);
            targetRotation *= Quaternion.Euler(initailHipsRotation);
            targetRotation *= Quaternion.Euler(initailSpineRotation);
            behaviourController.GetAnimator.SetBoneLocalRotation(HumanBodyBones.Spine, Quaternion.Inverse(hips.rotation) * targetRotation);
            float xcamRot = Quaternion.LookRotation(behaviourController.playerCamera.forward).eulerAngles.x;
            targetRotation = Quaternion.AngleAxis(xcamRot + armRotation, this.transform.right);
            if (weapons[activeWeapon] && weapons[activeWeapon].weaponType == InteractiveWeapon.WeaponType.LONG)
            {
                targetRotation *= Quaternion.AngleAxis(9f, transform.right);
                targetRotation *= Quaternion.AngleAxis(20f, transform.up);
            }
            targetRotation *= spine.rotation;
            targetRotation *= Quaternion.Euler(initailChestRotation);
            behaviourController.GetAnimator.SetBoneLocalRotation(HumanBodyBones.Chest, Quaternion.Inverse(spine.rotation) * targetRotation);


        }
    }
    private void LateUpdate()
    {
        if(isAiming && weapons[activeWeapon] && weapons[activeWeapon].weaponType == InteractiveWeapon.WeaponType.SHORT)
        {
            leftArm.localEulerAngles = leftArm.localEulerAngles + leftArmShortAim;
        }
    }
}
