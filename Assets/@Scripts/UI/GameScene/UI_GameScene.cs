using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class UI_GameScene : UI_Scene
{

    private Canvas canvas;
    public override bool Init()
    {
        canvas = this.GetComponent<Canvas>();
        if (base.Init() == false)
            return false;

        canvas.worldCamera = Camera.main;

        return true;
    }
}

