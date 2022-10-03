using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인지 타입에 따라 특정 거리로 부터 가깝진 않지만 시야가 막히지 않았고 위험요소를 감지했거나
/// 너무 가까운 거리에 타켓이 있는지 판단
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
    [Tooltip("어떤 크기로 위험요소 감지를 하겠습니까")]
    public Sense sense;

    [Tooltip("현재 엄폐물을 해제할까여?")]
    public bool invalidateCoverSpot;

    private float Radius;//센스에 따른 범위 
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
        //타켓이 존재하고 시야가 막히지 않았다면
        if(hasTarget && !controller.BlockedSight())
        {
            if(invalidateCoverSpot)
            {
                controller.CoverSpot = Vector3.positiveInfinity;//쓰레기값
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
