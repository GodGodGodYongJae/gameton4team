using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    GameObject effect;
    //SpriteRenderer sprite;
    public override void Start()
    {
        //sprite = GetComponent<SpriteRenderer>();
        base.Start();
     

    }
    void FEffectFollow()
    {

        //if (effect == null && effect.activeSelf == true)
        //{
            effect.transform.position = (Vector2)transform.position + weaponData.EffectPos;
        //}
    }

    public override async UniTaskVoid Attack()
    {
        isAttack = true;
        effect = await weaponData.Effect();
        Managers.FixedUpdateAction += FEffectFollow;
        await UniTask.Delay(weaponData.AttackDuration);
        Managers.FixedUpdateAction -= FEffectFollow;
        effect.SetActive(false);
        isAttack = false;
        await UniTask.Delay(weaponData.AttackDealay);
        Attack().Forget();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isAttack == false) return;

        Creature creature = collision.GetComponent<Creature>();
       
        if (creature == null) return;

        if(creature.GetType() == Creature.Type.Monster)
        {
            creature.Damage(weaponData.AttackDamge, player);
        }
            

    }
}
