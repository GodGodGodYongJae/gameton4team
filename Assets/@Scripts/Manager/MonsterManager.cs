using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager 
{


    LinkedList<Creature> SpawnedList = new LinkedList<Creature>();

    public int GetSpawnMonsterCount => SpawnedList.Count; 
    
    public void Init()
    {
        Managers.Events.AddListener(Define.GameEvent.monsterDestroy, RemoveMonster);
    }

    public async UniTask<GameObject> AddMonster(string AssetName,Vector2 Spawnpos)
    {
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

}
