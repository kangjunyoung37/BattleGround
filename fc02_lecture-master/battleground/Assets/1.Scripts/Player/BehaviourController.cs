using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 현재 동작과 기본 동작, 오버라이딩 동작, 잠긴 동작, 마우스 이동 값
/// 땅에 서있는지, GenericBehaviour를 상속받은 동작들을 업데이트 시켜줌
/// </summary>
public class BehaviourController : MonoBehaviour
{
    private List<GenericBehaviour> behaviours;//동작들
    private List<GenericBehaviour> overrideBehaviours;//우선시 되는 동작
    private int currentBehaviour;//현재 동작 해시코드
    private int defaultBehaviour;//기본 동작 해시코드
    private int behaviourLocked;//잠긴 동작 해시코드
    //캐싱
    public Transform playerCamera;
    private Animator myAnimator;
    private Rigidbody myRigidbody;
    private ThirdPersonOrbitCamera camScript;
    private Transform myTransform;
    //
    private float h; // 수평 값
    private float v; // 수직 값
    public float turnSmoothing = 0.06f;//카메라 향하도록 움직일 때 회전속도
    private bool changedFOV;//달리기 동작이 카메라 시야각이 변경되었을 때 저장되었는가 물어보는 변수
    public float sprintFOV = 100;//달리기 시야각
    private Vector3 lastDirection; //마지막 향했던 방향
    private bool sprint;//달리는 중인가
    private int hFloat;//애니메이터 관련 가로축 값
    private int vFloat;//애니메이터 관련 세로축 값
    private int groundedBool; // 땅에 있는가 
    private Vector3 colExtents;// 땅과의 충돌체크를 위한 충돌체 영역

    public float GetH { get => h; }
    public float GetV { get => v; }
    public ThirdPersonOrbitCamera GetCamScript{ get => camScript; }
    public Rigidbody GetRigidBody{ get => myRigidbody; }
    public Animator GetAnimator{ get => myAnimator; }
    public int GetDefaultBehaviour{ get => defaultBehaviour; }

    private void Awake()
    {
        behaviours = new List<GenericBehaviour>();
        overrideBehaviours = new List<GenericBehaviour>();
        myAnimator = GetComponent<Animator>();
        hFloat = Animator.StringToHash(KJY.AnimatorKey.Horizontal);
        vFloat = Animator.StringToHash(KJY.AnimatorKey.Vertical);
        camScript = playerCamera.GetComponent<ThirdPersonOrbitCamera>();
        myRigidbody = GetComponent<Rigidbody>();
        groundedBool = Animator.StringToHash(KJY.AnimatorKey.Grounded);
        colExtents = GetComponent<Collider>().bounds.extents;
        myTransform = transform;
    }

    public bool IsMoving()
    {
        return Mathf.Abs(h) > Mathf.Epsilon || Mathf.Abs(v) > Mathf.Epsilon;
    }
    public bool IsHorizontalMoving()
    {
        return Mathf.Abs(h) > Mathf.Epsilon;
    }
    public bool CanSprint()
    {
        foreach(GenericBehaviour behaviour in behaviours)
        {
            if(!behaviour.AllowSprint)
            {
                return false;
            }
        }
        foreach(GenericBehaviour genericBehaviour in overrideBehaviours)
        {
            if(!genericBehaviour.AllowSprint)
            {
                return false;
            }
        }
        return true;
    }
    public bool IsSprinting()
    {
        return sprint && IsMoving() && CanSprint();
    }

    public bool IsGrounded()
    {
        Ray ray = new Ray(myTransform.position + Vector3.up * 2 * colExtents.x, Vector3.down);
        return Physics.SphereCast(ray, colExtents.x, colExtents.x + 0.2f);
    }
    private void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        myAnimator.SetFloat(hFloat, h, 0.1f, Time.deltaTime);
        myAnimator.SetFloat(vFloat, v, 0.1f, Time.deltaTime);
        sprint = Input.GetButton(ButtonName.Sprint);
        if((IsSprinting()))
        {
            changedFOV = true;
            camScript.SetFOV(sprintFOV);
        }
        else if(changedFOV)
        {
            camScript.ResetFOV();
            changedFOV = false;
        }
        myAnimator.SetBool(groundedBool, IsGrounded());
    }

    public void RePositioning()
    {
        if(lastDirection != Vector3.zero)
        {
            lastDirection.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(lastDirection);
            Quaternion newRotation = Quaternion.Slerp(myRigidbody.rotation, targetRotation, turnSmoothing);
            myRigidbody.MoveRotation(newRotation);

        }
    }
    private void FixedUpdate()
    {
        bool isAnyBehaviourActive = false;
        if(behaviourLocked > 0 || overrideBehaviours.Count == 0)
        {
            foreach(GenericBehaviour behaviour in behaviours)
            {
                if(behaviour.isActiveAndEnabled && currentBehaviour == behaviour.GetBehaviourCode)
                {
                    isAnyBehaviourActive = true;
                    behaviour.LocalFixedUpdate();
                }
            }
        }
        else
        {
            foreach(GenericBehaviour behaviour in overrideBehaviours)
            {
                behaviour.LocalFixedUpdate();
            }
        }
        if(!isAnyBehaviourActive && overrideBehaviours.Count == 0)
        {
            myRigidbody.useGravity = true;
            RePositioning();
        }
    }
    private void LateUpdate()
    {
        if(behaviourLocked >0 || overrideBehaviours.Count == 0)
        {
            foreach(GenericBehaviour behaviour in behaviours)
            {
                if(behaviour.isActiveAndEnabled && currentBehaviour == behaviour.GetBehaviourCode)
                {
                    behaviour.LocalLateUpdate();
                }
            }
        }
        else
        {
            foreach(GenericBehaviour behaviour in overrideBehaviours)
            {
                behaviour.LocalLateUpdate();
            }
        }
    }
    public void SubCribeBahaviour(GenericBehaviour behaviour)
    {
        behaviours.Add(behaviour);
    }

    public void RegisterDefaultBehaviour(int behaviourCode)
    {
        defaultBehaviour = behaviourCode;
        currentBehaviour = behaviourCode;
    }

    public void RegisterBehaviour(int behaviourCode)
    {
        if(currentBehaviour == defaultBehaviour)
        {
            currentBehaviour = behaviourCode;
        }

    }
    public void UnRegisterBehaviour(int behaviourCode)
    {
        if(currentBehaviour == behaviourCode)
        {
            currentBehaviour = defaultBehaviour;
        }
    }
    public bool OverrideWihtBehaviour(GenericBehaviour behaviour)
    {
        if(!overrideBehaviours.Contains(behaviour))
        {
            if(overrideBehaviours.Count == 0)
            {
                foreach(GenericBehaviour behaviour1 in behaviours)
                {
                    if(behaviour1.isActiveAndEnabled && currentBehaviour == behaviour1.GetBehaviourCode)
                    {
                        behaviour1.OnOverride();
                        break;
                    }
                }
            }
            overrideBehaviours.Add(behaviour);
            return true;
        }
        return false;
    }
    public bool RevokeOverridingBehaviour(GenericBehaviour behaviour)
    {
        if(overrideBehaviours.Contains(behaviour))
        {
            overrideBehaviours.Remove(behaviour);
            return true;
        }
        return false;
        
    }
    public bool IsOverring(GenericBehaviour behaviour = null)
    {
        if(behaviour == null)
        {
            return overrideBehaviours.Count > 0;
        }
        return overrideBehaviours.Contains(behaviour);
    }
    public bool IsCurrentBehaviour(int behaviourCode)
    {
        return this.currentBehaviour == behaviourCode;
    }
    public bool GetTempLockStatus(int behaviourCode = 0)
    {
        return (behaviourLocked != 0 && behaviourLocked != behaviourCode);
    }
    public void LockTempBehaviour(int behaviourCode)
    {
        if(behaviourLocked == 0)
        {
            behaviourLocked = behaviourCode;
        }
    }
    public void UnLockTempBehaviour(int behaviourCode)
    {
        if(behaviourLocked == behaviourCode)
        {
            behaviourLocked = 0;
        }
    }
    public Vector3 GetLastDirection()
    {
        return lastDirection;
    }
    public void SetLastDirection(Vector3 direction)
    {
        lastDirection = direction;
    }
}

public abstract class GenericBehaviour : MonoBehaviour
{
    protected int speedFloat;
    protected BehaviourController behaviourController;
    protected int behaviourCode;//해쉬코드
    protected bool canSprint;

    protected void Awake()
    {
        this.behaviourController = GetComponent<BehaviourController>();
        speedFloat = Animator.StringToHash(KJY.AnimatorKey.Speed);
        canSprint = true;
        //동작 타입을 해시코드로 가지고 있음 추후에 구별용으로 사용
        behaviourCode = this.GetType().GetHashCode();
    }
    public int GetBehaviourCode
    {
        get
        {
            return behaviourCode;
        }
    }
    public bool AllowSprint
    {
        get
        {
            return canSprint;
        }
    }
    public virtual void LocalLateUpdate()
    {

    }
    public virtual void LocalFixedUpdate()
    {

    }
    public  virtual void OnOverride()
    {

    }
}
