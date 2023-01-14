using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager 
{


    LinkedList<Creature> SpawnedList = new LinkedList<Creature>();
    //어드레서블로 쭉 밀어줘야 하기 때문에 일단 필요.
    List<string> spawndMonsterAddressable = new List<string>();
    public int GetSpawnMonsterCount => SpawnedList.Count; 
    
    public void Init()
    {
        Managers.Events.AddListener(Define.GameEvent.stageClear, StageClear);
        Managers.Events.AddListener(Define.GameEvent.monsterDestroy, RemoveMonster);
    }


    public async UniTask<GameObject> AddMonster(string AssetName,Vector2 Spawnpos)
    {
        if (!spawndMonsterAddressable.Contains(AssetName)) spawndMonsterAddressable.Add(AssetName);
        List<GameObject> getHealthBarList = Managers.Object.GetPoolObject(StringData.HealthBar);
        if (getHealthBarList == null)
        {
            await Managers.Object.RegisterObject(StringData.HealthBar, Define.PoolHpBar);
        }

        GameObject go = await Managers.Object.InstantiateAsync(AssetName, Spawnpos);
        Managers.Events.PostNotification(Define.GameEvent.SpawnMonster, null, go.GetComponent<Creature>());
        SpawnedList.AddLast(go.GetComponent<Creature>());
        return go;
    }

    private void RemoveMonster(Define.GameEvent eventType, Component Sender, object param)
    {
        if (eventType.HasFlag(Define.GameEvent.monsterDestroy))
        {
            Creature creature = (Creature)Sender;
            if (SpawnedList.Contains(creature))
            {
                SpawnedList.Remove(creature);
            }
        }
    }

    private void StageClear(Define.GameEvent eventType, Component Sender, object param)
    {
        if(eventType == Define.GameEvent.stageClear && Utils.EqualSender<Ground>(Sender))
        {
            foreach (var item in spawndMonsterAddressable)
            {
                Managers.Object.RemoveObjectPool(item);
            }
            spawndMonsterAddressable.Clear();
            SpawnedList.Clear();
        }
       
    }
}
