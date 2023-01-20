
using UnityEngine;
using System;
using System.Collections.Generic;
[System.Serializable]
public class GroundSpawnData
{
    [SerializeField]
    GameObject groundsPrefab;

    public GameObject GroundPrefab => groundsPrefab;

    [SerializeField]
    List<CreatureSpawn> spawnMonsterList;


}
