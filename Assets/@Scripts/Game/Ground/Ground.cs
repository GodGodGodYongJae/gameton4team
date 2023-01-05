using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    [SerializeField]
    CreatureGenerator CreatureGenerator;

    LinkedList<Creature> SpawnedList = new LinkedList<Creature>();

    bool iscurrentGround = false;
    bool isspawned = false;

    public int MonsterCount => (isSpawned || CreatureGenerator == null) ?SpawnedList.Count:-1;
    public bool isSpawned => isspawned;
    public bool isHaveGenerator => CreatureGenerator != null;

    private void Start()
    {
        if(CreatureGenerator != null)
            Managers.Events.AddListener(Define.GameEvent.monsterDestroy, DeathMonster);
    }

    private void DeathMonster(Define.GameEvent eventType, Component Sender, object param)
    {
        if(eventType.HasFlag(Define.GameEvent.monsterDestroy) && iscurrentGround)
        {
            Creature creature = (Creature)Sender;
            if (SpawnedList.Contains(creature))
            {
                SpawnedList.Remove(creature);
            }
        }
    }

    public async UniTaskVoid SpawnMonster()
    {
      
        SpawnedList.Clear();
        if (CreatureGenerator == null)
            return;
        List<GameObject> getHealthBarList = Managers.Object.GetPoolObject(StringData.HealthBar);
        if ( getHealthBarList == null)
        {
           await Managers.Object.RegisterObject(StringData.HealthBar, CreatureGenerator.SpawnMonsterList.Count);
        }

        foreach (var item in CreatureGenerator.SpawnMonsterList)
        {
            Vector2 spawnPos = transform.position;
            spawnPos += item.spawnPos;
            GameObject go = await Managers.Object.InstantiateAsync(item.spawnObj.name, spawnPos);
            Managers.Events.PostNotification(Define.GameEvent.SpawnMonster, this, go.GetComponent<Creature>());
            SpawnedList.AddLast(go.GetComponent<Creature>());

        }
        isspawned = iscurrentGround = true;
    }
}


