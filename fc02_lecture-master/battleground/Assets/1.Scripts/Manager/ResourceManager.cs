using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

/// <summary>
/// Resource.Load�� �����ϴ� Ŭ����
/// ���߿� ��¹��鵵 ����
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
