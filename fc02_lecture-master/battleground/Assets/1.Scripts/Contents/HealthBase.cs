using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBase : MonoBehaviour
{
    public class DamageInfo
    {
        public Vector3 location, direction;
        public float damage;
        public Collider bodyPart;//��� �¾Ҵ��� �˰� ������
        public GameObject orgin;//�ǰ�����Ʈ

        public DamageInfo(Vector3 location,Vector3 direction, float damage, Collider bodyPart = null , GameObject origin = null)
        {
            this.location = location;
            this.direction = direction;
            this.bodyPart = bodyPart;
            this.damage = damage;
            this.orgin = origin;
        }
    }
    [HideInInspector]public bool IsDead;
    protected Animator myAnimator;
    public virtual void TakdDamage(Vector3 location, Vector3 direction,float damage, Collider bodyPart = null, GameObject origin = null)
    {

    }
    public void HitCallBack(DamageInfo damageInfo)
    {
        this.TakdDamage(damageInfo.location, damageInfo.direction, damageInfo.damage, damageInfo.bodyPart, damageInfo.orgin);
    }


}
