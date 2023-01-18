using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
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


    void FEffectFollow()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        effect.transform.position = (Vector2)box.bounds.center + weaponData.EffectPos;
    }

    public override async UniTaskVoid Attack()
    {
        damagedMonsterList.Clear();
        isAttack = true;
        effect = await weaponData.Effect();
        Managers.FixedUpdateAction += FEffectFollow;
        await UniTask.Delay(weaponData.AttackDuration);
        Managers.FixedUpdateAction -= FEffectFollow;
        effect.SetActive(false);
        effect = null;

        isAttack = false;
        
        await UniTask.Delay(weaponData.AttackDealay);
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

        if(creature.GetType == Creature.Type.Monster)
        {
            damagedMonsterList.Add(collision.gameObject);
            creature.Damage(weaponData.AttackDamge + player.GetPlayerDamage(), player);
        }
            

    }
}
