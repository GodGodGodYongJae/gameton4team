using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;


public class TitleScene : BaseScene
{
    UI_TitleScene _titleSceneUI;
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = SceneType.TitleScene;

        Managers.UI.ShowSceneUI<UI_TitleScene>(callback: (titleSceneUI) =>
        {
            _titleSceneUI = titleSceneUI;
        });

        WaitLoad().Forget();
        return true;
    }

    async UniTaskVoid WaitLoad()
    {
        //while (Managers.Data.Loaded() == false)
        //    yield return null;

        while (Managers.UI.SceneUI == null)
            await UniTask.NextFrame();

        //while (Managers.Game.IsLoaded == false)
        //    yield return null;

            _titleSceneUI.ReadyToStart();
    }
}
