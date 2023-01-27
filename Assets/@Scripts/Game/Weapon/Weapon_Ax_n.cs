using Assets._Scripts.Game.Weapon;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
public class Weapon_Ax_n : Weapon
{
    BoxCollider2D boxCollider;
    public override void Start()
    {
        base.Start();
        boxCollider = GetComponent<BoxCollider2D>();
    }


    public override async UniTaskVoid Attack()
    {

        damagedMonsterList.Clear();
        float direction = Mathf.Clamp(player.transform.localScale.x, -1, 1);
        Vector2 effectPos = (Vector2)boxCollider.bounds.center + (weaponData.EffectPos * direction);
        GameObject effectGo = await Managers.Object.InstantiateAsync(weaponData.Effect.name, effectPos);
        effectGo.transform.localScale = new Vector2(effectGo.transform.localScale.x * direction, effectGo.transform.localScale.y);
        PlayerBullet bullet = effectGo.GetOrAddComponent<PlayerBullet>();
        bullet.InitBulletData(weaponData, player);

        await UniTask.Delay(weaponData.AttackDealay, cancellationToken: cts.Token);
        Attack().Forget();
    }

    public override void ChangeWeaponFixedUpdateDelete()
    {
        cts.Dispose();
        cts = new CancellationTokenSource();
    }
}