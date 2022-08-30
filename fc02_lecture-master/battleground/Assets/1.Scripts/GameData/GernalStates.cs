using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName ="PluggableAI/GeneralStats")]
public class GeneralStates : ScriptableObject
{
    [Header("General")]
    [Tooltip("npc ���� �ӵ� clear state")]
    public float patrolSpeed = 2f;
    [Tooltip("npc ������� �ӵ� warning state")]
    public float chaseSpeed = 5f;
    [Tooltip ("npc ȸ���ϴ� �ӵ� engage state")]
    public float evadeSpeed = 15f;
    [Tooltip("��������Ʈ���� ����ϴ� �ð�")]
    public float patrolWaitTime = 2f;

    [Header("Animation")]
    [Tooltip("��ֹ� ���̾� ����ũ")]
    public LayerMask obstacleMask;
    [Tooltip("���ؽ� �������� ���ϱ����� �ּ� Ȯ�� �ޱ�")]
    public float angleDeadZone = 5f;
    [Tooltip("�ӵ� ���� �ð�")]
    public float speedDampTime = 0.4f;
    [Tooltip("���ҵ� ���� �ð�")]
    public float angularSpeedDampTime = 0.2f;
    [Tooltip("���ӵ��ȿ��� ���� ȸ���� ���� ���� �ð�")]
    public float angleResponseTime = 0.2f;

    [Header("Cover")]
    [Tooltip("��ֹ��� ������ �� ����ؾ��� �ּ� ���� ��")]
    public float aboveCoverHeight = 1.5f;
    [Tooltip("��ֹ� ���̾� ����ũ")]
    public LayerMask coverMask;
    [Tooltip("��� ���̾� ����ũ")]
    public LayerMask shotMask;
    [Tooltip("Ÿ�� ���̾� ����ũ")]
    public LayerMask targetMask;




}
