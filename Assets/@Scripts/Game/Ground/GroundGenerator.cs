using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObj/GroundGenerator", fileName ="Stage_")]
public class GroundGenerator : ScriptableObject
{
    [SerializeField]
    GameObject[] _groundsPrefab;

    public IReadOnlyList<GameObject> Grounds => _groundsPrefab;
}
