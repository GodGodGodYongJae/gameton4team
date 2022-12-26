using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using static Define;

public class GameScene : BaseScene
{
    UI_GameScene _gameSceneUI;
    [SerializeField]
    GroundGenerator _groundGenerator;

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

        WaitLoad().Forget();
        return true;
    }

    async UniTaskVoid WaitLoad()
    {
  
        while (Managers.UI.SceneUI == null)
            await UniTask.NextFrame();

        //Player 생성.
        Managers.Object.Instantiate(StringData.Player, new Vector2(0, 0));

        //차후 Data 불러와서, 바꿔야 함.
        foreach (var item in _groundGenerator.Grounds)
        {
            Managers.Object.RegisterObject(item.name, PoolGroundSize);
            await UniTask.Awaiter();
        }
        //_groundController.Init(_groundGenerator.Grounds);

    }

}
