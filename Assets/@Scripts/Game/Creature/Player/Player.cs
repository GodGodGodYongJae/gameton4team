using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed partial class Player : Creature
{
    public enum PlayerActionKey
    { 
        None,
        Direction,
        Jump,
        Damage,
        Death,
        End
    }

    private Dictionary<PlayerActionKey, Action> PlayerAction = new Dictionary<PlayerActionKey, Action>();
    public void PlayerActionAdd(PlayerActionKey key, Action action)
    {
        PlayerAction[key] += action;
    }

    protected override void Awake()
    {
        base.Awake();
        PostEventHp();
        for (int i = 0; i < (int)PlayerActionKey.End; i++)
        {
            PlayerAction.Add((PlayerActionKey)i, null);
        }
        Managers.Events.AddListener(Define.GameEvent.playerEvents, PlayerActionCall);
        Init();
    }

    private void PlayerActionCall(Define.GameEvent eventType, Component Sender, object param)
    {
        if(eventType == Define.GameEvent.playerEvents)
        {
            PlayerActionKey key = (PlayerActionKey)param;
            PlayerAction[key]?.Invoke();
        }
    }
    private void PostEventHp()
    {
        Define.PlayerEvent_HPData data = new Define.PlayerEvent_HPData();
        data.maxHp = _creatureData.MaxHP;
        data.curHp = _hp;
        Managers.Events.PostNotification(Define.GameEvent.playerHealthChange, this, data);
    }
    #region Getter
    public float GetPlayerLeft()
    {
        return directionVector.x;
    }
    public float Speed { get { return _creatureData.Speed; } }
    public int GetPlayerDamage()
    {
        return _creatureData.AttackDamage;
    }
    #endregion
}
