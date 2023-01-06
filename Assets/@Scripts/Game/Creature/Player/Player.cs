using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Creature
{
    public enum PlayerActionKey
    { 
        None,
        Direction,
        Jump,
        Damage,
        Death,
    }

    Vector2 _directionVector;
    SwipeController SwipeController;
    private Dictionary<PlayerActionKey, Action> PlayerAction = new Dictionary<PlayerActionKey, Action>();
    public void PlayerActionAdd(PlayerActionKey key, Action action)
    {
        PlayerAction.Add(key,action);
    }
    public float Speed {get{ return _creatureData.Speed; } }
    public float GetPlayerLeft()
    {
        return _directionVector.x;
    }
    public int GetPlayerDamage()
    {
        return _creatureData.AttackDamage;
    }
    protected override void Awake()
    {
        base.Awake();
        _type = Type.Player;
        _directionVector = Vector2.right;
        SwipeController = new SwipeController();
        PostEventHp();
        PlayerActionAdd(PlayerActionKey.Direction, ChangeDirection);
        PlayerActionAdd(PlayerActionKey.Jump, Jump);
        
        Managers.Events.AddListener(Define.GameEvent.playerEvents, PlayerActionCall);
        Managers.FixedUpdateAction += Move;
    }

    private void PlayerActionCall(Define.GameEvent eventType, Component Sender, object param)
    {
        if(eventType == Define.GameEvent.playerEvents)
        {
            PlayerActionKey key = (PlayerActionKey)param;
            PlayerAction[key]?.Invoke();
        }
    }

    void Move() => transform.Translate(_directionVector * _creatureData.Speed * Time.deltaTime);   

    void ChangeDirection()
    {
        _directionVector = _directionVector * -1;
        transform.localScale = new Vector2(_directionVector.x, 1);
    }
    float _jumpForce = 7f;
    void Jump()
    {
        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = new Vector2(rigid.velocity.x, _jumpForce);
    }

    bool isDamage = false;
    int invinvibilityDuration = 3000;
    public override void Damage(int dmg, Creature Target)
    {
        if (isDamage == true) return;
        invincibilityDealy().Forget();
        blinkObject().Forget();
        KnockBack(Target.gameObject);
        _hp -= dmg;
        PostEventHp();
      
        if (_hp <= 0)
        {
            Death();
        }
    }

    private async UniTaskVoid invincibilityDealy()
    {
        isDamage = true;;
        await UniTask.Delay(invinvibilityDuration);
        isDamage = false;
    }
    void PostEventHp()
    {
        Define.PlayerEvent_HPData data = new Define.PlayerEvent_HPData();
        data.maxHp = _creatureData.MaxHP;
        data.curHp = _hp;
        Managers.Events.PostNotification(Define.GameEvent.playerHealthChange, this, data);
    }
}
