using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    float _speed = 5.0f;
    
    //���ӵ�. 
    float _acceleration = 0f;
    
    private Rigidbody2D _rigidBody;


    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        Managers.FixedUpdateAction += FUpdate;
    }
    // ���� Move ��ü�� Component�� �����ϰų� �и��ؾ���.
    void FUpdate()
    {
        //�ִ� ���ӵ��� ���� ���ǵ��� 2�� ��ŭ
        _acceleration = _speed * 2;
        _rigidBody.AddForce(Vector2.right * _speed);

        if (_rigidBody.velocity.x >= _acceleration) _rigidBody.velocity = new Vector2(_acceleration, _rigidBody.velocity.y);
        else if (_rigidBody.velocity.x <= -_acceleration) _rigidBody.velocity = new Vector2(-_acceleration, _rigidBody.velocity.y);
    }

 
}
