using Cysharp.Threading.Tasks;
using MonsterLove.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;

// 요구사항 Player와 가까워지면 은신
public class ScarceAssasin : SPUM_Monster
{

    [SerializeField]

    BoxCollider2D AttackBox;
    [SerializeField]
    [Tooltip("보여질 거리")]
    float showDistance = 1.0f;

    protected override void Awake()
    {
        base.Awake();
        fsm = new StateMachine<States>(this);
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
    }
    //이동가능한 시간 
    float moveTime = 0;
    void MOVE_Update()
    {
        if (moveTime < monsterData.MoveTime)
        {
            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance <= showDistance)
            {
                spriteListEnable(true);
            }
           else
            {
                spriteListEnable(false);
            }

            if (distance <= monsterData.AttackRange)
            {
                if (attackDealy <= 0)
                    fsm.ChangeState(States.ATTACK);
                else
                    fsm.ChangeState(States.IDLE);
            }

            float remainigDistance = (transform.position - target.gameObject.transform.position).sqrMagnitude;
            Vector3 newPos = Vector3.MoveTowards(_rigid.position, target.gameObject.transform.position, monsterData.Speed * Time.deltaTime);
            newPos.y = transform.position.y;
            _rigid.MovePosition(newPos);
            remainigDistance = (transform.position - target.gameObject.transform.position).sqrMagnitude;
            moveTime += Time.deltaTime;
        }
        if (moveTime > monsterData.MoveTime)
        {
            moveTime = 0;
            fsm.ChangeState(States.IDLE);
        }

    }
    IEnumerator ATTACK_Enter()
    {
        sPUM_Prefab.PlayAnimation("2_Attack_Normal");
        //TODO
        AttackBox.enabled = true;
        yield return new WaitForSeconds(attackAnimSync);
        AttackBox.enabled = false;
        this.attackDealy = monsterData.AttackDealy;
        fsm.ChangeState(States.IDLE);
    }
    #endregion

}
