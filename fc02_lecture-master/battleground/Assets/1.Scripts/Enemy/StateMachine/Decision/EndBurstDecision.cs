using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ѹ� ����� ���۵Ǹ� �ѹ��� �� �� �ִ� ������ ������ �� �� �ִ� �Ѿ��� ���� �Ǵ�
/// </summary>

[CreateAssetMenu(menuName ="PluggableAI/Decisions/EndBurst")]
public class EndBurstDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        //����� ���� ������ ������(Wait)
        return controller.variables.currentShots >= controller.variables.shotsInRounds;
    }
}
