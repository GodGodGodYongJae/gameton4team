using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObj/Generator/GroundGenerator", fileName ="Stage_")]
public class GroundGenerator : ScriptableObject
{
    [SerializeField]
    GameObject[] _groundsPrefab;

    public IReadOnlyList<GameObject> Grounds => _groundsPrefab;
    [SerializeField]
    private float _chatperSize;

    public float ChatperSize => _chatperSize;
}
