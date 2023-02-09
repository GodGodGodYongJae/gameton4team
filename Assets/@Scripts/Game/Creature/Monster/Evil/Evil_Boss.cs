using Assets._Scripts.Game.Weapon;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MonsterLove.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;

public class Evil_Boss : SPUM_Monster
{
    [SerializeField]
    BoxCollider2D AttackBox;

    protected override void Awake()
    {
        base.Awake();
        fsm = new StateMachine<States>(this);
        MonsterAttackCol col = AttackBox.gameObject.GetOrAddComponent<MonsterAttackCol>();
        col.CreateAttackCol(this);
    }



    // 리플렉션으로 구현됨 MonsterLove ( FSM 플러그인 사용 ) 
    #region FSM

    //이동 후 다음 이동 쿨타임
    float moveDealy = 0;

    //공격 쿨타임
    float attackDealy = 0;
    void IDLE_Enter()
    {
        sPUM_Prefab.PlayAnimation("0_idle");
    }
    void IDLE_Update()
    {
        float distance = Vector2.Distance(transform.position, target.transform.position);
        attackDealy += Time.deltaTime;

        if (attackDealy >= monsterData.AttackDealy && _rigid.velocity.y == 0)
        {
            Observable.Timer(TimeSpan.FromMilliseconds(1000))
             .Subscribe(_ => fsm.ChangeState(States.ATTACK));
        }
        else
        {
            moveDealy -= Time.deltaTime;
            if (moveDealy <= 0)
            {
                moveDealy = monsterData.MoveDealy;
                fsm.ChangeState(States.MOVE);
            }
        }
    }

    void MOVE_Enter()
    {
        sPUM_Prefab.PlayAnimation("1_Run");
        float direction = (transform.position.x > target.transform.position.x) ? Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x) * -1;
        transform.localScale = new Vector2(direction, transform.localScale.y);
        MoveSync().Forget();
    }
    async UniTaskVoid MoveSync()
    {
        float moveTime = 0;
        while (moveTime < monsterData.MovementTime && _rigid.velocity.y == 0)
        {
            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance <= monsterData.AttackRange)
            {
                if (attackDealy <= 0)
                    fsm.ChangeState(States.ATTACK);
                else
                    fsm.ChangeState(States.IDLE);
            }

            try
            {
                Vector2 targetPos = new Vector2(target.transform.position.x, transform.position.y);
                Vector2 newPos = Vector2.MoveTowards(_rigid.position, targetPos, _creatureData.Speed * Time.deltaTime);
                _rigid.MovePosition(newPos);
                await UniTask.WaitForFixedUpdate(cancellationToken: movects.Token);
                moveTime += Time.deltaTime;
            }
            catch
            {
                await UniTask.Yield();
            }

        }
        fsm.ChangeState(States.IDLE);
    }

    private float attackTime = 2f;
    Vector2 pos;
    void ATTACK_Enter()
    {
        attackTime = 2f;
        pos = target.gameObject.transform.position;
        Observable.Timer(TimeSpan.FromMilliseconds(1000)).
            Subscribe(_ =>
            {
                try
                {
                    float direction = (transform.position.x > target.transform.position.x) ? Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x) * -1;
                    transform.localScale = new Vector2(direction, transform.localScale.y);
                    AttackAsync().Forget();
                }
                catch
                {

                }

            }
         );

    }
    async UniTaskVoid AttackAsync()
    {

        sPUM_Prefab.PlayAnimation("2_Attack_Normal");
        //TODO
        AttackBox.enabled = true;

        Vector2 targetPos = new Vector2(target.transform.position.x, transform.position.y);

        while (_rigid.position != targetPos)
        {
            Vector2 newPos = Vector2.MoveTowards(_rigid.position, targetPos, (_creatureData.Speed * 1.5f) * Time.deltaTime);
            _rigid.MovePosition(newPos);
            await UniTask.WaitForFixedUpdate(cancellationToken: cts.Token);
            attackTime -= Time.deltaTime;
            if (attackTime <= 0) break;
        }

        this.attackDealy = 0;
        AttackBox.enabled = false;
        fsm.ChangeState(States.IDLE);


    }
    #endregion
    public override void Damage(float dmg, Creature Target)
    {
        base.Damage(dmg * 0.9f, Target);
        creatureHPBar.Damage(_hp, _creatureData.MaxHP);
    }
}
