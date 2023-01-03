using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{

    private GameObject _root;
    private Dictionary<string, GameObject> _singualrObject = new Dictionary<string, GameObject>();

    private Dictionary<string, List<GameObject>> _poolList = new Dictionary<string, List<GameObject>>();

    private Dictionary<string, GameObject> _objectPoolList = new Dictionary<string, GameObject>();

    /// <summary>
    /// �ܵ����� ��� ������Ʈ ����.
    /// </summary>
    /// <param name="AssetName">��巹���� �̸�</param>
    /// <param name="pos">������ ��ǥ</param>
    /// <returns></returns>
    public async UniTask<GameObject> InstantiateSingle(string AssetName, Vector2 pos)
    {
        GameObject rtngo = null;
        bool isTask = false;
        //root ������Ʈ�� ������ ���� ����.
        if (_root == null)
        {
            _root = new GameObject { name = "@ObjectManager" };
        }

        Managers.Resource.Instantiate(AssetName, _root.transform, (success) => {
            success.transform.position = pos;
            _singualrObject.Add(AssetName, success);
            rtngo = success;
            isTask = true;
        });
        await UniTask.WaitUntil(() => { return isTask == true; });
        return rtngo;
    }
    /// <summary>
    /// ������Ʈ Ǯ�� ���� posList�� ���� ���� ���� ������.
    /// </summary>
    /// <param name="AssetName">��巹���� �̸�</param>
    /// <param name="posList">��ǥ�� ����Ʈ</param>
    /// <returns></returns>
    public async UniTask<List<GameObject>> InstantiateAsync(string AssetName, List<Vector2> posList)
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
            var PoolObj = await TaskGet(AssetName);
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
        rtnGo = await RegisterObject(AssetName, ammount);
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
    public async UniTask<GameObject> InstantiateAsync(string AssetName, Vector2 pos)
    {
        GameObject rtnGo = null;
        //root ������Ʈ�� ������ ���� ����.
        if (_root == null)
        {
            _root = new GameObject { name = "@ObjectManager" };
        }

        var PoolObj = await TaskGet(AssetName);
        if (PoolObj != null)
        {
            PoolObj.SetActive(true);
            PoolObj.transform.position = pos;
            rtnGo = PoolObj;
        }

        //����Ʈ�� 1�̶� �ִٸ�, �����Ա� ������ ����.
        if (rtnGo != null) return rtnGo;

        //�ƿ� ��ϵ� ���� �ʾ��� ��.
        List<GameObject> listGo = await RegisterObject(AssetName, 1);
        rtnGo = listGo[0];
        rtnGo.SetActive(true);
        rtnGo.transform.position = pos;

        return rtnGo;

    }

    /// <summary>
    /// Ǯ���� ������Ʈ�� ��ϸ���.
    /// </summary>
    /// <param name="AssetName">����� ������Ʈ�̸�</param>
    /// <param name="ammount">����</param>
    /// <param name="callback">�Ϸ��� �ݹ�</param>
    /// <returns></returns>
    public async UniTask<List<GameObject>> RegisterObject(string AssetName, int ammount, Action callback = null)
    {
        bool registered = false;
        List<GameObject> list = new List<GameObject>();
        Managers.Resource.LoadAsync<GameObject>(AssetName, (success) => {

            _objectPoolList.Add(AssetName, success);

            GameObject folder = new GameObject();
            folder.name = "@" + AssetName;
            folder.transform.parent = _root.transform;

            for (int i = 0; i < ammount; i++)
            {
                GameObject inst = Instantiate(success);
                inst.SetActive(false);
                inst.transform.parent = folder.transform;
                list.Add(inst);
            }
            _poolList.Add(AssetName, list);
            registered = true;
            callback?.Invoke();
        });
        //��ϵ� �� ���� ���.
        await UniTask.WaitUntil(() => { return registered == true; });

        return list;
    }
    /// <summary>
    /// �ܵ� ������Ʈ�� �ҷ���
    /// </summary>
    /// <param name="name">�����̸�</param>
    /// <returns></returns>
    public GameObject GetSingularObjet(string name)
    {
        if (!_singualrObject.ContainsKey(name))
        {
            return null;
        }
        return _singualrObject[name];
    }

    /// <summary>
    /// ��ϵ� ������ƮǮ�� ����Ʈ�� ������
    /// </summary>
    /// <param name="name">�����̸�</param>
    /// <returns></returns>
    public List<GameObject> GetPoolObject(string name)
    {
        if (!_poolList.ContainsKey(name))
        {
            return null;
        }
        return _poolList[name];
    }

    async UniTask<GameObject> TaskGet(string name)
    {
        if (!_objectPoolList.ContainsKey(name))
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
        bool loadAwait = false;
        Managers.Resource.LoadAsync<GameObject>(name, (sucess) =>
        {
            GameObject folder = GameObject.Find("@" + name);

            inst = Instantiate(sucess);
            inst.SetActive(false);
            inst.transform.parent = folder.transform;

            _poolList[name].Add(inst);
            loadAwait = true;
        });
        // �ε��� �� ���� ���.
        await UniTask.WaitUntil(() => { return loadAwait == true; });
        return inst;
    }
}