using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
public class EnemyAnimation : MonoBehaviour
{
    [HideInInspector]public Animator anim;
    [HideInInspector] public float currentAimingAngleGap;
    [HideInInspector] public Transform gunMuzzle;
    [HideInInspector] public float angularSpeed;

    private StateController controller;
    private NavMeshAgent nav;
    private bool pendingAim;//������ ��ٸ��� �ð�
    private Transform hips, spine;//�� Ʈ������
    private Vector3 initialRootRotation;
    private Vector3 initialHipsRotation;
    private Vector3 initialSpineRotation;
    private Quaternion lastRotation;
    private float timeCountAim , timeCountGuard;
    private readonly float turnSpeed = 25f;//��Ʈ�����Ҷ� ��� �̵��ϰ� ����

    private void Awake()
    {
        controller = GetComponent<StateController>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        nav.updateRotation = false; //�׺�޽�������Ʈ�� �����̼��� ��

        hips = anim.GetBoneTransform(HumanBodyBones.Hips);
        spine = anim.GetBoneTransform(HumanBodyBones.Spine);

        initialRootRotation = (hips.parent == transform) ? Vector3.zero : hips.parent.localEulerAngles;
        initialHipsRotation = hips.localEulerAngles;
        initialSpineRotation = spine.localEulerAngles;

        anim.SetTrigger(KJY.AnimatorKey.ChangeWeapon);
        anim.SetInteger(KJY.AnimatorKey.Weapon, (int)Enum.Parse(typeof(WeaponType), controller.classStats.WeaponType));//classState = �����͵�
        
        foreach(Transform child in anim.GetBoneTransform(HumanBodyBones.RightHand))
        {
            gunMuzzle = child.Find("muzzle");
            if(gunMuzzle != null)
            {
                break;
            }

        }
        foreach(Rigidbody meber in GetComponentsInChildren<Rigidbody>())//�����ϴ� ���⿡ RigidBoyd�� ���� ��� ����.
        {
            meber.isKinematic = true;
        }
    }
    void SetUp(float speed, float angle , Vector3 strafeDirection)
    {
        angle *= Mathf.Deg2Rad;
        angularSpeed = angle / controller.generalStates.angleResponseTime;//���ӵ�

        anim.SetFloat(KJY.AnimatorKey.Speed, speed, controller.generalStates.speedDampTime, Time.deltaTime);
        anim.SetFloat(KJY.AnimatorKey.AngularSpeed, angularSpeed, controller.generalStates.angularSpeedDampTime, Time.deltaTime);
        //��Ʈ���ΰ���
        anim.SetFloat(KJY.AnimatorKey.Horizontal, strafeDirection.x, controller.generalStates.speedDampTime, Time.deltaTime);
        anim.SetFloat(KJY.AnimatorKey.Vertical, strafeDirection.y, controller.generalStates.speedDampTime, Time.deltaTime);


    }
    //
    void NavAnimSetup()
    {
        float speed;
        float angle;
        speed = Vector3.Project(nav.desiredVelocity, transform.forward).magnitude;//���ǵ�
        if(controller.focusSight)
        {
            Vector3 dest = (controller.personalTarget - transform.position);
            dest.y = 0.0f;
            angle = Vector3.SignedAngle(transform.forward, dest, transform.up);
            if(controller.Strafing)
            {
                dest = dest.normalized;
                Quaternion targetStrafeRotation = Quaternion.LookRotation(dest);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetStrafeRotation, turnSpeed * Time.deltaTime);

            }

        }
        else
        {
            if(nav.desiredVelocity == Vector3.zero)
            {
                angle = 0.0f;

            }
            else
            {
                angle = Vector3.SignedAngle(transform.forward, nav.desiredVelocity, transform.up);

            }
        }
        //�÷��̾ ���Ϸ� �Ҷ� ���� �Ÿ��� �ʵ��� ���� �������� ����
        if(!controller.Strafing && Mathf.Abs(angle) < controller.generalStates.angleDeadZone)
        {
            transform.LookAt(transform.position + nav.desiredVelocity);
            angle = 0f;
            if(pendingAim && controller.focusSight)
            {
                controller.Aiming = true;
                pendingAim = false;
            }
        }
        //Strafe direction
        Vector3 direction = nav.desiredVelocity;
        direction.y = 0.0f;
        direction = direction.normalized;
        direction = Quaternion.Inverse(transform.rotation) * direction;
        SetUp(speed, angle, direction);
    }
    private void Update()
    {
        NavAnimSetup();

    }
    private void OnAnimatorMove()
    {
        if(Time.timeScale > 0 && Time.deltaTime > 0)
        {
            nav.velocity = anim.deltaPosition / Time.deltaTime;
            if(!controller.Strafing)
            {
                transform.rotation = anim.rootRotation;
            }
        }

    }
    private void LateUpdate()
    {
        if(controller.Aiming)
        {
            Vector3 direction = controller.personalTarget - spine.position;
            if(direction.magnitude < 0.01f || direction.magnitude > 1000000.0f)
            {
                return;
            }
            Quaternion targetRotion = Quaternion.LookRotation(direction);
            targetRotion *= Quaternion.Euler(initialRootRotation);
            targetRotion *= Quaternion.Euler(initialHipsRotation);
            targetRotion *= Quaternion.Euler(initialSpineRotation);

            targetRotion *= Quaternion.Euler(KJY.VectorHelper.ToVector(controller.classStats.AimOffset));
            Quaternion frameRotation = Quaternion.Slerp(lastRotation, targetRotion, timeCountAim);
            //�����̸� �������� ô�� ȸ���� 60�� ������ ���� ��� ������ �����ϴ�
            if(Quaternion.Angle(frameRotation, hips.rotation) <= 60.0f)
            {
                spine.rotation = frameRotation;
                timeCountAim += Time.deltaTime;
            }
            else
            {
                if(timeCountAim == 0 && Quaternion.Angle(frameRotation,hips.rotation) > 70.0f)
                {
                    StartCoroutine(controller.UnstuckAim(2f));
                }
                spine.rotation = lastRotation;
                timeCountAim = 0;
            }

            lastRotation = spine.rotation;
            Vector3 target = controller.personalTarget - gunMuzzle.position;
            Vector3 forward = gunMuzzle.forward;
            currentAimingAngleGap = Vector3.Angle(target, forward);

            timeCountGuard = 0;

        }
        else
        {
            lastRotation = spine.rotation;
            spine.rotation *= Quaternion.Slerp(Quaternion.Euler(KJY.VectorHelper.ToVector(controller.classStats.AimOffset)), Quaternion.identity, timeCountGuard);
            timeCountGuard += Time.deltaTime;
            
        }
    }

    public void ActivatePendingAim()
    {
        pendingAim = true;
       
    }
    public void AbortPendingAim()
    {
        pendingAim = false;
        controller.Aiming = false;
    }

}
