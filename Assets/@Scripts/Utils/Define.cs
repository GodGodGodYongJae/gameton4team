using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum UIEvent 
    { 
        Click,
        Press
    }

    public enum SceneType 
    {
        Unknown,
        GameScene,
        Lobby,
        TitleScene
    }

    public const int PoolGroundSize = 4;
    public StringData stringData;

}
