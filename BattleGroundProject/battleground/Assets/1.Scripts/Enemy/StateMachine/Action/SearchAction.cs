using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Ÿ���� �ִٸ� Ÿ�ϱ��� �̵�������, Ÿ���� �Ҿ��ٸ� ������ ���ֽ��ϴ�.
/// </summary>
/// 

[CreateAssetMenu(menuName ="PluggableAI/Actions/Search")]
public class SearchAction : Action
{

    //�ʱ�ȭ
    public override void OnReadyAction(StateController controller)
    {
        controller.focusSight = false;
        controller.enemyAnimation.AbortPendingAim();
        controller.enemyAnimation.anim.SetBool(KJY.AnimatorKey.Crouch, false);
        controller.CoverSpot = Vector3.positiveInfinity;
    }
    public override void Act(StateController controller)
    {
        if(Equals(controller.personalTarget,Vector3.positiveInfinity))
        {
            controller.nav.destination = controller.transform.position;
        }
        else
        {
            controller.nav.speed = controller.generalStates.chaseSpeed;
            controller.nav.destination = controller.personalTarget;
        }
    }
}
