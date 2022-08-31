using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertChecker : MonoBehaviour
{
    [Range(0,50)]public float alertRadious;
    public int extraWaves = 1;
    public LayerMask alertMask = KJY.TagAndLayer.LayerMasking.Enemy;
    private Vector3 current;
    private bool alert;

    private void Start()
    {
        InvokeRepeating("PingAlert", 1, 1);//1�� �������� ������ �ݺ������� ����
    }
    private void AlertNearBy(Vector3 origin, Vector3 target, int wave = 0)
    {
        if(wave > this.extraWaves)
        {
            return;
        }
        //�� �ݰ� alretRadius��ŭ �ȿ� �ִ� ���ʹ̶�� ���̾� ����ũ�� ���� ���ӿ�����Ʈ�� ��� ������.
        Collider[] targetInViewRadius = Physics.OverlapSphere(origin, alertRadious, alertMask); 
        foreach(Collider obj in targetInViewRadius)
        {
            obj.SendMessageUpwards("AlertCallback", target, SendMessageOptions.DontRequireReceiver);
            AlertNearBy(obj.transform.position, target, wave + 1);
        }
    }
    public void RootAlertNearBy(Vector3 origin)
    {
        current = origin;
        alert = true;
    }
    void PingAlert()
    {
        if(alert)
        {
            alert = false;
            AlertNearBy(current, current);
        }
    }
}
