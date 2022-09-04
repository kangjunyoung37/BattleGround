using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// alertCheck를 통해 경고를 들었거나 (총소리를 들었거나)
/// 특정 거리에서 시야가 막혀있어도 특정 위치에서 타켓의 위치가 여러번 인지되었을경우
/// 들었다라고 판단
/// </summary>

[CreateAssetMenu(menuName ="PluggableAI/Decisions/Hear")]
public class HearDecision : Decision
{
    private Vector3 lastPos, currentPos;
    public override void OnEnableDecision(StateController controller)
    {
        //초기화
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
