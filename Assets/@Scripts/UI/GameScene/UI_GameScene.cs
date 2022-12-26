using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameScene : UI_Scene
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }
}
