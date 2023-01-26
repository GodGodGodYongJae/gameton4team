using Assets._Scripts.Controller;
using Assets._Scripts.Game.Weapon;
using Assets._Scripts.UI.LobbyScene;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UI_GameScene : UI_Scene
{

    enum LoadAssetGameObjects
    {
        WeaponSelect,
        //CharacterStatusSelect
    }

    enum WeaponSelectSlot 
    {
        Weapon_Slot1,
        Weapon_Slot2,
        Weapon_Slot3,
    }
 

    [SerializeField]
    GameObject btnMain;

    private GameScene GameScene;
    private WeaponSlotController WeaponSlotController;
    private WeaponController WeaponController;
    public void InitGameScene(GameScene gameScene)
    {
        GameScene = gameScene;
        WeaponSlotController = gameScene.WeaponSlotController;
        WeaponController = gameScene.WeaponController;
    }


    private Canvas canvas;
    public override bool Init()
    {
        canvas = this.GetComponent<Canvas>();
        if (base.Init() == false)
            return false;

        canvas.worldCamera = Camera.main;

        #region Bind
        BindObject(typeof(LoadAssetGameObjects));
        BindRectTrans(typeof(WeaponSelectSlot));
        GameObject selectObj = GetObject((int)LoadAssetGameObjects.WeaponSelect).gameObject;
        selectObj.SetActive(false);
        #endregion

        Managers.Events.AddListener(Define.GameEvent.playerEvents, UIEvent);
        Managers.Events.AddListener(Define.GameEvent.stageClear, StageClearEvent);
        return true;
    }


    struct WeaponCard
    {
        public Sprite image;
        public string name;
        public string Explanation;
    }


    private const int SelectCardNum = 3;
    private async UniTaskVoid OpenWeaponSelectBox()
    {
        Time.timeScale = 0;
        GameObject selectObj =  GetObject((int)LoadAssetGameObjects.WeaponSelect).gameObject;
        selectObj.SetActive(true);
        for (int i = 0; i < SelectCardNum; i++)
        {
            int RandomSlotWeapon = Random.Range((int)Define.WeaponType.None + 1, (int)Define.WeaponType.End - 1);
            WeaponSlotController.WeaponSlot slot = WeaponSlotController.GetWeapon((Define.WeaponType)RandomSlotWeapon);
            WeaponCard card = new WeaponCard();
            if ( slot == null)
            {
                slot = new WeaponSlotController.WeaponSlot((Define.WeaponType)RandomSlotWeapon);
                await UniTask.WaitUntil(() => { return slot.weaponData != null; });
                card.image = slot.weaponData.UIImage;
                card.name = slot.weaponData.DisplayName;
                card.Explanation = "무기 선택하기!";

            }
            else
            {
                card.image = slot.weaponData.UIImage;
                card.name = slot.weaponData.DisplayName;
                int randomUpgrade = Random.Range(0, (int)WeaponData.UpgradeType.end -1);
                string discriptions = "";
                int NextUpgradeNum = 0;
                int CurrentUpgradeNum = 0;
                switch ((WeaponData.UpgradeType)randomUpgrade)
                {
                    case WeaponData.UpgradeType.AttackDamage:
                        NextUpgradeNum = slot.weaponData.GetLevelData(WeaponData.UpgradeType.AttackDamage, slot.atklevel + 1);
                        CurrentUpgradeNum = slot.weaponData.GetLevelData(WeaponData.UpgradeType.AttackDamage, slot.atklevel);
                        discriptions = " 공격력 증가 :" + (NextUpgradeNum - CurrentUpgradeNum).ToString() + "\n" + " 현재 공격력 : " + CurrentUpgradeNum;
                        break;
                    case WeaponData.UpgradeType.AttackSpeed:
                        NextUpgradeNum = slot.weaponData.GetLevelData(WeaponData.UpgradeType.AttackSpeed, slot.atklevel + 1);
                        CurrentUpgradeNum = slot.weaponData.GetLevelData(WeaponData.UpgradeType.AttackSpeed, slot.atklevel);
                        discriptions = " 공격속도 증가 :" + (NextUpgradeNum - CurrentUpgradeNum).ToString() + "\n" + " 현재 공격속도 : " + CurrentUpgradeNum;
                        break;

                }
                card.Explanation = discriptions;
            }
            SlotCardUISet((WeaponSelectSlot)i, card);
        }
    }

    private void SlotCardUISet(WeaponSelectSlot slot, WeaponCard card)
    {
       GameObject SlotGo = GetRectTransform((int)slot).gameObject;
        Text NameTxt = SlotGo.transform.Find("Name").GetComponent<Text>();
        Image image = SlotGo.transform.Find("Image").GetComponent<Image>();
        Text ExplanationTxt = SlotGo.transform.Find("Explanation").GetComponent<Text>();
       
        NameTxt.text = card.name;
        image.sprite = card.image;
        ExplanationTxt.text = card.Explanation;
        SlotGo.BindEvent(ResetTest);
    }

    private void ResetTest()
    {
        OpenWeaponSelectBox().Forget();
    }
    private void StageClearEvent(Define.GameEvent eventType, Component Sender, object param)
    {
        if(eventType == Define.GameEvent.stageClear)
        {
            OpenWeaponSelectBox().Forget();
        }
    }


    private void UIEvent(Define.GameEvent eventType, Component Sender, object param)
    {
        if(eventType == Define.GameEvent.playerEvents && param != null)
        {
            Player.PlayerActionKey key = (Player.PlayerActionKey)param;
            if (key != Player.PlayerActionKey.Death) return;
            GameObject go = btnMain.transform.parent.gameObject;
            go.SetActive(true);
            Button btn = btnMain.AddComponent<Button>();
            btn.onClick.AddListener(() => {
              
                Time.timeScale = 1;
                Managers.Scene.ChangeScene(Define.SceneType.Lobby);
               Managers.OnDestorys();
            });
        }
    }
}

