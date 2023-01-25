using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Weapon_Spear_n : Weapon
{
    GameObject effect = null;
    WeaponData weapondata;
    //SpriteRenderer sprite;
    public override void Start()
    {
        //sprite = GetComponent<SpriteRenderer>();
        base.Start();

    }
    private void FixedUpdate()
    {
        FEffectFollow();
    }

    protected override void FEffectFollow()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box == null) return;
        try
        {
            effect.transform.position = (Vector2)box.bounds.center + weaponData.EffectPos;
        }
        catch
        {
            return;
        }
    }

    public override async UniTaskVoid Attack()
    {
        damagedMonsterList.Clear();
        isAttack = true;
        effect = await weaponData.Effect();
        await UniTask.Delay(weaponData.AttackDuration, cancellationToken: cts.Token);
        effect.SetActive(false);
        effect = null;

        isAttack = false;

        await UniTask.Delay(weaponData.AttackDealay, cancellationToken: cts.Token);
        Attack().Forget();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        //������ ���� ��Ÿ�� ���̶�� ����.
        if (isAttack == false) return;

        //�̹� �ش� ���Ͱ� �ѹ� ������ �޾����� ����
        if (damagedMonsterList.Contains(collision.gameObject))
            return;

        Creature creature = collision.GetComponent<Creature>();
        if (creature == null) return;

        if (creature.GetType == Creature.Type.Monster)
        {
            damagedMonsterList.Add(collision.gameObject);
            creature.Damage(weaponData.AttackDamge + player.GetPlayerDamage(), player);
        }
    }

    public override void ChangeWeaponFixedUpdateDelete()
    {
        cts.Dispose();
        cts = new CancellationTokenSource();
    }
}
