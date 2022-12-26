using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class GameScene : BaseScene
{
    UI_GameScene _gameSceneUI;
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


        
    }
}
