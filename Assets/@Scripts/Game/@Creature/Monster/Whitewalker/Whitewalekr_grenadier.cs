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




public class Whitewalekr_grenadier : SPUM_Monster
{
    public GameObject arrow;
    public GameObject bomb;
    public float direction;
    protected override void Awake()
    {
        //arrow = GetComponent<GameObject>();
        base.Awake();
        fsm = new StateMachine<States>(this);

    }


    #region FSM

    float moveDealy = 0;

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
        direction = (transform.position.x > target.transform.position.x) ? Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x) * -1;
        transform.localScale = new Vector2(direction, transform.localScale.y);
        sPUM_Prefab.PlayAnimation("1_Run", () => { MoveSync().Forget(); });


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
                {
                    moveTime = monsterData.MovementTime;
                    attackDealy = monsterData.AttackDealy;
                    fsm.ChangeState(States.ATTACK);
                }

                else
                    fsm.ChangeState(States.IDLE);
            }

            try
            {

                if (distance < monsterData.AttackRange / 2)
                {
                    Vector2 targetPos = new Vector2(transform.position.x + (direction * 2.5f), transform.position.y);
                    Vector2 newPos = Vector2.MoveTowards(_rigid.position, targetPos, _creatureData.Speed * Time.deltaTime);
                    _rigid.MovePosition(newPos);
                    await UniTask.WaitForFixedUpdate(cancellationToken: movects.Token);
                    moveTime += Time.deltaTime;
                }

                else
                {
                    Vector2 targetPos2 = new Vector2(target.transform.position.x, transform.position.y);
                    Vector2 newPos2 = Vector2.MoveTowards(_rigid.position, targetPos2, _creatureData.Speed * Time.deltaTime);
                    _rigid.MovePosition(newPos2);
                    await UniTask.WaitForFixedUpdate(cancellationToken: movects.Token);
                    moveTime += Time.deltaTime;
                }

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
        string attackString = "2_Attack_Bow";
        sPUM_Prefab.PlayAnimation(attackString);
        float frameTime = (attackAnimSync / 60f) * 1000;
        float endFrameTime = (sPUM_Prefab.GetAnimFrmae(attackString) / 60f) * 1000f - frameTime;
        await UniTask.Delay((int)frameTime, cancellationToken: cts.Token);
        float Bulletdirection = Mathf.Clamp(transform.localScale.x, -1, 1);
        await UniTask.Delay(TimeSpan.FromSeconds(0.3f));
        GameObject bulletGo = Managers.Object.InstantiateAsync(arrow.name, new Vector2(transform.position.x, transform.position.y + 0.5f));
        bulletGo.transform.localScale = new Vector2(-1 * bulletGo.transform.localScale.x * Bulletdirection, bulletGo.transform.localScale.y);
        MonsterBulletParabolaShot bullet = bulletGo.GetOrAddComponent<MonsterBulletParabolaShot>();
        bullet.InitBulletData(this);
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        GameObject bulletGo1 = Managers.Object.InstantiateAsync(bomb.name, new Vector2(bulletGo.transform.position.x, bulletGo.transform.position.y));
        bulletGo1.transform.localScale = new Vector2(-1 * bulletGo1.transform.localScale.x * Bulletdirection, bulletGo1.transform.localScale.y);
        MonsterBullet bullet1 = bulletGo1.GetOrAddComponent<MonsterBullet>();
        bullet1.InitBulletData(this);
        await UniTask.Delay((int)endFrameTime, cancellationToken: cts.Token);
        this.attackDealy = monsterData.AttackDealy;
        fsm.ChangeState(States.IDLE);
    }

    #endregion

}