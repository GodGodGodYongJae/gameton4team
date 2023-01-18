using Cysharp.Threading.Tasks;
using MonsterLove.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;

// 요구사항 Player와 가까워지면 은신
public class Berserk : SPUM_Monster
{

    [SerializeField]

    BoxCollider2D AttackBox;
  
    [SerializeField]
    private float attackAnimSync = 0.5f;
    protected override void Awake()
    {
        base.Awake();
        fsm = new StateMachine<States>(this);
    }

    public override void Damage(int dmg, Creature Target)
    {
        base.Damage(dmg, Target);
        creatureHPBar.Damage(_hp, _creatureData.MaxHP);
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

        if (attackDealy >= monsterData.AttackDealy)
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
    }
    //이동가능한 시간 
    float moveTime = 0;
    void MOVE_Update()
    {

        if (moveTime < monsterData.MoveTime)
        {
            float distance = Vector2.Distance(transform.position, target.transform.position);

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
    private float attackTime = 4f;
    Vector2 pos;
    void ATTACK_Enter()
    {
        attackTime = 4f;
        pos = target.gameObject.transform.position;
        float direction = (transform.position.x > target.transform.position.x) ? Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x) * -1;
        transform.localScale = new Vector2(direction, transform.localScale.y);
    }
    void ATTACK_Update()
    {
        sPUM_Prefab.PlayAnimation("2_Attack_Normal");
        //TODO
        AttackBox.enabled = true;

        Vector3 newPos = Vector3.MoveTowards(_rigid.position, pos, monsterData.Speed * Time.deltaTime * 2.5f);
        newPos.y = transform.position.y;
        _rigid.MovePosition(newPos);
        attackTime -= Time.deltaTime;

        if (attackTime <= 0 || _rigid.position == pos)
        {
            this.attackDealy = 0;
            AttackBox.enabled = false;
            fsm.ChangeState(States.IDLE);
            
        }
    }
    #endregion

}
