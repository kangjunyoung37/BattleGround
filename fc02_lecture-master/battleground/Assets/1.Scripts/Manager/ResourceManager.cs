using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

/// <summary>
/// Resource.Load를 래핑하는 클래스
/// 나중에 어셋번들도 변경
/// </summary>
public class ResourceManager
{
    public static UnityObject Load(string path)
    {
        return Resources.Load(path);
    }
    public static GameObject LoadAndInstiate(string path)
    {
        UnityObject source = Load(path);
        if(source == null)
        {
            return null;
        }
        return GameObject.Instantiate(source) as GameObject;

    }


}
