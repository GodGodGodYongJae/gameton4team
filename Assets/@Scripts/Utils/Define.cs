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

    // 풀링할 그라운드 숫자.
    public const int PoolGroundSize = 4;
    // 그라운드 생성 y값 위치 
    public const float GroundPosY = -5f;
    public StringData stringData;

}
