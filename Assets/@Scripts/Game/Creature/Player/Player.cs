using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Creature
{


    Vector2 _directionVector;
    private int _hp;
    public float Speed {get{ return _creatureData.Speed; } }
    public float GetPlayerLeft()
    {
        return _directionVector.x;
    }
    private void Start()
    {
        _hp = _creatureData.MaxHP;
        _type = Type.Player;
        _directionVector = Vector2.right;

        Managers.UpdateAction += MUpdate;
        Managers.FixedUpdateAction += FUpdate;
    }
    // ���� Move ��ü�� Component�� �����ϰų� �и��ؾ���.

    void FUpdate()
    {
        transform.Translate(_directionVector * _creatureData.Speed * Time.deltaTime);
        // _rigidBody.AddForce(_directionVector * _speed);
        //if (_rigidBody.velocity.x >= _acceleration) _rigidBody.velocity = new Vector2(_acceleration, _rigidBody.velocity.y);
        //else if (_rigidBody.velocity.x <= -_acceleration) _rigidBody.velocity = new Vector2(-_acceleration, _rigidBody.velocity.y);
    }
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

        _hp -= dmg;
        if(_hp <= 0)
        {
            Death();
        }
    }

    private async UniTaskVoid invincibilityDealy()
    {
        isDamage = true;
        blinkPlayer().Forget();
        await UniTask.Delay(invinvibilityDuration);
        isDamage = false;
    }
    private int blinkCount = 3;
    private async UniTaskVoid blinkPlayer()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        for (int i = 0; i < blinkCount; i++)
        {
            sprite.enabled = false;
            await UniTask.Delay(300);
            sprite.enabled = true;
        }
    }
}