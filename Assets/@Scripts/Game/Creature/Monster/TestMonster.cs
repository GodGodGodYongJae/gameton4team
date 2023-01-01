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
        Managers.FixedUpdateAction += FUpdate;
    }

    void FUpdate()
    {
       // transform.Translate(_target.transform.position * _speed * Time.deltaTime);
        //_rigid.MovePosition();
    }
}
