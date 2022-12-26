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
/// 오브젝트 생성
/// </summary>
/// <param name="AssetName">생성할 오브젝트 어드레서블 이름</param>
/// <param name="pos">생성할 좌표</param>
/// <param name="pool">풀링할건지</param>
/// <param name="ammount">풀링할 갯수</param>
    public void Instantiate(string AssetName, List<Vector2> posList, bool pool = false, int ammount = 1)
    {
        //root 오브젝트가 없으면 생성 해줌.
        if(_root == null)
        {
            _root = new GameObject { name = "@ObjectManager" };
        }

        //생성할 벡터의 사이즈가 맞지 않으면, Debug Error를 내준다.
        if (posList.Count != ammount)
            Debug.LogError("갯수가 맞지 않습니다. 포스 갯수 :" + posList.Count + " 생성할 수량 : " + ammount);
 

        //해당 오브젝트가 풀링 리스트에 있는지 체크.
        for (int i = 0; i < ammount; i++)
        {
            GameObject PoolObj = Get(AssetName);
            if (PoolObj != null)
            {
                PoolObj.SetActive(true);
                PoolObj.transform.position = posList[i];
            }
        }
     

        //풀링할 오브젝트가 아니라면, 어드레서블 이름으로 찾아서 만들어줌.
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
    /// 오브젝트 생성2
    /// </summary>
    /// <param name="AssetName">생성할 오브젝트 어드레서블 이름</param>
    /// <param name="pos">생성할 좌표</param>
    /// <param name="pool">풀링할건지</param>
    /// <param name="ammount">풀링할 갯수</param>
    public void Instantiate(string AssetName, Vector2 pos, bool pool = false, int ammount = 1)
    {
        //root 오브젝트가 없으면 생성 해줌.
        if (_root == null)
        {
            _root = new GameObject { name = "@ObjectManager" };
        }

        //생성할 벡터의 사이즈가 맞지 않으면, Debug Error를 내준다.



        //해당 오브젝트가 풀링 리스트에 있는지 체크.
        for (int i = 0; i < ammount; i++)
        {
            GameObject PoolObj = Get(AssetName);
            if (PoolObj != null)
            {
                PoolObj.SetActive(true);
                PoolObj.transform.position = pos;
            }
        }


        //풀링할 오브젝트가 아니라면, 어드레서블 이름으로 찾아서 만들어줌.
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

        // 모든 object가 false 상태임을 의미함. 
        // 그러면 새로 만들어줘야 함.
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
