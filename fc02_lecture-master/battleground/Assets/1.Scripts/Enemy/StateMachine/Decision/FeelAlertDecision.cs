using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ��� �����ߴ°�? �÷��װ� ���� ���·� �Ǵ�
/// </summary>

[CreateAssetMenu(menuName = "PluggableAI/Decisions/FeelAlert")]
public class FeelAlertDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        return controller.variables.feelAlert;
    }
}
