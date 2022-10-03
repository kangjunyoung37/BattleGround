using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 타겟이 죽었는지 체크하는 Decision
/// </summary>
/// 
[CreateAssetMenu(menuName = "PluggableAI/Decisions/TargetDead")]
public class TargetDeadDecision : Decision
{

    public override bool Decide(StateController controller)
    {
        try
        {
            return controller.aimTarget.root.GetComponent<HealthBase>().IsDead;
        }
        catch(UnassignedReferenceException)
        {
            Debug.LogError("HealthBase를 붙여주세여" + controller.name, controller.gameObject);
        }
        return false;
    }

}
