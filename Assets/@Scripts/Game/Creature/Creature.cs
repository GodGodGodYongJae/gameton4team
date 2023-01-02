using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Creature : MonoBehaviour, IDamageble
{
    public enum Type { 
        Player,
        Monster
    }
    [SerializeField]
    protected CreatureData _creatureData;
    protected Type _type; 
    protected int _hp;

    private void Awake()
    {
        _hp = _creatureData.MaxHP;
    }
    public virtual Type GetType()
    { return _type; }

    public virtual void Damage(int dmg, Creature Target)
    {
        _hp -= dmg;
        blinkObject().Forget();
        KnockBack(Target.gameObject);
        if(_hp <= 0)
        {
            Death();
        }
    }
    
    public virtual void Death()
    {
        
        transform.gameObject.SetActive(false);
        _hp = _creatureData.MaxHP;
    }

    private int blinkCount = 3;
    protected async UniTaskVoid blinkObject()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        for (int i = 0; i < blinkCount; i++)
        {
            sprite.enabled = false;
            await UniTask.Delay(150);
            sprite.enabled = true;
            await UniTask.Delay(150);
        }
        sprite.enabled = true;
    }

    private int knockBackStrength = 7, knockBackdelay = 150; 
    protected void KnockBack(GameObject sender)
    {
        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        if (rigid == null) Debug.LogError("해당 Object에, rigid body가 없습니다." + gameObject.name);
        ResetKnocback().Forget();
        Vector2 knockBackPos = (transform.position - sender.transform.position).normalized;
        knockBackPos.y += 1;
        rigid.AddForce(knockBackPos * knockBackStrength, ForceMode2D.Impulse);
        ResetKnocback().Forget();

    }
    private async UniTaskVoid ResetKnocback()
    {
        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        await UniTask.Delay(knockBackdelay);
        rigid.velocity = Vector2.zero;
    }
}
