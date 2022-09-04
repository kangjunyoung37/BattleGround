using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ֹ��� �̵��� �� �ִ� ��Ȳ���� �ƴ��� �Ǵ�
/// ������ �Ѿ��� ���� �ְų�, ���󹰷� �̵��ϱ� ���� ��� �ð��� �����ְų�
/// ���࿡ �������� ������ ���� ���� ���з� ����
/// �׿ܿ��� ���󹰷� �̵�
/// </summary>

[CreateAssetMenu(menuName = "PluggableAI/Decisions/TakeCover")]

public class TakeCoverDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        //���� ������ �Ѿ��� �����ְų�, ��� �ð��� �� �ʿ��ϰų� ������ġ�� ��ã�Ҵٸ� false
        if (controller.variables.currentShots < controller.variables.shotsInRounds || controller.variables.waitInCoverTime > controller.variables.coverTime || Equals(controller.CoverSpot,Vector3.positiveInfinity))
        {
            return false;
        }
        else
        {
            return true;
        }
    } 
}
