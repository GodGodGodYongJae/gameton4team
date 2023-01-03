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
    /// 단독으로 운영할 오브젝트 생성.
    /// </summary>
    /// <param name="AssetName">어드레서블 이름</param>
    /// <param name="pos">생성할 좌표</param>
    /// <returns></returns>
    public async UniTask<GameObject> InstantiateSingle(string AssetName, Vector2 pos)
    {
        GameObject rtngo = null;
        bool isTask = false;
        //root 오브젝트가 없으면 생성 해줌.
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
    /// 오브젝트 풀링 생성 posList에 따라 생성 갯수 정해짐.
    /// </summary>
    /// <param name="AssetName">어드레서블 이름</param>
    /// <param name="posList">좌표값 리스트</param>
    /// <returns></returns>
    public async UniTask<List<GameObject>> InstantiateAsync(string AssetName, List<Vector2> posList)
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
            var PoolObj = await TaskGet(AssetName);
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
        rtnGo = await RegisterObject(AssetName, ammount);
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
    public async UniTask<GameObject> InstantiateAsync(string AssetName, Vector2 pos)
    {
        GameObject rtnGo = null;
        //root 오브젝트가 없으면 생성 해줌.
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

        //리스트가 1이라도 있다면, 가져왔기 때문에 리턴.
        if (rtnGo != null) return rtnGo;

        //아예 등록도 되지 않았을 때.
        List<GameObject> listGo = await RegisterObject(AssetName, 1);
        rtnGo = listGo[0];
        rtnGo.SetActive(true);
        rtnGo.transform.position = pos;

        return rtnGo;

    }

    /// <summary>
    /// 풀링할 오브젝트를 등록만함.
    /// </summary>
    /// <param name="AssetName">등록할 오브젝트이름</param>
    /// <param name="ammount">갯수</param>
    /// <param name="callback">완료후 콜백</param>
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
        //등록될 때 까지 대기.
        await UniTask.WaitUntil(() => { return registered == true; });

        return list;
    }
    /// <summary>
    /// 단독 오브젝트를 불러옴
    /// </summary>
    /// <param name="name">에셋이름</param>
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
    /// 등록된 오브젝트풀링 리스트를 가져옴
    /// </summary>
    /// <param name="name">에셋이름</param>
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
        // 모든 object가 false 상태임을 의미함. 
        // 그러면 새로 만들어줘야 함.
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
        // 로드할 때 까지 대기.
        await UniTask.WaitUntil(() => { return loadAwait == true; });
        return inst;
    }
}