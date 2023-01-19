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
        playerLevelChange,
        monsterDestroy,
        SpawnMonster,
        playerEvents,
        stageClear,
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

    public struct PlayerEvent_LevelData
    {
        public int maxExp;
        public int curExp;
    }


    // Ǯ���� �׶��� ����.
    private const int _poolGroundSize = 4;
    // �׶��� ���� y�� ��ġ 
    private const float _groundPosY = -5f;
    // Ǯ���� HPBAR ����
    private const int _poolHpBar = 10;
    public static int PoolGroundSize => _poolGroundSize;
    public static float GroundPosY => _groundPosY;
    public static int PoolHpBar => _poolHpBar;


}
