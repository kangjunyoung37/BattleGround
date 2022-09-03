using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� Ÿ�Կ� ���� Ư�� �Ÿ��� ���� ������ ������ �þ߰� ������ �ʾҰ� �����Ҹ� �����߰ų�
/// �ʹ� ����� �Ÿ��� Ÿ���� �ִ��� �Ǵ�
/// </summary>
[CreateAssetMenu(menuName ="PluggableAI/Decisions/Focus")]
public class FocusDecision : Decision
{
    public enum Sense
    {
        NEAR,
        PERCEPTION,
        VIEW,
    }
    [Tooltip("� ũ��� ������ ������ �ϰڽ��ϱ�")]
    public Sense sense;

    [Tooltip("���� ������ �����ұ?")]
    public bool invalidateCoverSpot;

    private float Radius;//������ ���� ���� 
    public override void OnEnableDecision(StateController controller)
    {
        switch(sense)
        {
            case Sense.NEAR:
                Radius = controller.nearRadius;
                break;
            case Sense.PERCEPTION:
                Radius = controller.perceptionRadius;
                break;
            case Sense.VIEW:
                Radius = controller.viewRadius;
                break;
            default:
                Radius = controller.nearRadius;
                break;
        }
    }
    private bool MyHandleTargets(StateController controller,bool hasTarget, Collider[] targetInHearRadius)
    {
        //Ÿ���� �����ϰ� �þ߰� ������ �ʾҴٸ�
        if(hasTarget && !controller.BlockedSight())
        {
            if(invalidateCoverSpot)
            {
                controller.CoverSpot = Vector3.positiveInfinity;//�����Ⱚ
            }
            controller.targetInSight = true;
            controller.personalTarget = controller.aimTarget.position;
            return true;
        }
        return false;
    }
    public override bool Decide(StateController controller)
    {
        return (sense != Sense.NEAR && controller.variables.feelAlert && !controller.BlockedSight()) || Decision.CheckTargetsInRadius(controller, Radius, MyHandleTargets);
    }
}
