using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Lobby : UI_Scene
{
    private CanvasScaler _canvas;
    public override bool Init()
    {
        _canvas = this.GetComponent<CanvasScaler>();
        if (base.Init() == false)
            return false;


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
            _canvas.matchWidthOrHeight = 0;
        }
        else if (currentAspectRatio < fixedAspectRatio)
        {
            _canvas.matchWidthOrHeight = 1;
        }
    }

}
