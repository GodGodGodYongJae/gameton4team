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
/// ������Ʈ ����
/// </summary>
/// <param name="AssetName">������ ������Ʈ ��巹���� �̸�</param>
/// <param name="pos">������ ��ǥ</param>
/// <param name="pool">Ǯ���Ұ���</param>
/// <param name="ammount">Ǯ���� ����</param>
    void Instantiate(string AssetName, Vector2[] pos, bool pool = false, int ammount = 1)
    {
        //root ������Ʈ�� ������ ���� ����.
        if(_root == null)
        {
            _root = new GameObject { name = "@ObjectManager" };
        }

        //������ ������ ����� ���� ������, Debug Error�� ���ش�.
        if (pos.Length != ammount)
            Debug.LogError("������ ���� �ʽ��ϴ�. ���� ���� :" + pos.Length + " ������ ���� : " + ammount);
 

        //�ش� ������Ʈ�� Ǯ�� ����Ʈ�� �ִ��� üũ.
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
     

        //Ǯ���� ������Ʈ�� �ƴ϶��, ��巹���� �̸����� ã�Ƽ� �������.
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
