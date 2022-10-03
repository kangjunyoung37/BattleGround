using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����Ʈ ������� ��ο� ����Ʈ Ÿ�Ե��� �Ӽ������͸� ������ �ְ� �Ǹ�
/// ������ �����ε� ����� ���� �ְ� -Ǯ���� ���� ����̱⵵ �մϴ�.
/// ����Ʈ �ν��Ͻ� ��ɵ� ���� ������ - Ǯ���� �����ؼ� ���
/// </summary>
public class EffectClip
{
    //���� �Ӽ��� ������ �ٸ� ����Ʈ Ŭ���� ���� �� �־ �к���
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
            this.effectPrefab = null;//null�� �ٲ������� ������ �ݷ��Ϳ��� �˾Ƽ� �޸� ����
        }
    }

    /// <summary>
    /// ���ϴ� ��ġ�� ���� ���ϴ� ����Ʈ�� �����մϴ�.
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
