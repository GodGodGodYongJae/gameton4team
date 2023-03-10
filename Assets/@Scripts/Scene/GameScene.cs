using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using static Define;
using Assets._Scripts.Controller;

public class GameScene : BaseScene
{
    public enum Wall
    {
        Prev,Front
    }

    UI_GameScene _gameSceneUI;

    [SerializeField]
    GroundGenerator[] _groundGenerator;

    [SerializeField]
    GameObject[] _wallObjects;

    WeaponController weaponController;
    WeaponSlotController weaponSlotController = new WeaponSlotController();

    GameObject _playerGo;
    Player _player;
    public GameObject[] WallObjects { get { return _wallObjects; } }
    public GameObject PlayerGo { get { return _playerGo; } }
    public Player Player { get { return _player; } }
    
    GroundController _groundController;
    CameraController cameraController;
    public GroundController GroundContoroller => _groundController;
    public WeaponSlotController WeaponSlotController => weaponSlotController;
    public WeaponController WeaponController => weaponController;

    private int StageIdx = 0;
    public int StageIndex => StageIdx;
    protected override bool Init()
    {

        if (base.Init() == false)
            return false;

        Managers.Monster.Init();

        SceneType = SceneType.GameScene;

        Managers.UI.ShowSceneUI<UI_GameScene>(callback: (gameSceneUI) =>
        {
            _gameSceneUI = gameSceneUI;
            _gameSceneUI.InitGameScene(this);
            WaitLoad().Forget();
        });

        Managers.Events.AddListener(Define.GameEvent.stageClear, StageClear);

        return true;
    }

    private void StageClear(Define.GameEvent eventType, Component Sender, object param)
    {
        if(eventType == Define.GameEvent.stageClear && Utils.EqualSender<Ground>(Sender))
        {
            if (_groundGenerator.Length - 1 > StageIdx)
                StageIdx++;

            cameraController.FadeInOut(
                () => {
                    //TODO UI나 무기 교체 로직 다 진행하면 됨.
                    this.WaitLoadGround(() => Managers.Events.PostNotification(Define.GameEvent.stageClear, this, _groundGenerator[StageIdx]));
                });

           
        }

    }

    async UniTaskVoid WaitLoad()
    {
  
        while (Managers.UI.SceneUI == null)
            await UniTask.NextFrame();

        //Player 생성.
        _playerGo = Managers.Object.InstantiateSingle(StringData.Player, new Vector2(0, 0));
        _player = _playerGo.GetComponent<Player>();

        Managers.Object.InstantiateSingle("coin", new Vector2(0, 0));
        #region DI
        //무기 DI 

        weaponController = new WeaponController(this); 
        //지형 DI
        _groundController = new GroundController(this, _groundGenerator[StageIdx]);
        //카메라 DI
        cameraController = Camera.main.GetComponent<CameraController>();
        cameraController.Init(this);
        cameraController.FadeInOut();
        #endregion

        //플레이어 무기 슬롯 & 실제 등록.
        Weapon weapon = await weaponController.WeaponChange(WeaponType.Weapon_Sword);
        WeaponSlotController.WeaponSlot weaponSlot = new WeaponSlotController.WeaponSlot(weapon.weaponData);
        weaponSlot.Type = WeaponType.Weapon_Sword;
        WeaponSlotController.NewWeapon(weaponSlot);

        _gameSceneUI.SyncInventoryInfo();
        //_gameSceneUI.selectWeaponUI.OpenWeaponSelectBox().Forget();
        Managers.Sound.PlayBGM("InGame1");

         //지형 등록
         ////차후 Data 불러와서, 바꿔야 함.
         WaitLoadGround(() => _groundController.Init().Forget());

    }

     void WaitLoadGround(Action callback = null)
    {

        _groundController.ClearPrevStageData();

        foreach (var item in _groundGenerator[StageIdx].Grounds)
        {
           Managers.Object.RegisterObject(item.name, Define.PoolGroundSize);
        }
        callback?.Invoke();
    }
}
