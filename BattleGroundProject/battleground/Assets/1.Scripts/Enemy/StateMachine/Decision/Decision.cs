using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ü���ϴ� Ŭ����
/// ���� üũ�� ���� Ư�� ��ġ�κ��� ���ϴ� �˻� �ݰ濡 �ִ� �浹ü�� ã�Ƽ�
/// �� �ȿ� Ÿ���� �ִ��� Ȯ��
/// </summary>
public abstract class Decision : ScriptableObject
{
    public abstract bool Decide(StateController controller);
    
    public virtual void OnEnableDecision(StateController controller)
    {

    }
    public delegate bool HandleTargets(StateController controller, bool hasTargets, Collider[] tagetInRadius);

    public static bool CheckTargetsInRadius(StateController controller, float radius,HandleTargets handleTargets)
    {
        if(controller.aimTarget.root.GetComponent<HealthBase>().IsDead)
        {
            return false;
        }
        else
        {
            Collider[] targetsInRadius = Physics.OverlapSphere(controller.transform.position, radius, controller.generalStates.targetMask);
            return handleTargets(controller, targetsInRadius.Length > 0, targetsInRadius);
        }
    }
}
