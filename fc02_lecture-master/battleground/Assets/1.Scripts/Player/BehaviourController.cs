using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ���۰� �⺻ ����, �������̵� ����, ��� ����, ���콺 �̵� ��
/// ���� ���ִ���, GenericBehaviour�� ��ӹ��� ���۵��� ������Ʈ ������
/// </summary>
public class BehaviourController : MonoBehaviour
{
    private List<GenericBehaviour> behaviours;//���۵�
    private List<GenericBehaviour> overrideBehaviours;//�켱�� �Ǵ� ����
    private int currentBehaviour;//���� ���� �ؽ��ڵ�
    private int defaultBehaviour;//�⺻ ���� �ؽ��ڵ�
    private int behaviourLocked;//��� ���� �ؽ��ڵ�
    //ĳ��
    public Transform playerCamera;
    private Animator myAnimator;
    private Rigidbody myRigidbody;
    private ThirdPersonOrbitCamera camScript;
    private Transform myTransform;
    //
    private float h; // ���� ��
    private float v; // ���� ��
    public float turnSmoothing = 0.06f;//ī�޶� ���ϵ��� ������ �� ȸ���ӵ�
    private bool changedFOV;//�޸��� ������ ī�޶� �þ߰��� ����Ǿ��� �� ����Ǿ��°� ����� ����
    public float sprintFOV = 100;//�޸��� �þ߰�
    private Vector3 lastDirection; //������ ���ߴ� ����
    private bool sprint;//�޸��� ���ΰ�
    private int hFloat;//�ִϸ����� ���� ������ ��
    private int vFloat;//�ִϸ����� ���� ������ ��
    private int groundedBool; // ���� �ִ°� 
    private Vector3 colExtents;// ������ �浹üũ�� ���� �浹ü ����

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
    protected int behaviourCode;//�ؽ��ڵ�
    protected bool canSprint;

    protected void Awake()
    {
        this.behaviourController = GetComponent<BehaviourController>();
        speedFloat = Animator.StringToHash(KJY.AnimatorKey.Speed);
        canSprint = true;
        //���� Ÿ���� �ؽ��ڵ�� ������ ���� ���Ŀ� ���������� ���
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
