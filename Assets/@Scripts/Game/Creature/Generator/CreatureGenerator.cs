using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObj/Generator/CreatureGenerator", fileName = "CreatureGenerator_")]
public class CreatureGenerator : ScriptableObject
{
    [SerializeField]
    List<CreatureSpawn> spawnMonsterList;
    public IReadOnlyList<CreatureSpawn> SpawnMonsterList => spawnMonsterList;
}
