using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_TitleScene : UI_Scene
{
    enum LoadAssetGameObjects
    {
        BG,
        //TitleText,
        end
    }

    enum Texts
    {
        StartText,
    }

    bool _isLoaded = false;

    private CanvasScaler _canvas;
    private PlayFab_Login login;
    public override bool Init()
    {
        _canvas = this.GetComponent<CanvasScaler>();
        if (base.Init() == false)
            return false;

        BindObject(typeof(LoadAssetGameObjects));
        BindText(typeof(Texts));
        login = gameObject.GetOrAddComponent<PlayFab_Login>();

        GetObject((int)LoadAssetGameObjects.BG).BindEvent(OnClickBG);


        for (int i = 0; i < (int)LoadAssetGameObjects.end; i++)
        {
            GameObject Go = GetObject(i);
            string loadNameAsset = this.name + "_" + Go.name;
            Managers.Resource.LoadAsync<Sprite>(loadNameAsset, (success) =>
            {
                Go.GetComponent<Image>().sprite = success;
            });
        }

        login.OnLogin(() =>
        {
            GetText((int)Texts.StartText).text = "Tap To Start!";
       
        });
        
        //Managers.Sound.Play(Sound.Effect, "Sound_Opening");
        //Screen.SetResolution(Screen.width, (Screen.width / 9) * 16, true);
        MatchDisplay();
        return true;
    }


    void MatchDisplay()
    {
        _canvas.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        float fixedAspectRatio = 9f / 16f;
        float currentAspectRatio = (float)Screen.width / (float)Screen.height;
        if (currentAspectRatio > fixedAspectRatio)
        {
            _canvas.matchWidthOrHeight = 1;
        }
        else if (currentAspectRatio < fixedAspectRatio)
        {
            _canvas.matchWidthOrHeight = 0;
        }
    }

    public void ReadyToStart()
    {
        _isLoaded = true;
        GetText((int)Texts.StartText).enabled = true;
    }

    #region EventHandler
    void OnClickBG()
    {
        if(_isLoaded && login.Login)
            Managers.Scene.ChangeScene(Define.SceneType.Lobby);

        //Managers.Sound.Play(Sound.Effect, "Sound_ButtonMain");
    }
    #endregion
}
