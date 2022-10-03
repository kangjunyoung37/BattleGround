using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ݰ� ���ÿ� �̵��ϴ� Action 
/// ȸ���Ҷ��� ȸ���� �ϰ� ȸ���� �������� starfing�� Ȱ��ȭ
/// Attack �ϸ鼭 ���ÿ� �Ͼ
/// </summary>
/// 
[CreateAssetMenu (menuName = "PluggableAI/Actions/Focus Move")]
public class FocusMoveAction : Action
{
    public ClearShotDecision clearShotDecision;

    private Vector3 currentDest;
    private bool aligned;//Ÿ�ٰ� ������ �Ǿ��°�
    public override void OnReadyAction(StateController controller)
    {
        controller.hadClearShot = controller.haveClearShot = false;
        currentDest = controller.nav.destination;
        controller.focusSight = true;
        aligned = false;
    }
    public override void Act(StateController controller)
    {
        if(!aligned)
        {
            controller.nav.destination = controller.personalTarget;
            controller.nav.speed = 0f;
            if(controller.enemyAnimation.angularSpeed == 0f)
            {
                controller.Strafing = true;
                aligned = true;
                controller.nav.destination = currentDest;
                controller.nav.speed = controller.generalStates.evadeSpeed;
            }
        }
        else
        {
            controller.haveClearShot = clearShotDecision.Decide(controller);
            if(controller.hadClearShot != controller.haveClearShot)
            {
                controller.Aiming = controller.haveClearShot;
                //����� �����ϴٸ� ���� �̵� ��ǥ�� ���󹰰� �ٸ����� �ϴ� �̵����� ���ƶ�                
                if(controller.haveClearShot && !Equals(currentDest,controller.CoverSpot))
                {
                    controller.nav.destination = controller.transform.position;
                }
            }
            controller.hadClearShot = controller.haveClearShot;
        }
    }
}
