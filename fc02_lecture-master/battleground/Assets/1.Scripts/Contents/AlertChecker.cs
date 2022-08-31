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
        InvokeRepeating("PingAlert", 1, 1);//1초 간격으로 실행을 반본적으로 실행
    }
    private void AlertNearBy(Vector3 origin, Vector3 target, int wave = 0)
    {
        if(wave > this.extraWaves)
        {
            return;
        }
        //구 반경 alretRadius만큼 안에 있는 에너미라는 레이어 마스크를 가진 게임오브젝트에 경고를 보내라.
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
