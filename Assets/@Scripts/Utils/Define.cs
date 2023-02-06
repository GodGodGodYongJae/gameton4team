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
        playerExpChange,
        monsterDestroy,
        SpawnMonster,
        playerEvents,
        stageClear,
        LobbyCurrency
    }

    public enum SceneType
    {
        Unknown,
        GameScene,
        Lobby,
        TitleScene
    }

    public enum WeaponType
    {
        None,
        Weapon_Sword,
        Weapon_Ax_n,
        Weapon_Spear_n,
        Weapon_Wand_n,
        Weapon_Bow_n,
        End
    }
    public struct PlayerEvent_HPData
    {
        public float maxHp;
        public float curHp;
    }

    public struct PlayerEvent_LevelData
    {
        public int maxExp;
        public int curExp;
    }


    // 풀링할 그라운드 숫자.
    private const int _poolGroundSize = 5;
    // 그라운드 생성 y값 위치 
    private const float _groundPosY = -5f;
    private const int _nextWallSencer = 1;

    // 풀링할 HPBAR 갯수
    private const int _poolHpBar = 10;
    public static int PoolGroundSize => _poolGroundSize;
    public static float GroundPosY => _groundPosY;
    public static int PoolHpBar => _poolHpBar;
    public static int NextWallSence => _nextWallSencer;

}
