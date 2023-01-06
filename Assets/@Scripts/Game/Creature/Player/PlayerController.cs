using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    SwipeController SwipeController;
    Vector2 directionVector;
    PlayerData playerData;
    void Init()
    {
        _type = Type.Player;
        playerData = (PlayerData)_creatureData;
        SwipeController = new SwipeController();
        directionVector = Vector2.right;
        PlayerActionAdd(PlayerActionKey.Direction, ChangeDirection);
        PlayerActionAdd(PlayerActionKey.Jump, Jump);
        Managers.FixedUpdateAction += Move;
    }

 

    #region 조작 및 이동관련
    void Move() => transform.Translate(directionVector * _creatureData.Speed * Time.deltaTime);

    void Jump() => _rigid.velocity = new Vector2(_rigid.velocity.x, playerData.JumpForce);

    void ChangeDirection()
    {
        directionVector = directionVector * -1;
        transform.localScale = new Vector2(directionVector.x, 1);
    }
    #endregion

    #region 데미지 관련

    bool isDamage = false;
    public override void Damage(int dmg, Creature Target)
    {
        if (isDamage == true) return;
        Managers.Events.PostNotification(Define.GameEvent.playerEvents, this, PlayerActionKey.Damage);
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
        isDamage = true; ;
        await UniTask.Delay(playerData.InvinvibilityDuration);
        isDamage = false;
    }

    #endregion
}
