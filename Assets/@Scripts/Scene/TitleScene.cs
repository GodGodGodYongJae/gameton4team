using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
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


        DownloadAssetsAsync().Forget();
        WaitLoad().Forget();
        return true;
    }
    int loadCount = 0;
    private async UniTaskVoid DownloadAssetsAsync()
    {

        var setting = AddressableAssetSettingsDefaultObject.Settings;
        if (setting != null)
        {
            var group = setting.FindGroup("Default Local Group");
            if (group != null)
            {
                int ass = group.entries.Count;
                for (int i = 0; i < group.entries.Count; i++)
                {
                    var entry = group.entries.ElementAt(i);
                    Type entriType = entry.MainAssetType;
                    if(entriType == typeof(Texture2D))
                    {
                        entriType = typeof(Sprite);
                    }

                    var Resources = typeof(ResourceManager).GetMethod("LoadAsync2");
                    var refs = Resources.MakeGenericMethod(entriType);
                    refs.Invoke(Managers.Resource, new object[] { entry.address, 
                        (Action)(() => { loadCount++; }) 
                    });

                }
                await UniTask.WaitUntil(() => { return group.entries.Count <= loadCount; });

               
            }
        }
       
   
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
