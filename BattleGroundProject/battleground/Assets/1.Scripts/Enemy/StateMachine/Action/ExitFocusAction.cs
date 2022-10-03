using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Ÿ������ Ǯ��
/// </summary>
[CreateAssetMenu(menuName ="PluggableAI/Actions/Exit Focus")]
public class ExitFocusAction : Action
{
    public override void OnReadyAction(StateController controller)
    {
        controller.focusSight = false;
        controller.variables.feelAlert = false;
        controller.variables.healAlert = false;
        controller.Strafing = false;
        controller.nav.destination = controller.personalTarget;
        controller.nav.speed = 0;
    }
    public override void Act(StateController controller)
    {

    }
}
