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

    protected override bool Init()
    {

        if (base.Init() == false)
            return false;


        SceneType = SceneType.GameScene;

        Managers.UI.ShowSceneUI<UI_GameScene>(callback: (gameSceneUI) =>
        {
            _gameSceneUI = gameSceneUI;
        });

         _groundController = new GroundController(this);
        WaitLoad().Forget();
        return true;
    }

    async UniTaskVoid WaitLoad()
    {
  
        while (Managers.UI.SceneUI == null)
            await UniTask.NextFrame();


        //Player 생성.
        _playerGo = await Managers.Object.InstantiateSingle(StringData.Player, new Vector2(0, 0));
        _player = _playerGo.GetComponent<Player>();

        //지형 등록
        foreach (var item in _groundGenerator.Grounds)
        {
            await Managers.Object.RegisterObject(item.name, PoolGroundSize);
        }
        _groundController.Init(_groundGenerator.Grounds).Forget();

        ////차후 Data 불러와서, 바꿔야 함.

        //await UniTask.WaitUntil(() => { return _groundGenerator.Grounds.Count == condision; });
      //  _groundController.Init(_groundGenerator.Grounds);

    }

}
