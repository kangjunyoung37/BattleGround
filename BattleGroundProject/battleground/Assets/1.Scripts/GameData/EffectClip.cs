using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 이펙트 프리펩과 경로와 이펙트 타입등의 속성데이터를 가지고 있게 되며
/// 프리펩 사전로딩 기능을 갖고 있고 -풀링을 위한 기능이기도 합니다.
/// 이펙트 인스턴스 기능도 갖고 있으며 - 풀링과 연계해서 사용
/// </summary>
public class EffectClip
{
    //추후 속성은 같지만 다른 이펙트 클립이 있을 수 있어서 분별용
    public int realId = 0;
    
    public EffectType effectType = EffectType.NORMAL;
    public GameObject effectPrefab = null;
    public string effectName = string.Empty;
    public string effectPath = string.Empty;
    public string effectFullPath = string.Empty;
    public EffectClip()
    {

    }
    public void PreLoad()
    {
        this.effectFullPath = effectPath + effectName;
        if(this.effectFullPath != string.Empty && this.effectPrefab == null)
        {
            this.effectPrefab = ResourceManager.Load(effectFullPath) as GameObject;
        }
    }
    public void ReleaseEffect()
    {
        if(this.effectPrefab != null)
        {
            this.effectPrefab = null;//null로 바꿔줌으로 가비지 콜렉터에서 알아서 메모리 해제
        }
    }

    /// <summary>
    /// 원하는 위치에 내가 원하는 이펙트를 생성합니다.
    /// </summary>
    public GameObject Instatiate(Vector3 pos)
    {
        if (this.effectPrefab == null)
        {
            this.PreLoad();
        }
        if(this.effectPrefab != null)
        {
            GameObject effect = GameObject.Instantiate(effectPrefab,pos,Quaternion.identity);
            return effect;
        }
        return null;
    }

}
