using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestMonster : Monster
{
    protected override void Awake()
    {
        moveAction += MoveDelay;
        base.Awake();
    }
    public override void Damage(int dmg, Creature Target)
    {
        base.Damage(dmg, Target);
        creatureHPBar.Damage(_hp, _creatureData.MaxHP);
    }
    public override void Death()
    {
        Managers.Events.PostNotification(Define.GameEvent.monsterDestroy, this);
        base.Death();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Creature creature = collision.gameObject.GetComponent<Creature>();
        if (creature == null) return;

        if (creature.GetType == Creature.Type.Player)
        {
            creature.Damage(_creatureData.AttackDamage, this);
        }

    }
  
    async UniTaskVoid MoveDelay()
    {
        float moveTime = 0;
        while (moveTime < 0.8f)
        {
            if (transform.gameObject.activeSelf == false) return;
            float remainigDistance = (transform.position - target.gameObject.transform.position).sqrMagnitude;
            Vector3 newPos = Vector3.MoveTowards(_rigid.position, target.gameObject.transform.position, _creatureData.Speed * Time.deltaTime);
            _rigid.MovePosition(newPos);
            remainigDistance = (transform.position - target.gameObject.transform.position).sqrMagnitude;

            await UniTask.WaitForFixedUpdate();
            moveTime += Time.deltaTime;

        }
        await UniTask.Delay(1500);
        MoveDelay().Forget();
    }
    async UniTaskVoid Move()
    {
        if (transform.gameObject.activeSelf == false) return;
        float remainigDistance = (transform.position - target.gameObject.transform.position).sqrMagnitude;
        while(remainigDistance > float.Epsilon)
        {
            Vector3 newPos = Vector3.MoveTowards(_rigid.position,target.gameObject.transform.position, _creatureData.Speed * Time.deltaTime);
            _rigid.MovePosition(newPos);
            remainigDistance = (transform.position - target.gameObject.transform.position).sqrMagnitude;

            await UniTask.WaitForFixedUpdate();
        }
        Move().Forget();
    }



}
