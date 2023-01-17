using Cysharp.Threading.Tasks;
using MonsterLove.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;



//요구조건, 캐릭터가 시야거리안에 들어오면 뛰어서 다가가고 사정거리안에 들면 공격
//2초마다 도끼를 휘두름
public class OrcWarrior : SPUM_Monster
{
    enum States
    {  
        IDLE,
        MOVE,
        ATTACK,
        DEATH
    }
    StateMachine<States> fsm;

    protected override void Awake()
    {
        fsm = new StateMachine<States>(this);
        fsm.ChangeState(States.IDLE);
        base.Awake();
    }

    public override void Damage(int dmg, Creature Target)
    {
        base.Damage(dmg, Target);
        creatureHPBar.Damage(_hp, _creatureData.MaxHP);
    }

    #region FSM
    //이동가능한 시간 
    float moveTime = 0;
    //이동 후 다음 이동 쿨타임
    float moveDealy = 0;
    // 리플렉션으로 구현됨 MonsterLove ( FSM 플러그인 사용 ) 
    void IDLE_Update()
    {
        float distance = Vector2.Distance(transform.position, target.transform.position);
        if (distance <= monsterData.AttackRange)
        {
            fsm.ChangeState(States.ATTACK);
        }
        else
        {
            moveDealy -= Time.deltaTime;
            Debug.Log(moveDealy);
            if (moveDealy <= 0)
            {
                fsm.ChangeState(States.MOVE);
            }
        }
    }

    void MOVE_Update()
    {
        if(moveTime < monsterData.MoveTime)
        {
            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance <= monsterData.AttackRange)
            {
                fsm.ChangeState(States.ATTACK);
            }

            float remainigDistance = (transform.position - target.gameObject.transform.position).sqrMagnitude;
            Vector3 newPos = Vector3.MoveTowards(_rigid.position, target.gameObject.transform.position, monsterData.Speed * Time.deltaTime);
            _rigid.MovePosition(newPos);
            remainigDistance = (transform.position - target.gameObject.transform.position).sqrMagnitude;
            moveTime += Time.deltaTime;
        }
        else
        {
            fsm.ChangeState(States.IDLE);
        }

    }
    void MOVE_Exit()
    {
        moveTime = 0;
        moveDealy = monsterData.MoveDealy;
    }
    void ATTACK_Enter()
    {
        Debug.Log("공격!");
        fsm.ChangeState(States.IDLE);
    }
    #endregion

}
