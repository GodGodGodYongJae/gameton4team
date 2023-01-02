using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMonster : Creature
{
    Creature _target;
    Rigidbody2D _rigid;
    private void Start()
    {
        _type = Type.Monster;
        _rigid = GetComponent<Rigidbody2D>();
        _target = GameObject.Find("Player").GetComponent<Player>();
        //Managers.FixedUpdateAction += FUpdate;
        MoveDelay().Forget();
    }

    void FUpdate()
    {
       // transform.Translate(_target.transform.position * _speed * Time.deltaTime);
        //_rigid.MovePosition();
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        Creature creature = collision.gameObject.GetComponent<Creature>();
        if (creature == null) return;

        if (creature.GetType() == Creature.Type.Player)
        {
            creature.Damage(_creatureData.AttackDamage, this);
        }
    }

    async UniTaskVoid MoveDelay()
    {
        float moveTime = 0;
        while (moveTime < 0.8f)
        {
            float remainigDistance = (transform.position - _target.gameObject.transform.position).sqrMagnitude;
            Vector3 newPos = Vector3.MoveTowards(_rigid.position, _target.gameObject.transform.position, _creatureData.Speed * Time.deltaTime);
            _rigid.MovePosition(newPos);
            remainigDistance = (transform.position - _target.gameObject.transform.position).sqrMagnitude;

            await UniTask.WaitForFixedUpdate();
            moveTime += Time.deltaTime;

        }
        await UniTask.Delay(1500);
        MoveDelay().Forget();
    }
    async UniTaskVoid Move()
    {
        float remainigDistance = (transform.position - _target.gameObject.transform.position).sqrMagnitude;
        while(remainigDistance > float.Epsilon)
        {
            Vector3 newPos = Vector3.MoveTowards(_rigid.position,_target.gameObject.transform.position, _creatureData.Speed * Time.deltaTime);
            _rigid.MovePosition(newPos);
            remainigDistance = (transform.position - _target.gameObject.transform.position).sqrMagnitude;

            await UniTask.WaitForFixedUpdate();
        }
        Move().Forget();
    }
}
