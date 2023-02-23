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
    /// 단독으로 운영할 오브젝트 생성.
    /// </summary>
    /// <param name="AssetName">어드레서블 이름</param>
    /// <param name="pos">생성할 좌표</param>
    /// <returns></returns>
    public GameObject InstantiateSingle(string AssetName, Vector2 pos, GameObject parent = null)
    {
        GameObject rtngo = null;
        //root 오브젝트가 없으면 생성 해줌.
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
    /// 오브젝트 풀링 생성 posList에 따라 생성 갯수 정해짐.
    /// </summary>
    /// <param name="AssetName">어드레서블 이름</param>
    /// <param name="posList">좌표값 리스트</param>
    /// <returns></returns>
    public List<GameObject> InstantiateAsync(string AssetName, List<Vector2> posList)
    {
        List<GameObject> rtnGo = new List<GameObject>();
        int ammount = posList.Count;
        //root 오브젝트가 없으면 생성 해줌.
        if (_root == null)
        {
            _root = new GameObject { name = "@ObjectManager" };
        }
        //해당 오브젝트가 풀링 리스트에 있는지 체크.
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
        //리스트가 1이라도 있다면, 가져왔기 때문에 리턴.
        if (rtnGo.Count != 0) return rtnGo;

        //아예 등록도 되지 않았을 때.
        rtnGo = RegisterObject(AssetName, ammount);
        for (int i = 0; i < rtnGo.Count; i++)
        {
            rtnGo[i].SetActive(true);
            rtnGo[i].transform.position = posList[i];
        }
        return rtnGo;

    }
    /// <summary>
    /// 오브젝트 풀링 생성 하나만 등록할때
    /// </summary>
    /// <param name="AssetName">어드레서블 이름</param>
    /// <param name="pos">좌표값</param>
    /// <returns></returns>
    public  GameObject InstantiateAsync(string AssetName, Vector2 pos)
    {
        GameObject rtnGo = null;
        //root 오브젝트가 없으면 생성 해줌.
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

        //리스트가 1이라도 있다면, 가져왔기 때문에 리턴.
        if (rtnGo != null) return rtnGo;

        //아예 등록도 되지 않았을 때.
        List<GameObject> listGo = RegisterObject(AssetName, 1);
        rtnGo = listGo[0];
        rtnGo.SetActive(true);
        rtnGo.transform.position = pos;

        return rtnGo;

    }

    /// <summary>
    /// 풀링할 오브젝트를 등록만함
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
    /// 단독 오브젝트를 불러옴
    /// </summary>
    /// <param name="name">에셋이름</param>
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
    /// 등록된 오브젝트풀링 리스트를 가져옴
    /// </summary>
    /// <param name="name">에셋이름</param>
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
        // 모든 object가 false 상태임을 의미함. 
        // 그러면 새로 만들어줘야 함.
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
    /// 해당 오브젝트 풀링을 제거 하고 리소스도 해제 함. 
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
    /// 부모가 바뀐 Pool Object를 반환하는 코드 
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