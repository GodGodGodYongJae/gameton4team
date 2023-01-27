using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public enum Type
    {
        Player,
        Monster
    }
    protected Type _type;
    public new Type GetType => _type;
    [SerializeField]
    protected CreatureData _creatureData;
    protected SpriteRenderer _sprite;
    protected float _hp;
    protected Rigidbody2D _rigid;

    public CreatureData CreatureData => _creatureData;
    protected virtual void Awake()
    {
        _hp = _creatureData.MaxHP;
        _rigid = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();
    }



    #region 
    public virtual void Damage(float dmg, Creature Target)
    {
        _hp -= dmg;
        blinkObject().Forget();
        KnockBack(Target.gameObject);
        if (_hp <= 0)
        {
            Death();
        }
    }

    protected virtual void Death() => transform.gameObject.SetActive(false);

    protected CancellationTokenSource cts = new CancellationTokenSource();

    protected void OnEnable()
    {
        if (cts != null)
        {
            cts.Dispose();
        }
        cts = new CancellationTokenSource();
    }

    private int blinkCount = 3;
    protected virtual async UniTaskVoid blinkObject()
    {
        for (int i = 0; i < blinkCount; i++)
        {
            _sprite.enabled = false;
            await UniTask.Delay(150, cancellationToken: cts.Token);
            _sprite.enabled = true;
            await UniTask.Delay(150, cancellationToken: cts.Token);
        }
        _sprite.enabled = true;
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
        await UniTask.Delay(knockBackdelay, cancellationToken: cts.Token);
        rigid.velocity = Vector2.zero;
    }
    #endregion
}