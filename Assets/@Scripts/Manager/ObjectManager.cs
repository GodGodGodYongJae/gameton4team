using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{

    private GameObject _root;
    private GameObject[] _poolList;

    private Dictionary<string, GameObject> _objectPoolList = new Dictionary<string, GameObject>();

/// <summary>
/// 오브젝트 생성
/// </summary>
/// <param name="AssetName">생성할 오브젝트 어드레서블 이름</param>
/// <param name="pos">생성할 좌표</param>
/// <param name="pool">풀링할건지</param>
/// <param name="ammount">풀링할 갯수</param>
    void Instantiate(string AssetName, Vector2[] pos, bool pool = false, int ammount = 1)
    {
        //root 오브젝트가 없으면 생성 해줌.
        if(_root == null)
        {
            _root = new GameObject { name = "@ObjectManager" };
        }

        //생성할 벡터의 사이즈가 맞지 않으면, Debug Error를 내준다.
        if (pos.Length != ammount)
            Debug.LogError("갯수가 맞지 않습니다. 포스 갯수 :" + pos.Length + " 생성할 수량 : " + ammount);
 

        //해당 오브젝트가 풀링 리스트에 있는지 체크.
        for (int i = 0; i < ammount; i++)
        {
            GameObject PoolObj = Get(AssetName);
            if (PoolObj != null)
            {
                PoolObj.SetActive(true);
                PoolObj.transform.position = pos[i];
                return;
            }
        }
     

        //풀링할 오브젝트가 아니라면, 어드레서블 이름으로 찾아서 만들어줌.
        if(pool == false)
        {
            Managers.Resource.Instantiate(AssetName,_root.transform);
            return;
        }


        GameObject folder = new GameObject();
        folder.name = AssetName;
        folder.transform.parent = _root.transform;



    }

     GameObject Get(string name)
    {
        if(!_objectPoolList.ContainsKey(name))
        {
            return null;
        }
        return null;
    }
}
