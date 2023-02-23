using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

public class ObjectManager 
{
    private GameObject _root;
    private Dictionary<string, GameObject> singualrObject = new Dictionary<string, GameObject>();
    private Dictionary<string, List<GameObject>> poolList = new Dictionary<string, List<GameObject>>();
    private Dictionary<string, GameObject> objectPoolList = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> folderObjectList = new Dictionary<string, GameObject>();

    /// <summary>
    /// �ܵ����� ��� ������Ʈ ����.
    /// </summary>
    /// <param name="AssetName">��巹���� �̸�</param>
    /// <param name="pos">������ ��ǥ</param>
    /// <returns></returns>
    public GameObject InstantiateSingle(string AssetName, Vector2 pos, GameObject parent = null)
    {
        GameObject rtngo = null;
        //root ������Ʈ�� ������ ���� ����.
        if (_root == null)
        {
            _root = new GameObject { name = "@ObjectManager" };
        }
        GameObject Parent = (parent == null) ? _root : parent;


        Managers.Resource.Instantiate(AssetName, Parent.transform, (success) => {
            success.transform.position = pos;
            singualrObject.Add(AssetName, success);
            rtngo = success;
        });
        return rtngo;
    }

    /// <summary>
    /// ������Ʈ Ǯ�� ���� posList�� ���� ���� ���� ������.
    /// </summary>
    /// <param name="AssetName">��巹���� �̸�</param>
    /// <param name="posList">��ǥ�� ����Ʈ</param>
    /// <returns></returns>
    public List<GameObject> InstantiateAsync(string AssetName, List<Vector2> posList)
    {
        List<GameObject> rtnGo = new List<GameObject>();
        int ammount = posList.Count;
        //root ������Ʈ�� ������ ���� ����.
        if (_root == null)
        {
            _root = new GameObject { name = "@ObjectManager" };
        }
        //�ش� ������Ʈ�� Ǯ�� ����Ʈ�� �ִ��� üũ.
        for (int i = 0; i < ammount; i++)
        {
            var PoolObj = TaskGet(AssetName);
            if (PoolObj != null)
            {
                PoolObj.SetActive(true);
                PoolObj.transform.position = posList[i];
                rtnGo.Add(PoolObj);
            }
        }
        //����Ʈ�� 1�̶� �ִٸ�, �����Ա� ������ ����.
        if (rtnGo.Count != 0) return rtnGo;

        //�ƿ� ��ϵ� ���� �ʾ��� ��.
        rtnGo = RegisterObject(AssetName, ammount);
        for (int i = 0; i < rtnGo.Count; i++)
        {
            rtnGo[i].SetActive(true);
            rtnGo[i].transform.position = posList[i];
        }
        return rtnGo;

    }
    /// <summary>
    /// ������Ʈ Ǯ�� ���� �ϳ��� ����Ҷ�
    /// </summary>
    /// <param name="AssetName">��巹���� �̸�</param>
    /// <param name="pos">��ǥ��</param>
    /// <returns></returns>
    public  GameObject InstantiateAsync(string AssetName, Vector2 pos)
    {
        GameObject rtnGo = null;
        //root ������Ʈ�� ������ ���� ����.
        if (_root == null)
        {
            _root = new GameObject { name = "@ObjectManager" };
        }

        var PoolObj =  TaskGet(AssetName);
        if (PoolObj != null)
        {
            PoolObj.SetActive(true);
            PoolObj.transform.position = pos;
            rtnGo = PoolObj;
        }

        //����Ʈ�� 1�̶� �ִٸ�, �����Ա� ������ ����.
        if (rtnGo != null) return rtnGo;

        //�ƿ� ��ϵ� ���� �ʾ��� ��.
        List<GameObject> listGo = RegisterObject(AssetName, 1);
        rtnGo = listGo[0];
        rtnGo.SetActive(true);
        rtnGo.transform.position = pos;

        return rtnGo;

    }

    /// <summary>
    /// Ǯ���� ������Ʈ�� ��ϸ���
    /// </summary>
    /// <param name="AssetName"></param>
    /// <param name="ammount"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public List<GameObject> RegisterObject(string AssetName, int ammount, Action callback = null)
    {
        List<GameObject> rtnList = new List<GameObject>();
        if (poolList.ContainsKey(AssetName)) return poolList[AssetName];

        Managers.Resource.LoadResource<GameObject>(AssetName, (success) =>
        {

            objectPoolList.Add(AssetName, success);

            GameObject folder = new GameObject();
            folder.name = "@" + AssetName;
            folder.transform.SetParent(_root.transform,true);
            folderObjectList.Add(AssetName, folder);

            for (int i = 0; i < ammount; i++)
            {
                GameObject inst = Object.Instantiate(success);
                inst.SetActive(false);
                inst.transform.SetParent(folder.transform,true);
                rtnList.Add(inst);
            }
            poolList.Add(AssetName, rtnList);
            callback?.Invoke();
        });
        return rtnList;

    }

    /// <summary>
    /// �ܵ� ������Ʈ�� �ҷ���
    /// </summary>
    /// <param name="name">�����̸�</param>
    /// <returns></returns>
    public GameObject GetSingularObjet(string name)
    {
        if (!singualrObject.ContainsKey(name))
        {
            return null;
        }
        return singualrObject[name];

    }

    /// <summary>
    /// ��ϵ� ������ƮǮ�� ����Ʈ�� ������
    /// </summary>
    /// <param name="name">�����̸�</param>
    /// <returns></returns>
    public List<GameObject> GetPoolObject(string name)
    {
        if (!poolList.ContainsKey(name))
        {
            return null;
        }
        return poolList[name];
    }

    private GameObject TaskGet(string name)
    {
        if (!objectPoolList.ContainsKey(name))
        {
            return null;
        }
        foreach (var item in poolList[name])
        {
            if (item.gameObject.activeInHierarchy == true)
                continue;

            return item.gameObject;

        }
        // ��� object�� false �������� �ǹ���. 
        // �׷��� ���� �������� ��.
        GameObject inst = null;
        Managers.Resource.LoadResource<GameObject>(name, (sucess) =>
        {
            GameObject folder = GameObject.Find("@" + name);

            inst = Object.Instantiate(sucess);
            inst.SetActive(false);
            inst.transform.SetParent(folder.transform,true);

            poolList[name].Add(inst);
        });
        return inst;
    }

    public void RemoveAll()
    {
        this.folderObjectList.Clear();
        this.objectPoolList.Clear();
        this.poolList.Clear();
        this.singualrObject.Clear();
    }

    /// <summary>
    /// �ش� ������Ʈ Ǯ���� ���� �ϰ� ���ҽ��� ���� ��. 
    /// </summary>
    /// <param name="AssetName"></param>
    /// <returns></returns>
    public void RemoveObjectPool(string AssetName)
    {
        GameObject folder;
        if (folderObjectList.TryGetValue(AssetName, out folder))
        {
            Object.Destroy(folder);
            folderObjectList.Remove(AssetName);
            objectPoolList.Remove(AssetName);
            poolList.Remove(AssetName);
            //Managers.Resource.Release(AssetName);
        }
    }
    /// <summary>
    /// �θ� �ٲ� Pool Object�� ��ȯ�ϴ� �ڵ� 
    /// </summary>
    /// <param name="rtnGo"></param>
    public void ReturnToParent(GameObject rtnGo)
    {
        GameObject Parentfolder;
        string rtnGoAddressable = rtnGo.name.Replace("(Clone)", "").Trim();
        if (folderObjectList.TryGetValue(rtnGoAddressable, out Parentfolder))
        {
            rtnGo.transform.SetParent(Parentfolder.transform);
            rtnGo.SetActive(false);
        }
    }

}