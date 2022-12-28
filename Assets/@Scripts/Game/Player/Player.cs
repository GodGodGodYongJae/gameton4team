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

    //���� ī�޶� ������ ���ִ� ��ü�� �ʿ���
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
        _rigidBody = GetComponent<Rigidbody2D>();
        Managers.FixedUpdateAction += FUpdate;
        Managers.UpdateAction += CamUpdate;
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

    //���� Camera ��ü�� �־������ 
    void CamUpdate()
    {
        Vector3 camPoint = Vector2.Lerp(_camera.transform.position, transform.localPosition,Time.deltaTime * _speed/2);
        camPoint.z = -10;
        _camera.transform.position = camPoint;
    }
}
