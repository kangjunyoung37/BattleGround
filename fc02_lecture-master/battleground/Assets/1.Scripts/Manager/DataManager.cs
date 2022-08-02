using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private static EffectData effectData = null;

    void Start()
    {
        if(effectData == null)
        {
            effectData = ScriptableObject.CreateInstance<EffectData>();
            effectData.LoadData();

        }
    }
    public static EffectData EffectData()
    {
        if(effectData == null)
        {
            effectData = ScriptableObject.CreateInstance<EffectData>();
        }
        return effectData;
    }

  
}
