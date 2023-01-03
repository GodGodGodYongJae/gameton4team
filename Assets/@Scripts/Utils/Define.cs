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
    public enum GameEvent 
    {
        playerHealthChange,
        playerDead
    }

    public enum SceneType 
    {
        Unknown,
        GameScene,
        Lobby,
        TitleScene
    }
    public struct PlayerEvent_HPData
    {
        public int maxHp;
        public int curHp;
    }

    // 풀링할 그라운드 숫자.
    private const int _poolGroundSize = 5;
    // 그라운드 생성 y값 위치 
    private const float _groundPosY = -5f;
    public static int PoolGroundSize => _poolGroundSize;
    public static float GroundPosY => _groundPosY;
}
