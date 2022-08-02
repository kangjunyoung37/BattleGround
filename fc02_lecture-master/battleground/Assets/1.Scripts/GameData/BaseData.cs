using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// data�� �⺻ Ŭ����]
/// �������� �����͸� ������ �ְ� �ǰ�, �̸��� ������ ����
/// �������� ������ �̸��� ��� ����Ʈ�� ���� �� �ִ�.
/// </summary>
public class BaseData : ScriptableObject
{
    public const string dataDirectory = "/9.ResourcesData/Resources/Data/";
    public string[] names = null;

    public BaseData()
    {

    }
    public int GetDataCount()
    {

        int retValue = 0;
        if(this.names != null)
        {
            retValue = this.names.Length;
        }
        return retValue;

    }
    /// <summary>
    /// ���� ����ϱ� ���� �̸� ����� ������ִ� �Լ� 
    /// </summary>
    public string[] GetNameList(bool showId,string filterWord= " ")
    {
        string[] retList = new string[0];
        if(this.names == null)
        {
            return retList;
        }
        retList = new string[this.names.Length];
        for(int i = 0; i <this.names.Length; i++)
        {
            //if(filterWord != "")
            //{
            //    if (names[i].ToLower().Contains(filterWord.ToLower())== false)
            //    {
            //        continue;
            //    }
            //}���͸� ��Ű��
            if(showId)
            {
                retList[i] = i.ToString() + " : " + this.names[i];
            }
            else
            {
                retList[i] = this.names[i];
            }
        }

        return retList;
    }
    public virtual int AddData(string newName)
    {
        return GetDataCount();
    }
    public virtual void RemoveData(int index)
    {

    }
    public virtual void Copy(int index)
    {

    }

}
