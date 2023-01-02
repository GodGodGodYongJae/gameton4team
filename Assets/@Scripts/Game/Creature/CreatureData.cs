using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObj/Data/Creature",fileName = "Creature_")]
public class CreatureData : ScriptableObject
{

    [SerializeField]
    private int _maxhp;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private int _attackDmg;

    public int MaxHP => _maxhp;
    public float Speed => _speed;
    public int AttackDamage => _attackDmg;

}
