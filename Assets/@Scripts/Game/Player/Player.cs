using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    float _speed = 5.0f;
    
    //가속도. 
    float _acceleration = 0f;
    
    private Rigidbody2D _rigidBody;


    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        Managers.FixedUpdateAction += FUpdate;
    }
    // 차후 Move 자체를 Component로 빼야하거나 분리해야함.
    void FUpdate()
    {
        //최대 가속도는 현재 스피드의 2배 만큼
        _acceleration = _speed * 2;
        _rigidBody.AddForce(Vector2.right * _speed);

        if (_rigidBody.velocity.x >= _acceleration) _rigidBody.velocity = new Vector2(_acceleration, _rigidBody.velocity.y);
        else if (_rigidBody.velocity.x <= -_acceleration) _rigidBody.velocity = new Vector2(-_acceleration, _rigidBody.velocity.y);
    }

 
}
