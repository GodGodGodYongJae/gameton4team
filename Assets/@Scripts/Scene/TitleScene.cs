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

        WaitLoad().Forget();
        DownloadAssetsAsync();
        return true;
    }
    private  void DownloadAssetsAsync()
    {

        var setting = AddressableAssetSettingsDefaultObject.Settings;
        if (setting != null)
        {
            var group = setting.FindGroup("Default Local Group");
            if (group != null)
            {
                //foreach (var entri in group.entries)
                //{
                //    Managers.Resource.LoadAsync<GameObject>(entri.address,(success) => { 

                //    });

                //}

                
                for (int i = 0; i < group.entries.Count; i++)
                {
                    var entry = group.entries.ElementAt(i);
                    Type entriType = entry.MainAssetType;

                    var Resources = typeof(ResourceManager).GetMethod("LoadAsync");
                    var refs = Resources.MakeGenericMethod(entriType);
                    refs.Invoke(Managers.Resource, new object[] { entry.address, null }) ;
                    //Managers.Resource.LoadAsync<GameObject>(entriType, entriGroup[i].address, (success) =>
                    //{

                    //});
                    //Managers.Resource.LoadAsync< testa[i].MainAssetType >()
                    
                }
              
            }
        }
        Debug.Log("Success!@!@~@~@~");
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
