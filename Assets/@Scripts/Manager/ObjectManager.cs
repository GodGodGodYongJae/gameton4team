using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{

    private GameObject _root;
    private Dictionary<string, GameObject> _singualrObject = new Dictionary<string, GameObject>();

    private Dictionary<string, List<GameObject>> _poolList = new Dictionary<string, List<GameObject>>();

    private Dictionary<string, GameObject> _objectPoolList = new Dictionary<string, GameObject>();

/// <summary>
/// ������Ʈ ����
/// </summary>
/// <param name="AssetName">������ ������Ʈ ��巹���� �̸�</param>
/// <param name="pos">������ ��ǥ</param>
/// <param name="pool">Ǯ���Ұ���</param>
/// <param name="ammount">Ǯ���� ����</param>
    public void Instantiate(string AssetName, List<Vector2> posList, bool pool = false, int ammount = 1)
    {
        //root ������Ʈ�� ������ ���� ����.
        if(_root == null)
        {
            _root = new GameObject { name = "@ObjectManager" };
        }

        //������ ������ ����� ���� ������, Debug Error�� ���ش�.
        if (posList.Count != ammount)
            Debug.LogError("������ ���� �ʽ��ϴ�. ���� ���� :" + posList.Count + " ������ ���� : " + ammount);
 

        //�ش� ������Ʈ�� Ǯ�� ����Ʈ�� �ִ��� üũ.
        for (int i = 0; i < ammount; i++)
        {
            GameObject PoolObj = Get(AssetName);
            if (PoolObj != null)
            {
                PoolObj.SetActive(true);
                PoolObj.transform.position = posList[i];
            }
        }
     

        //Ǯ���� ������Ʈ�� �ƴ϶��, ��巹���� �̸����� ã�Ƽ� �������.
        if(pool == false)
        {
            GameObject go = GetSingularObjet(AssetName);
            if (go != null)
            {
                go.transform.position = posList[0];
                go.SetActive(true);
                return;

            }
            Managers.Resource.Instantiate(AssetName, _root.transform, (success) => {
                success.transform.position = posList[0];
                _singualrObject.Add(AssetName, success);
            });
            return;
        }

        RegisterObject(AssetName, ammount, () => {
            for (int i = 0; i < ammount; i++)
            {
                Get(AssetName).SetActive(true);
            }
        });
    }
    /// <summary>
    /// ������Ʈ ����2
    /// </summary>
    /// <param name="AssetName">������ ������Ʈ ��巹���� �̸�</param>
    /// <param name="pos">������ ��ǥ</param>
    /// <param name="pool">Ǯ���Ұ���</param>
    /// <param name="ammount">Ǯ���� ����</param>
    public void Instantiate(string AssetName, Vector2 pos, bool pool = false, int ammount = 1)
    {
        //root ������Ʈ�� ������ ���� ����.
        if (_root == null)
        {
            _root = new GameObject { name = "@ObjectManager" };
        }

        //������ ������ ����� ���� ������, Debug Error�� ���ش�.



        //�ش� ������Ʈ�� Ǯ�� ����Ʈ�� �ִ��� üũ.
        for (int i = 0; i < ammount; i++)
        {
            GameObject PoolObj = Get(AssetName);
            if (PoolObj != null)
            {
                PoolObj.SetActive(true);
                PoolObj.transform.position = pos;
            }
        }


        //Ǯ���� ������Ʈ�� �ƴ϶��, ��巹���� �̸����� ã�Ƽ� �������.
        if (pool == false)
        {
            GameObject go = GetSingularObjet(AssetName);
            if (go != null)
            {
                go.transform.position = pos;
                go.SetActive(true);
                return;

            }
            Managers.Resource.Instantiate(AssetName, _root.transform,(success)=> {
                success.transform.position = pos;
                _singualrObject.Add(AssetName, success);
            });
            return;
        }

        RegisterObject(AssetName, ammount,()=> {
            for (int i = 0; i < ammount; i++)
            {
                Get(AssetName).SetActive(true);
            }
        });
        
    }

    public void RegisterObject(string AssetName,int ammount, Action callback = null)
    {
        Managers.Resource.LoadAsync<GameObject>(AssetName,(sucess)=> {
            
            _objectPoolList.Add(AssetName,sucess);
            
            GameObject folder = new GameObject();
            folder.name = "@"+AssetName;
            folder.transform.parent = _root.transform;
            List<GameObject> list = new List<GameObject>();
            
            for (int i = 0; i < ammount; i++)
            {
                GameObject inst = Instantiate(sucess);
                inst.SetActive(false);
                inst.transform.parent = folder.transform;
                list.Add(inst);
            }
            _poolList.Add(AssetName, list);
            callback?.Invoke();
        });
    
    }

    public GameObject GetSingularObjet(string name)
    {
        if(!_singualrObject.ContainsKey(name))
        {
            return null;
        }
        return _singualrObject[name];
    }

    public GameObject Get(string name)
    {
        if(!_objectPoolList.ContainsKey(name))
        {
            return null;
        }
        foreach (var item in _poolList[name])
        {
            if (item.gameObject.activeInHierarchy == true) 
                continue;

            return item.gameObject;
            
        }

        // ��� object�� false �������� �ǹ���. 
        // �׷��� ���� �������� ��.
        GameObject inst = null;
        Managers.Resource.LoadAsync<GameObject>(name, (sucess) =>
        {
            GameObject folder = GameObject.Find("@"+name);
            
            inst = Instantiate(sucess);
            inst.SetActive(false);
            inst.transform.parent = folder.transform;

            _poolList[name].Add(inst);

       
        });

        return inst;

    }
}
