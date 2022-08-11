using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ���콺 �����ʹ�ư���� ����, �ٸ� ������ ��ü�ؼ� �����ϰ� �ȴ�.
/// ���콺 �ٹ�ư���� �¿� ī�޶� ����
/// ���� �𼭸����� �����Ҷ� ��ü�� ��¦ ��￩�ִ� ���.
/// </summary>
public class AimBehaviour : GenericBehaviour
{
    public Texture2D crossHair;//���ڼ� �̹���
    public float aimTurnSmoothing = 0.15f;//ī�޶� ���ϵ��� �����Ҷ� ȸ���ӵ�
    public Vector3 aimPivotOffset = new Vector3(0.5f, 1.2f, 0.0f);
    public Vector3 aimCamOffset = new Vector3(0.0f, 0.4f, -0.7f);

    private int aimBool;//�ִϸ����� �Ķ���� ����
    private bool aim; // ����������
    private int cornerBool;//�ִϸ����� ����
    private bool peekCorner;//�÷��̾ �ڳ� �𼭸��� �ִ��� ����
    private Vector3 initialRootRotation;
    private Vector3 initialHipRotation;
    private Vector3 initialSpineRotation;
    private Transform myTransform;

    private void Start()
    {
        aimBool = Animator.StringToHash(FC.AnimatorKey.Aim);
        cornerBool = Animator.StringToHash(FC.AnimatorKey.Corner);

        Transform hips = behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Hips);//ĳ������ ���� ��ġ�� ������
        initialRootRotation = (hips.parent == transform) ? Vector3.zero : hips.parent.localEulerAngles;
        initialHipRotation = hips.localEulerAngles;
        initialSpineRotation = behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Spine).localEulerAngles;
        myTransform = transform;
    }
    //ī�޶� ���� �÷��̾ �ùٸ� �������� ȸ��
    void Rotating()
    {
        Vector3 forward = behaviourController.playerCamera.TransformDirection(Vector3.forward);
        forward.y = 0.0f;
        forward = forward.normalized;

        Quaternion targetRotation = Quaternion.Euler(0f, behaviourController.GetCamScript.GetH, 0.0f);
        float minSpeed = Quaternion.Angle(myTransform.rotation, targetRotation) * aimTurnSmoothing; //ī�޶�������� ���ݾ� �����̱�

        if(peekCorner)
        {
            //���� ���϶� �÷��̾� ��ü�� ��¦ ��￩�ֱ� ����
            myTransform.rotation = Quaternion.LookRotation(-behaviourController.GetLastDirection());
            targetRotation *= Quaternion.Euler(initialRootRotation);
            targetRotation *= Quaternion.Euler(initialHipRotation);
            targetRotation *= Quaternion.Euler(initialSpineRotation);
            Transform spine = behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Spine);
            spine.rotation = targetRotation;
        }
        else
        {
            behaviourController.SetLastDirection(forward);
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, minSpeed *Time.deltaTime);
        }

    }
    //�������϶� �����ϴ� �Լ�
    void AimManageMent()
    {
        Rotating();
       
    }
    private IEnumerator ToggleAimOn()
    {
        yield return new WaitForSeconds(0.05f);
        //������ �Ұ����� �����϶��� ���� ���� ó��
        if(behaviourController.GetTempLockStatus(this.behaviourCode) && behaviourController.IsOverring(this))
        {
            yield return false;
        }
        else
        {
            aim = true;
            int signal = 1;
            if(peekCorner)
            {
                signal = (int)Mathf.Sign(behaviourController.GetH);
            }
            aimCamOffset.x = Mathf.Abs(aimCamOffset.x) * signal;
            aimPivotOffset.x=Mathf.Abs(aimPivotOffset.x) * signal;
            yield return new WaitForSeconds(0.1f);
            behaviourController.GetAnimator.SetFloat(speedFloat, 0.0f);
            behaviourController.OverrideWihtBehaviour(this);

        }
    }
    private IEnumerator ToggleAimOff()
    {
        aim = false;
        yield return new WaitForSeconds(0.3f);
        behaviourController.GetCamScript.ResetTargetOffset();
        behaviourController.GetCamScript.ResetMaxVerticalAngel();
        yield return new WaitForSeconds(0.1f);
        behaviourController.RevokeOverridingBehaviour(this);
    }
    public override void LocalFixedUpdate()
    {
        if(aim)
        {
            behaviourController.GetCamScript.SetTargetOffset(aimPivotOffset, aimCamOffset);
        }
    }
    public override void LocalLateUpdate()
    {
        AimManageMent();
    }
    private void Update()
    {
        peekCorner = behaviourController.GetAnimator.GetBool(cornerBool);
        if(Input.GetAxisRaw(ButtonName.Aim) != 0&& !aim)
        {
            StartCoroutine(ToggleAimOn());
        }
        else if(aim && Input.GetAxisRaw(ButtonName.Aim) == 0)
        {
            StartCoroutine(ToggleAimOff());
        }
        //�������϶��� �޸��⸦ ���� ����
        canSprint = !aim;
        if(aim && Input.GetButtonDown(ButtonName.Shoulder) && !peekCorner)
        {
            aimCamOffset.x = aimCamOffset.x * (-1);
            aimPivotOffset.x = aimPivotOffset.x * (-1);
        }
        behaviourController.GetAnimator.SetBool(aimBool, aim);

    }
    private void OnGUI()
    {
        if(crossHair != null)
        {
            float length = behaviourController.GetCamScript.GetCurrentPivotMagitude(aimPivotOffset);
            if(length < 0.05f)
            {
                GUI.DrawTexture(new Rect(Screen.width * 0.5f - (crossHair.width * 0.5f), Screen.height * 0.5f - (crossHair.height * 0.5f), crossHair.width, crossHair.height), crossHair);
            }
        }
    }
}
