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

    Transform _prevWall;
    Transform _nextWall;
    public void Init(GameScene gameScene)
    {
       _cam = this.GetComponent<Camera>();
       _scene = gameScene;
       _player = _scene.Player;
       _playerTransform = _player.transform;
        
       _height = _cam.orthographicSize;
       _width = _height * Screen.width / Screen.height;
        _prevWall = _scene.WallObjects[(int)GameScene.Wall.Prev].transform;
        _nextWall = _scene.WallObjects[(int)GameScene.Wall.Front].transform;
        Managers.FixedUpdateAction += CameraUpdate;//LimitCameraArea; 

    }
    float moveCenter, minMapSize, maxMapSize = 0;
    void CameraUpdate()
    {
        //camera 포지션 잡기.
        _cameraPosition.x = (_player.GetPlayerLeft() > 0) ? _player.Speed : -_player.Speed;
        _cameraMoveSpeed = _player.Speed / 2;

        float lx = -_width + transform.position.x;
        if (lx > _prevWall.position.x) _prevWall.position = new Vector2(lx, _prevWall.position.y);

        transform.position = Vector3.Lerp(transform.position, _playerTransform.position + _cameraPosition, Time.deltaTime * _cameraMoveSpeed);

        if (moveCenter < _player.transform.position.x) moveCenter = _playerTransform.position.x;

        minMapSize = _prevWall.position.x;
        maxMapSize = _nextWall.position.x;
        _mapSize.x = Mathf.Abs(minMapSize - maxMapSize) / 2;
        _center.x = (Mathf.Abs(maxMapSize) - Mathf.Abs(minMapSize)) / 2 + _prevWall.position.x;
        transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
        LimitCameraArea();

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
