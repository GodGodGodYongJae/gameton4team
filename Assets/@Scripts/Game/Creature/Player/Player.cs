using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Creature
{

    Vector2 _directionVector;
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
        
        PostEventHp();

        Managers.UpdateAction += MUpdate;
        Managers.FixedUpdateAction += FUpdate;
    }
    // 차후 Move 자체를 Component로 빼야하거나 분리해야함.

    void FUpdate() => transform.Translate(_directionVector * _creatureData.Speed * Time.deltaTime);   
    
    void MUpdate()
    {
        
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _directionVector = _directionVector * -1;
                transform.localScale = new Vector2(_directionVector.x, 1);
            }
       
        }
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
