using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 더블 체크를 하고 근처에 장애물이나 엄폐물이 가깝게 있는지 체크
/// 타겟 목표까지 장애물이나 엄폐물이 있는지 체크 만약 충돌 검출된 충돌체가 플레이어라면 막힌게 없다는 뜻
/// </summary>

[CreateAssetMenu(menuName =("PluggableAI/Decisions/ClearShot"))]
public class ClearShotDecision : Decision
{
    [Header("Extra Decisions")]
    public FocusDecision targetNear;

    private bool HaveClearShot(StateController controller)
    {
        Vector3 shotOrigin = controller.transform.position + Vector3.up * (controller.generalStates.aboveCoverHeight + controller.nav.radius);
        Vector3 shotDirection = controller.personalTarget - shotOrigin;

        bool blockedShot = Physics.SphereCast(shotOrigin, controller.nav.radius, shotDirection, out RaycastHit hit, controller.nearRadius, controller.generalStates.coverMask | controller.generalStates.obstacleMask);
        if(!blockedShot)
        {
            blockedShot = Physics.Raycast(shotOrigin, shotOrigin, out hit, shotDirection.magnitude, controller.generalStates.coverMask | controller.generalStates.obstacleMask);
            if(blockedShot)
            {
                blockedShot = (hit.transform.root == controller.aimTarget.root);//플레이어라는 뜻
            }
        }
        return !blockedShot;
    }
    public override bool Decide(StateController controller)
    {
        return targetNear.Decide(controller) || HaveClearShot(controller);
    }

}
