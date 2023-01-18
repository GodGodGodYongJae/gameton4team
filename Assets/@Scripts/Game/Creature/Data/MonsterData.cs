using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObj/Data/Monster", fileName = "Monster_")]
public class MonsterData : CreatureData
{
    //TIP: 범위는 리치
    [SerializeField]
    [Tooltip("시야거리")]
    private float _visibility;
    [SerializeField]
    [Tooltip("사정거리")]
    private float _attackRange;
    [SerializeField]
    [Tooltip("공격 재사용 시간 ms 단위")]
    private int _attackDealy;
    [SerializeField]
    [Tooltip("이동딜레이 Scond 단위")]
    private int _moveDealy;
    [SerializeField]
    [Tooltip("이동시간")]
    private float _moveTime;
    public float Visibility => _visibility;
    public float AttackRange => _attackRange;
    public int AttackDealy => _attackDealy;
    public int MoveDealy => _moveDealy;

    public float MoveTime => _moveTime;
}
