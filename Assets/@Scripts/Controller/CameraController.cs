using System.Collections;
using System.Collections.Generic;
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


    public void Init(GameScene gameScene)
    {
       _cam = this.GetComponent<Camera>();
       _scene = gameScene;
       _player = _scene.Player;
       _playerTransform = _player.transform;
        
       _height = _cam.orthographicSize;
       _width = _height * Screen.width / Screen.height;

        Managers.FixedUpdateAction += LimitCameraArea; 

    }

    void LimitCameraArea()
    {
        //camera 포지션 잡기.
        _cameraPosition.x = (_player.GetPlayerLeft() > 0) ? 1.5f : -1.5f;
        _cameraMoveSpeed = _player.Speed * 2;
        //_cameraPosition.x = _player.GetPlayerLeft() + 0.5f; 

        //mapsize 
        _mapSize.x = _scene.GroundContoroller.GetExtendSize();
        //center 잡기.
        _center.x = _scene.GroundContoroller.GetCurrentGroundPos();


        transform.position = Vector3.Lerp(transform.position, _playerTransform.position + _cameraPosition, Time.deltaTime * _cameraMoveSpeed);

        float lx = _mapSize.x - _width;
        float clampX = Mathf.Clamp(transform.position.x, -lx + _center.x, lx + _center.x);
        //float clampX = Mathf.Clamp(transform.position.x, _t1 ,_t2 );

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
