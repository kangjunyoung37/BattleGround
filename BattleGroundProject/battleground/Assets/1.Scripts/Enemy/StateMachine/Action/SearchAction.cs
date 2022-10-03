using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 타겟이 있다면 타켓까지 이동하지만, 타켓을 잃었다면 가만히 서있습니다.
/// </summary>
/// 

[CreateAssetMenu(menuName ="PluggableAI/Actions/Search")]
public class SearchAction : Action
{

    //초기화
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
