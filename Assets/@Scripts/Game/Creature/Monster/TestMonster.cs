using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TestMonster : Monster
{
    protected CancellationTokenSource movects = new CancellationTokenSource();
    protected override void Awake()
    {
        moveAction += MoveDelay;
        base.Awake();
    }
    public override void Damage(int dmg, Creature Target)
    {
        base.Damage(dmg, Target);
        movects.Dispose();
        movects = new CancellationTokenSource();
        MoveDelay().Forget();
        creatureHPBar.Damage(_hp, _creatureData.MaxHP);
    }
    protected override void Death()
    {
        base.Death();
        Managers.Object.ReturnToParent(HPCanvas);
        Managers.Events.PostNotification(Define.GameEvent.monsterDestroy, this);
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
            try
            {
                float remainigDistance = (transform.position - target.gameObject.transform.position).sqrMagnitude;
                Vector3 newPos = Vector3.MoveTowards(_rigid.position, target.gameObject.transform.position, _creatureData.Speed * Time.deltaTime);
                _rigid.MovePosition(newPos);
                remainigDistance = (transform.position - target.gameObject.transform.position).sqrMagnitude;

                await UniTask.WaitForFixedUpdate(cancellationToken: movects.Token);
                moveTime += Time.deltaTime;
            }
            catch
            {
                await UniTask.Yield();
            }

        }
        await UniTask.Delay(1500, cancellationToken: movects.Token);
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
