using Assets._Scripts.Game.Weapon;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading;
using UnityEngine;


public class WeaponMelee : Weapon
{
    BoxCollider2D boxCollider;
    protected int effectoDir;
    public override void Start()
    {
        base.Start();
        boxCollider = GetComponent<BoxCollider2D>();
        effectoDir = -1;
    }


    public override async UniTaskVoid Attack()
    {

        await UniTask.Delay(weaponData.AttackDealay, cancellationToken: cts.Token);
        Rigidbody2D pRigid = player.gameObject.GetComponent<Rigidbody2D>();
        await UniTask.WaitUntil(() =>
        {
            return pRigid.velocity.y == 0;
        },cancellationToken:cts.Token);



        damagedMonsterList.Clear();
        float direction = Mathf.Clamp(player.transform.localScale.x, -1, 1);
        Vector2 playerPos = player.transform.position;
        
            if (this == null) return;
            Vector2 effectPos = playerPos + new Vector2(weaponData.EffectPos.x * direction, weaponData.EffectPos.y);
            GameObject effectGo = Managers.Object.InstantiateAsync(weaponData.Effect.name, effectPos);
            effectGo.transform.localScale = new Vector2(effectoDir * effectGo.transform.localScale.x * direction, effectGo.transform.localScale.y);
            PlayerBullet bullet = effectGo.GetOrAddComponent<PlayerBullet>();
            bullet.InitBulletData(weaponData, player);
            Attack().Forget();
        

    }

    public override void ChangeWeaponFixedUpdateDelete()
    {
        cts.Dispose();
        cts = new CancellationTokenSource();
    }

}

