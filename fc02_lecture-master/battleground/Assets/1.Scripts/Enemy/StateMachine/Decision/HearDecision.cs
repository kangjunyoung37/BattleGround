using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// alertCheck�� ���� ��� ����ų� (�ѼҸ��� ����ų�)
/// Ư�� �Ÿ����� �þ߰� �����־ Ư�� ��ġ���� Ÿ���� ��ġ�� ������ �����Ǿ������
/// ����ٶ�� �Ǵ�
/// </summary>

[CreateAssetMenu(menuName ="PluggableAI/Decisions/Hear")]
public class HearDecision : Decision
{
    private Vector3 lastPos, currentPos;
    public override void OnEnableDecision(StateController controller)
    {
        //�ʱ�ȭ
        lastPos = currentPos = Vector3.positiveInfinity;
        

    }
    private bool MyHandleTargets(StateController controller, bool hasTarget, Collider[] targetInHearRadius)
    {
        if(hasTarget)
        {
            currentPos = targetInHearRadius[0].transform.position;
            if(!Equals(lastPos, Vector3.positiveInfinity))
            {
                if (!Equals(lastPos, currentPos))
                {
                    controller.personalTarget = currentPos;
                    return true;
                }
            }
            lastPos = currentPos;
        }
        return false;
    }
    public override bool Decide(StateController controller)
    {
        if(controller.variables.healAlert)
        {
            controller.variables.healAlert = false;
            return true;
        }
        else
        {
            return CheckTargetsInRadius(controller, controller.perceptionRadius, MyHandleTargets);
        }
    }
}
