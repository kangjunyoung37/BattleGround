using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ���� üũ�� �ϰ� ��ó�� ��ֹ��̳� ������ ������ �ִ��� üũ
/// Ÿ�� ��ǥ���� ��ֹ��̳� ������ �ִ��� üũ ���� �浹 ����� �浹ü�� �÷��̾��� ������ ���ٴ� ��
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
                blockedShot = (hit.transform.root == controller.aimTarget.root);//�÷��̾��� ��
            }
        }
        return !blockedShot;
    }
    public override bool Decide(StateController controller)
    {
        return targetNear.Decide(controller) || HaveClearShot(controller);
    }

}
