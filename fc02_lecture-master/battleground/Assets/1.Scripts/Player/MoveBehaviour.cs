using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 이동과 점프 동작을 담당하는 컴포넌트
/// 충돌처리에 대한 기능도 포함
/// 기본 동작으로써 작동
/// </summary>
public class MoveBehaviour : GenericBehaviour
{
    public float walkSpeed = 0.15f;
    public float runSpeed = 1.0f;
    public float sprintSpeed = 2.0f;
    public float speedDampTime = 0.1f;

    public float jumpHeight = 1.5f;
    public float jumpInertiaForce = 10f; //점프 관성
    public float speed, speedSeeker; //속도 , 스피드 조절
    private int jumpBool; 
    private int groundedBool;
    private bool jump;
    private bool isColliding;//충돌체크
    private CapsuleCollider capsuleCollider;
    private Transform myTransform;

    private void Start()
    {
        myTransform = transform;
        capsuleCollider = GetComponent<CapsuleCollider>();
        jumpBool = Animator.StringToHash(KJY.AnimatorKey.Jump);
        groundedBool = Animator.StringToHash(KJY.AnimatorKey.Grounded);
        behaviourController.GetAnimator.SetBool(groundedBool, true);

        //
        behaviourController.SubCribeBahaviour(this);
        behaviourController.RegisterDefaultBehaviour(this.behaviourCode);
        speedSeeker = runSpeed;

    }
    Vector3 Rotating(float horizontal ,float vertical)
    {
        Vector3 forward = behaviourController.playerCamera.TransformDirection(Vector3.forward);

        forward.y = 0.0f;
        forward = forward.normalized;

        Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);//직교하는 벡터
        Vector3 targetDirection = Vector3.zero; ;
        targetDirection = forward * vertical + right * horizontal;

        if(behaviourController.IsMoving() && targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection); //움직이고자 하는 방향의 로테이션을 구함
            Quaternion newRotation = Quaternion.Slerp(behaviourController.GetRigidBody.rotation,targetRotation,behaviourController.turnSmoothing);//그 방향을 구해서 지금 로테이션에서 타겟 로테이션으로 각도 보간한 쿼터니언 값을 가져온다
            behaviourController.GetRigidBody.MoveRotation(newRotation);
            behaviourController.SetLastDirection(targetDirection);
           
        }
        if (!(Mathf.Abs(horizontal) > 0.9f || Mathf.Abs(vertical) > 0.9f))
        {
            behaviourController.RePositioning();
        }
        return targetDirection;
    }

    private void RemoveVerticalVelocity()
    {
        Vector3 horizontalVelocity = behaviourController.GetRigidBody.velocity;
        horizontalVelocity.y = 0;
        behaviourController.GetRigidBody.velocity = horizontalVelocity;

    }
    void MoveMentMangeMent(float horizontal , float vertical)
    {
        if(behaviourController.IsGrounded())
        {
            behaviourController.GetRigidBody.useGravity = true;
        }
        else if(!behaviourController.GetAnimator.GetBool(jumpBool) && behaviourController.GetRigidBody.velocity.y > 0)
        {
            RemoveVerticalVelocity();
        }
        Rotating(horizontal, vertical);
        Vector2 dir = new Vector2(horizontal, vertical);
        speed = Vector2.ClampMagnitude(dir, 1f).magnitude;
        speedSeeker += Input.GetAxis("Mouse ScrollWheel");
        speedSeeker = Mathf.Clamp(speedSeeker, walkSpeed, runSpeed);
        speed *= speedSeeker;
        if(behaviourController.IsSprinting())
        {
            speed = sprintSpeed;
        }
        behaviourController.GetAnimator.SetFloat(speedFloat, speed, speedDampTime, Time.deltaTime);

    }
    private void OnCollisionStay(Collision collision)
    {
        isColliding = true;
        if(behaviourController.IsCurrentBehaviour(GetBehaviourCode) && collision.GetContact(0).normal.y <= 0.1f)
        {
            float vel = behaviourController.GetAnimator.velocity.magnitude;
            Vector3 targentMove = Vector3.ProjectOnPlane(myTransform.forward,collision.GetContact(0).normal).normalized * vel;
            behaviourController.GetRigidBody.AddForce(targentMove, ForceMode.VelocityChange);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        isColliding = false;
    }
    void JumpManageMent()
    {
        if(jump && !behaviourController.GetAnimator.GetBool(jumpBool) && behaviourController.IsGrounded())
        {
            behaviourController.LockTempBehaviour(behaviourCode);
            behaviourController.GetAnimator.SetBool(jumpBool, true);
            if(behaviourController.GetAnimator.GetFloat(speedFloat)> 0.1f )
            {
                //마찰력을 다 0으로 설정
                capsuleCollider.material.dynamicFriction = 0f;
                capsuleCollider.material.staticFriction = 0f;
                RemoveVerticalVelocity();
                float velocity = 2f * Mathf.Abs(Physics.gravity.y) * jumpHeight;
                velocity = Mathf.Sqrt(velocity);
                behaviourController.GetRigidBody.AddForce(Vector3.up * velocity, ForceMode.VelocityChange);

            }
        }
        else if(behaviourController.GetAnimator.GetBool(jumpBool))
        {
            if(!behaviourController.IsGrounded() && !isColliding && behaviourController.GetTempLockStatus())
            {
                behaviourController.GetRigidBody.AddForce(myTransform.forward * jumpInertiaForce * Physics.gravity.magnitude * sprintSpeed, ForceMode.Acceleration);

            }
            if(behaviourController.GetRigidBody.velocity.y < 0f && behaviourController.IsGrounded())
            {
                behaviourController.GetAnimator.SetBool(groundedBool, true);
                capsuleCollider.material.dynamicFriction = 0.6f;
                capsuleCollider.material.staticFriction = 0.6f;
                jump = false;
                behaviourController.GetAnimator.SetBool(jumpBool, false);
                behaviourController.UnLockTempBehaviour(this.behaviourCode);
            }
        }
    }
    private void Update()
    {
        if(!jump && Input.GetButtonDown(ButtonName.Jump) && behaviourController.IsCurrentBehaviour(this.behaviourCode) && !behaviourController.IsOverring())
        {
            jump = true;
        }
    }
    public override void LocalFixedUpdate()
    {
        MoveMentMangeMent(behaviourController.GetH, behaviourController.GetV);
        JumpManageMent();
    }

}
