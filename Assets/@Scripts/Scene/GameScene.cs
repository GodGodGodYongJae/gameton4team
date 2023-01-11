using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using static Define;

public class GameScene : BaseScene
{
    public enum Wall
    {
        Prev,Front
    }

    UI_GameScene _gameSceneUI;

    [SerializeField]
    GroundGenerator _groundGenerator;

    [SerializeField]
    GameObject[] _wallObjects;

    GameObject _playerGo;
    Player _player;
    public GameObject[] WallObjects { get { return _wallObjects; } }
    public GameObject PlayerGo { get { return _playerGo; } }
    public Player Player { get { return _player; } }
    
    GroundController _groundController;

    public GroundController GroundContoroller { get { return _groundController; } }

    protected override bool Init()
    {

        if (base.Init() == false)
            return false;


        SceneType = SceneType.GameScene;

        Managers.UI.ShowSceneUI<UI_GameScene>(callback: (gameSceneUI) =>
        {
            _gameSceneUI = gameSceneUI;
        });

      
        WaitLoad().Forget();
        return true;
    }

    async UniTaskVoid WaitLoad()
    {
  
        while (Managers.UI.SceneUI == null)
            await UniTask.NextFrame();

        //Player ����.
        _playerGo = await Managers.Object.InstantiateSingle(StringData.Player, new Vector2(0, 0));
        _player = _playerGo.GetComponent<Player>();

        #region DI
        //���� DI
        _groundController = new GroundController(this, _groundGenerator);
        //ī�޶� DI
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        cameraController.Init(this);
       
        #endregion

        //���� ���
        ////���� Data �ҷ��ͼ�, �ٲ�� ��.
        foreach (var item in _groundGenerator.Grounds)
        {
            await Managers.Object.RegisterObject(item.name, Define.PoolGroundSize);
        }
        _groundController.Init().Forget();

    }

}
