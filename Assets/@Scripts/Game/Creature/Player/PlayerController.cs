using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public partial class Player
{
    SwipeController SwipeController;
    public Vector2 directionVector;
    PlayerData playerData;

    public PlayerData PlayerData => playerData;

    public int CurrentExp;
    void Init()
    {
        _type = Type.Player;
        // DataLoad
        playerData = (PlayerData)_creatureData;
        playerData.AssetLoad();
        LevelUP();       
        //End
        SwipeController = new SwipeController();
        directionVector = Vector2.right;
        PlayerActionAdd(PlayerActionKey.Direction, ChangeDirection);
        PlayerActionAdd(PlayerActionKey.Jump, Jump);
        Managers.FixedUpdateAction += Move;
        Managers.Events.AddListener(Define.GameEvent.monsterDestroy, MonsterKill);

    }


    private void MonsterKill(Define.GameEvent eventType, Component Sender, object param)
    {
        if(eventType == Define.GameEvent.monsterDestroy)
        {
            Monster monster = (Monster)Sender;
            CurrentExp += monster.MonsterData.Exp;
            if(CurrentExp >= playerData.ExperiencePoint && playerData.ExperiencePoint != int.MaxValue)
            {

                LevelUpText().Forget();
                LevelUP();
            }
            PostChangeExp();
        }
    }
    private async UniTaskVoid LevelUpText()
    {
        await Managers.Object.InstantiateAsync("LevelUpText", transform.position);
    }


    #region 레벨업/경험치 관련
    void PostChangeExp()
    {
        Managers.Events.PostNotification(Define.GameEvent.playerExpChange, this);
    }
    void LevelUP()
    {
        playerData.LevelUp();
        _hp = playerData.MaxHP;
        CurrentExp = 0;
        PostEventHp();
        PostChangeExp();
    }
    #endregion

    #region 조작 및 이동관련
    void Move() => transform.Translate(directionVector * _creatureData.Speed * Time.deltaTime);

    void Jump() 
    { 
        if(_rigid.velocity.y == 0)
        {
            //Vector2 pos = transform.position;
            //pos.y += 0.6f;
            _rigid.AddForce(Vector2.up * playerData.JumpForce);
             //_rigid.velocity = new Vector2(_rigid.velocity.x, playerData.JumpForce );
        }
    }


    void ChangeDirection()
    {
        directionVector = directionVector * -1;
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
    }
    // 위치 초기화
    public void InitPosition(float x = 0,float y = 0)
    {
        transform.position = new Vector2(x, y);
    }
    #endregion

    #region 데미지 관련

    bool isDamage = false;
    public override void Damage(float dmg, Creature Target)
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
            Time.timeScale = 0;
            Managers.Events.PostNotification(Define.GameEvent.playerEvents, this, PlayerActionKey.Death);
            //animator.SetBool("Death", true);
        }
    }

    public void Respawn()
    {
        Time.timeScale = 1;
        AddHp(playerData.MaxHP);
    }
    public void AddHp(float add)
    {
        _hp = (_hp + add >= playerData.MaxHP) ? playerData.MaxHP : _hp + add;
        PostEventHp();
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
