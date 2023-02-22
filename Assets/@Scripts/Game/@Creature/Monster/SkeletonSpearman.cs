
using Cysharp.Threading.Tasks;
using MonsterLove.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;

//요구조건,
//3초마다 창을 찔러 공격 
public class SkeletonSpearman : SPUM_Monster
{

    protected override void Awake()
    {
        base.Awake();
        fsm = new StateMachine<States>(this);
        GameObject WeaponHandGo = _root._weaponList[2].gameObject;
        MonsterAttackCol col = WeaponHandGo.AddComponent<MonsterAttackCol>();
        col.CreateAttackCol(this);
        Attackbox = WeaponHandGo.AddComponent<BoxCollider2D>();
        Attackbox.isTrigger = true;
        Attackbox.enabled = false;
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
        attackDealy -= Time.deltaTime;

        if (distance <= monsterData.AttackRange &&
            attackDealy <= 0)
        {
            fsm.ChangeState(States.ATTACK);
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
        while (moveTime < 0.8f && _rigid.velocity.y == 0)
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

    void ATTACK_Enter()
    {
        AttackAsync().Forget();
    }


    async UniTaskVoid AttackAsync()
    {
        string attackString = "2_Attack_Normal";
        sPUM_Prefab.PlayAnimation(attackString);
        float frameTime = (attackAnimSync / 60f) * 1000;
        try {
            float endFrameTime = (sPUM_Prefab.GetAnimFrmae(attackString) / 60f) * 1000f - frameTime;
            await UniTask.Delay((int)frameTime, cancellationToken: cts.Token);
            Attackbox.enabled = true;
            await UniTask.Delay((int)endFrameTime, cancellationToken: cts.Token);
            Attackbox.enabled = false;
            this.attackDealy = monsterData.AttackDealy;
            fsm.ChangeState(States.IDLE);
        }
        catch
        {
           await UniTask.Yield();
        }

    }
    #endregion

}
