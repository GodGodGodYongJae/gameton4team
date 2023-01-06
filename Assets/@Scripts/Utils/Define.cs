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
        monsterDestroy,
        SpawnMonster,
        playerEvents,
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

    // Ǯ���� �׶��� ����.
    private const int _poolGroundSize = 5;
    // �׶��� ���� y�� ��ġ 
    private const float _groundPosY = -5f;
    public static int PoolGroundSize => _poolGroundSize;
    public static float GroundPosY => _groundPosY;
}
