using Cysharp.Threading.Tasks;
using MonsterLove.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;



//�䱸����, ĳ���Ͱ� �þ߰Ÿ��ȿ� ������ �پ �ٰ����� �����Ÿ��ȿ� ��� ����
//2�ʸ��� ������ �ֵθ�
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
    //�̵������� �ð� 
    float moveTime = 0;
    //�̵� �� ���� �̵� ��Ÿ��
    float moveDealy = 0;
    // ���÷������� ������ MonsterLove ( FSM �÷����� ��� ) 
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
        Debug.Log("����!");
        fsm.ChangeState(States.IDLE);
    }
    #endregion

}
