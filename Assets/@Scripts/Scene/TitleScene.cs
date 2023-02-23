using Cysharp.Threading.Tasks;
using System;
using System.Linq;

using UnityEngine;
using static Define;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.ResourceLocations;
using static UnityEngine.EventSystems.EventTrigger;

#if UNITY_EDITOR
using UnityEditor.AddressableAssets;
#endif

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
            LoadDatas().Forget();
            WaitLoad().Forget();
        });



        return true;
    }
    int loadCount = 0;

    async UniTaskVoid WaitLoad()
    {
        await UniTask.WaitUntil(() => { return LoadSuccess == true; });
        Debug.Log("Load Complated");
            _titleSceneUI.ReadyToStart();
    }
    // 어드레서블의 Label을 얻어올 수 있는 필드.
    public AssetLabelReference assetLabel;

    private IList<IResourceLocation> _locations;
    bool LoadSuccess = false;
    private async UniTaskVoid LoadDatas()
    {
        bool waitLoad = false;
        Addressables.LoadResourceLocationsAsync(assetLabel.labelString).Completed +=
            (handle) =>
            {
                _locations = handle.Result;
                waitLoad = true;
            };

        await UniTask.WaitUntil(() => { return waitLoad == true; });

        for (int i = 0; i < _locations.Count; i++)
        {
            string Key = _locations[i].PrimaryKey;
            Type type = _locations[i].ResourceType;
            if (type == typeof(Texture2D)) { loadCount++; continue; }
         
            var Resources = typeof(ResourceManager).GetMethod("LoadAsync2");
            var refs = Resources.MakeGenericMethod(type);
            bool WaitLoadData = false;
            _titleSceneUI.statusText.text = "Wait! Load [" + Key + "] Data";
            refs.Invoke(Managers.Resource, new object[] { Key,
                        (Action)(() => { loadCount++; WaitLoadData = true; })
                    });;
            await UniTask.WaitUntil(() => { return WaitLoadData == true; });
        }
        await UniTask.WaitUntil(() => { return _locations.Count <= loadCount; });
        //var location = _locations[UnityEngine.Random.Range(0, _locations.Count)];
        LoadSuccess = true;
    }

#if UNITY_EDITOR
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
                    if (entriType == typeof(Texture2D))
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
#endif
}
