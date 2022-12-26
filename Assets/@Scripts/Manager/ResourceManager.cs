using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;
public class ResourceManager 
{

    //���� �ε��� ���ҽ�
    Dictionary<string, UnityEngine.Object> _resources = new Dictionary<string, Object>();

    //�񵿱� ���ҽ� ���� ��Ȳ.
    Dictionary<string, AsyncOperationHandle> _handles = new Dictionary<string, AsyncOperationHandle>();
    public int HandlesCount = 0;

    public void LoadAsync<T>(string key, Action<T> callback = null) where T: UnityEngine.Object
    {
        //ĳ�� Ȯ��.
        if(_resources.TryGetValue(key, out Object resource))
        {
            callback?.Invoke(resource as T);
            return;
        }
        //�ε��� ���������� �Ϸ���� �ʾҴٸ�, �ݹ鸸 �߰�.
        if(_handles.ContainsKey(key))
        {
            _handles[key].Completed += (op) => { callback?.Invoke(op.Result as T); };
        }
        //���ҽ� �񵿱� �ε� ����.
        _handles.Add(key, Addressables.LoadAssetAsync<T>(key));
        HandlesCount++;
        _handles[key].Completed += (op) =>
        {
            _resources.Add(key, op.Result as UnityEngine.Object);
            callback?.Invoke(op.Result as T);
            HandlesCount--;
        };
    }

    public void Release(string key)
    {
        if (_resources.TryGetValue(key, out Object resource) == false)
            return;

        _resources.Remove(key);

        if (_handles.TryGetValue(key, out AsyncOperationHandle handle))
            Addressables.Release(handle);

        _handles.Remove(key);
    }

    public void Clear()
    {
        _resources.Clear();

        foreach (var handle in _handles.Values)
            Addressables.Release(handle);

        _handles.Clear();
    }
    #region ������
    public void Instantiate(string key, Transform parent = null, Action<GameObject> callback = null)
    {
        LoadAsync<GameObject>(key, (prefab) =>
        {
            GameObject go = GameObject.Instantiate(prefab, parent);
            go.name = prefab.name;
            go.transform.localPosition = prefab.transform.position;
            callback?.Invoke(go);
        });

        //Addressables.InstantiateAsync(key, parent).Completed += (go) => 
        //{ 
        //	onInstantiate?.Invoke(go.Result); 
        //};
    }

    public void Destroy(GameObject go, float seconds = 0.0f)
    {
        Object.Destroy(go, seconds);

        //if (seconds == 0.0f)
        //	Addressables.ReleaseInstance(go);
        //else
        //	Managers.Instance.StartCoroutine(CoDestroyAfter(go, seconds));
    }

    public async UniTaskVoid UniTaskDestroyAfterAsync(GameObject go, float seconds)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(seconds));
        Addressables.ReleaseInstance(go);
    }
    #endregion
}