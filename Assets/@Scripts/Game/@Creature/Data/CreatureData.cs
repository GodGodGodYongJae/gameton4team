using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObj/Data/Creature",fileName = "Creature_")]
public class CreatureData : ScriptableObject
{

    [SerializeField]

    protected int _maxhp;
    [SerializeField]

    protected float _speed;

    [SerializeField] 
    protected float attackDamage;
    public int MaxHP => _maxhp;
    public float Speed => _speed;
    public float AttackDamage => attackDamage;

}
