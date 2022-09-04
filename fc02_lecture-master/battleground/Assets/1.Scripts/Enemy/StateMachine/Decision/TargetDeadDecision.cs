using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ÿ���� �׾����� üũ�ϴ� Decision
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
            Debug.LogError("HealthBase�� �ٿ��ּ���" + controller.name, controller.gameObject);
        }
        return false;
    }

}
