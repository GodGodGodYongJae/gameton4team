using System.Collections;
using System.Collections.Generic;
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
    public override bool Init()
    {
        _canvas = this.GetComponent<CanvasScaler>();
        if (base.Init() == false)
            return false;

        BindObject(typeof(LoadAssetGameObjects));
        BindText(typeof(Texts));

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
       
        
        //Managers.Sound.Play(Sound.Effect, "Sound_Opening");
        //Screen.SetResolution(Screen.width, (Screen.width / 9) * 16, true);
        //MatchDisplay();
        return true;
    }

    void MatchDisplay()
    {
        _canvas.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        float fixedAspectRatio = 9f / 19f;
        float currentAspectRatio = (float)Screen.width / (float)Screen.height;
        if(currentAspectRatio > fixedAspectRatio )
        {
            _canvas.matchWidthOrHeight = 0;
        }
        else if(currentAspectRatio < fixedAspectRatio)
        {
            _canvas.matchWidthOrHeight = 1;
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
        Debug.Log("클릭했음!"+ _isLoaded);
        if(_isLoaded)
            Managers.Scene.ChangeScene(Define.SceneType.GameScene);

        //Managers.Sound.Play(Sound.Effect, "Sound_ButtonMain");
    }
    #endregion
}
