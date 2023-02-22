using Assets._Scripts.Game._Creature._Player._Helper;
using Assets._Scripts.Game._Creature._Player._Helper.HelperInterface;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : Creature
{
    public enum PlayerActionKey
    {
        None,
        Direction,
        Jump,
        Damage,
        Death,
        End
    }
    enum PlayerHelperKey
    {
        Controll,
        Level,
        Damage
    }

    private Dictionary<PlayerActionKey, Action> PlayerAction = new Dictionary<PlayerActionKey, Action>();
    private Dictionary<PlayerHelperKey, PlayerHelper> PlayerHelper = new Dictionary<PlayerHelperKey, PlayerHelper>();

    private SPUM_Prefabs SPUM;
    private PlayerData playerData; // 데이터
    protected override void Awake()
    {
        base.Awake();
        SPUM = transform.GetChild(0).GetComponent<SPUM_Prefabs>();
        DataInit();
        ActionInit();
        HelperInit();
    }

    public void Respawn()
    {
        IDamageHelper helper = (IDamageHelper)PlayerHelper[PlayerHelperKey.Damage];
        helper.Respawn();
    }

    public void AddHp(float add)
    {
        IDamageHelper helper = (IDamageHelper)PlayerHelper[PlayerHelperKey.Damage];
        helper.AddHp(add);
    }

    #region Init 함수 

    private void DataInit()
    {
        _type = Type.Player;
        playerData = (PlayerData)_creatureData;
        playerData.AssetLoad();
    }

    private void HelperInit()
    {
        PlayerHelper.Add(PlayerHelperKey.Damage, new DamageHelper(this));
        PlayerHelper.Add(PlayerHelperKey.Controll, new ControllHelper(this));
        PlayerHelper.Add(PlayerHelperKey.Level, new LevelHelper(this));

    }

    /// <summary>
    /// 이벤트 액션 등록
    /// </summary>
    private void ActionInit()
    {
        for (int i = 0; i < (int)PlayerActionKey.End; i++)
        {
            PlayerAction.Add((PlayerActionKey)i, null);
        }
        Managers.Events.AddListener(Define.GameEvent.playerEvents, PlayerActionCall);
    }
    #endregion

    #region override
    public override void Damage(float Dmg, Creature Sender)
    {
        IDamageHelper helper = (IDamageHelper)PlayerHelper[PlayerHelperKey.Damage];
        helper.Damage(Dmg, Sender);
    }
    
    
    #endregion


    #region Getter
    public float Speed { get { return playerData.Speed; } }
    public float GetPlayerDamage(int weaponDamage)
    {
        float CriticalSuccess = Random.Range(0.0f, 100.0f);
        float CriticalDmg = 1;
        if (CriticalSuccess <= playerData.CriticalProbaility)
            CriticalDmg = 1.75f;
        return (playerData.AttackDamage + weaponDamage) * CriticalDmg;
    }

    public PlayerData PlayerData => playerData;
    public Vector2 DirectionVector()
    {
        IControllerHelper Direction = (IControllerHelper)PlayerHelper[PlayerHelperKey.Controll];
        return Direction.returnToDirection();
    }

    public SPUM_SpriteList GetSPUMSpriteList => SPUM._spriteOBj;
    #endregion

    /// <summary>
    /// 위치 초기화
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void InitPosition(float x = 0, float y = 0)
    {
        transform.position = new Vector2(x, y);
    }

    /// <summary>
    /// 액션 메서드 등록
    /// </summary>
    /// <param name="key"></param>
    /// <param name="action"></param>
    public void PlayerActionAdd(PlayerActionKey key, Action action)
    {
        PlayerAction[key] += action;
    }
    /// <summary>
    /// 액션 리스너
    /// </summary>
    private void PlayerActionCall(Define.GameEvent eventType, Component Sender, object param)
    {
        if (eventType == Define.GameEvent.playerEvents)
        {
            PlayerActionKey key = (PlayerActionKey)param;
            PlayerAction[key]?.Invoke();
        }
    }
}