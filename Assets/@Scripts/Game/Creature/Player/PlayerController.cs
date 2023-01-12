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

    void Jump() 
    { 
        if(_rigid.velocity.y == 0)
        {
             _rigid.velocity = new Vector2(_rigid.velocity.x, playerData.JumpForce);
        }
    }


    void ChangeDirection()
    {
        directionVector = directionVector * -1;
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
    }
    // 위치 초기화
    public void InitPosition()
    {
        transform.position = new Vector2(0, transform.position.y);
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

    //무적시간 
    private async UniTaskVoid invincibilityDealy()
    {
        isDamage = true; 
        await UniTask.Delay(playerData.InvinvibilityDuration);
        isDamage = false;
    }


    /*TODO ---
    해당 부분 차후 리팩토링 필요함, 
    */
    [SerializeField]
    public SPUM_SpriteList _root;
    private int _blinkCount = 3;

    bool spriteEnable = true;
    protected override async UniTaskVoid blinkObject()
    {

        SPUM_SpriteList spum_List = _root;
        List<List<SpriteRenderer>> AllSpriteList = spum_List.AllSpriteList;
        
        for (int i = 0; i < _blinkCount * 2; i++)
        {
            spriteEnable = !spriteEnable;
            foreach (var item in AllSpriteList)
            {
                List<SpriteRenderer> spriteList = item;
                foreach (var item2 in spriteList)
                {
                    item2.enabled = spriteEnable;
                }
            }
            await UniTask.Delay(150);
        }

    }
    #endregion
}
