using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField]
    Vector3 _cameraPosition;

    [SerializeField]
    Vector2 _center;
    [SerializeField]
    Vector2 _mapSize;

    [SerializeField]
    float _cameraMoveSpeed;
    float _height;
    float _width;
    Camera _cam;

    // DI
    GameScene _scene;
    #region PlayerProperty
    Player _player;
    Transform _playerTransform;
    #endregion

    Transform _prevWall;
    Transform _nextWall;
    BoxCollider2D Cambox;
    public void Init(GameScene gameScene)
    {
       _cam = this.GetComponent<Camera>();
       _scene = gameScene;
       _player = _scene.Player;
       _playerTransform = _player.transform;
        
       _height = _cam.orthographicSize;
        _mapSize.y = _height;
        _center.y = 0.5f;//_height / 2;

       _width = _height * Screen.width / Screen.height;
       _prevWall = _scene.WallObjects[(int)GameScene.Wall.Prev].transform;
       _nextWall = _scene.WallObjects[(int)GameScene.Wall.Front].transform;

       Cambox = this.AddComponent<BoxCollider2D>();
       Cambox.isTrigger = true;
       Cambox.size = new Vector2(_width,_height)*2;

       Managers.FixedUpdateAction += CameraUpdate;//LimitCameraArea; 

    }

    public void SetPositionX(float x)
    {
        transform.position = new Vector3(x, transform.position.y, -10);
        float lx = -_width + transform.position.x;
        _prevWall.position = new Vector2(lx, _prevWall.position.y);
    }


    float minMapSize, maxMapSize = 0;
    void CameraUpdate()
    {
        //camera 포지션 잡기.
       
        if(transform.position.x >= _playerTransform.position.x)
        {
            _cameraMoveSpeed = _player.Speed / 2;
        }
        else
        {
            _cameraMoveSpeed = _player.Speed * 1.5f;
        }

        float lx = -_width + transform.position.x;
        if (lx > _prevWall.position.x) _prevWall.position = new Vector2(lx, _prevWall.position.y);

        CameraMove();

        minMapSize = _prevWall.position.x;
        maxMapSize = _nextWall.position.x - Define.NextWallSence;
        _mapSize.x = Mathf.Abs(minMapSize - maxMapSize) / 2;
        _center.x = (Mathf.Abs(maxMapSize) - Mathf.Abs(minMapSize)) / 2 + _prevWall.position.x;
        transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
        LimitCameraArea();

    }


    bool isStop = false;
    void CameraMove()
    {
        if(isStop == true)
        {    //camera 포지션 잡기.
            _cameraPosition.x = (_player.GetPlayerLeft() > 0) ? _player.Speed : -_player.Speed;
            transform.position = Vector3.Lerp(transform.position, _playerTransform.position + _cameraPosition, Time.deltaTime * _cameraMoveSpeed);
        }
        
        else
        {
            transform.position = Vector3.Lerp(transform.position,transform.position + Vector3.right , Time.deltaTime * _cameraMoveSpeed);
        }
         
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        Creature creature = collision.GetComponent<Creature>();
        if (creature == null) return;
        if(creature.GetType == Creature.Type.Monster)
        {
            isStop = true;
            return;
        }
        isStop = false;
    }

    void LimitCameraArea()
    {
        float lx = _mapSize.x - _width;
        float clampX = Mathf.Clamp(transform.position.x, -lx + _center.x, lx + _center.x);

        float ly = _mapSize.y - _height;
        float clampY = Mathf.Clamp(transform.position.y, -ly + _center.y, ly + _center.y);

        transform.position = new Vector3(clampX, clampY, -10f);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_center, _mapSize * 2);
    }


}
