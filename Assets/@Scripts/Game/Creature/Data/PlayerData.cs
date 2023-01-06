using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObj/Data/Player", fileName = "Player_")]
public class PlayerData : CreatureData
{
    [SerializeField]
    [Tooltip("점프력")]
    private float _jumpForce;
    [SerializeField]
    [Tooltip("피격후 무적시간")]
    private int _invinvibilityDuration;
    public float JumpForce => _jumpForce;
    public int InvinvibilityDuration => _invinvibilityDuration;
}
