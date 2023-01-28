using Assets._Scripts.Controller;
using Assets._Scripts.Game.Weapon;
using Assets._Scripts.UI.GameScene;
using Assets._Scripts.UI.LobbyScene;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class UI_GameScene : UI_Scene
{

    enum LoadAssetGameObjects
    {
        WeaponSelect,
        WeaponSelectSkip
        //CharacterStatusSelect
    }

    enum WeaponSelectSlot 
    {
        Weapon_Slot1,
        Weapon_Slot2,
        Weapon_Slot3,
    }

    enum WeaponInventory
    {
        Inventory_Slot1,
        Inventory_Slot2,
        Inventory_Slot3,
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
        BindButton(typeof(WeaponInventory));

        GameObject selectObj = GetObject((int)LoadAssetGameObjects.WeaponSelect).gameObject;
        selectObj.SetActive(false);

        GameObject selectSkipObj = GetObject((int)LoadAssetGameObjects.WeaponSelectSkip).gameObject;
        selectSkipObj.BindEvent(OnWeaponSelectSkip);
        #endregion

        Managers.Events.AddListener(Define.GameEvent.playerEvents, UIEvent);
        Managers.Events.AddListener(Define.GameEvent.stageClear, StageClearEvent);

        // for 문이 안먹어서 btn을 배열로 빼야할듯

        Button btn1 = GetButton(0).GetComponent<Button>();
        btn1.onClick.AddListener(() => { OnChangeWeapon(btn1,0).Forget(); });

        Button btn2 = GetButton(1).GetComponent<Button>();
        btn2.onClick.AddListener(() => { OnChangeWeapon(btn2,1).Forget(); });

        Button btn3 = GetButton(2).GetComponent<Button>();
        btn3.onClick.AddListener(() => { OnChangeWeapon(btn3,2).Forget(); });
        return true;
    }

   
    public void OnWeaponSelectSkip()
    {
        Time.timeScale = 1;
        GameObject selectObj = GetObject((int)LoadAssetGameObjects.WeaponSelect).gameObject;
        selectObj.SetActive(false);
    }

    public void SyncInventoryInfo()
    {
        for (int i = 0; i < WeaponSlotController.SlotList.Count; i++)
        {
            GameObject invenSlot = GetButton(i).gameObject;
            Text Level = invenSlot.transform.Find("Level").GetComponent<Text>();
            Image image = invenSlot.transform.Find("Image").GetComponent<Image>();
            image.sprite = WeaponSlotController.SlotList[i].weaponData.UIImage;
            string levelTxt = "LV. " + WeaponSlotController.SlotList[i].level.ToString();
            Level.text = levelTxt;
            InventoryButton invenBtn = GetButton(i).gameObject.GetOrAddComponent<InventoryButton>();
            invenBtn.Slot = WeaponSlotController.SlotList[i];
        }
    }
 
    public async UniTaskVoid OnChangeWeapon(Button button,int slotNum)
    {
        WeaponSlotController.WeaponSlot slot = button.gameObject.GetOrAddComponent<InventoryButton>().Slot;
        if (slot == null) return;
        await GameScene.WeaponController.WeaponChange(slot.Type, slot.weaponData);
        WeaponSlotController.CurrentWeaponSlot = slotNum;
    }
    #region SelectCard

    struct WeaponCard
    {
        public Sprite image;
        public string name;
        public string Explanation;
        public WeaponSlotController.WeaponSlot weaponSlot;
        public WeaponData.UpgradeType UpgradeType;
    }


    private const int SelectCardNum = 3;
    public async UniTaskVoid OpenWeaponSelectBox()
    {
        Time.timeScale = 0;
        GameObject selectObj = GetObject((int)LoadAssetGameObjects.WeaponSelect).gameObject;
        selectObj.SetActive(true);
        Random typeOverlab = new Random();
        Random upgradeOverlab = new Random();
        int NextUpgradeNum = 0;
        int CurrentUpgradeNum = 0;
        for (int i = 0; i < SelectCardNum; i++)
        {
            int RandomSlotWeapon = typeOverlab.Next((int)Define.WeaponType.None + 1, (int)Define.WeaponType.End);
            WeaponSlotController.WeaponSlot slot = WeaponSlotController.GetWeapon((Define.WeaponType)RandomSlotWeapon);
            WeaponCard card = new WeaponCard();

            bool Lock = false;
            if ( slot == null)
            {
                Lock = true;
                Managers.Resource.LoadAsync<ScriptableObject>(((Define.WeaponType)RandomSlotWeapon).ToString() + "_data", (succss) =>
                {

                    slot = new WeaponSlotController.WeaponSlot((WeaponData)succss);
                    slot.Type =(Define.WeaponType)RandomSlotWeapon;
                    CurrentUpgradeNum = slot.weaponData.GetLevelData(WeaponData.UpgradeType.AttackDamage, 1);
                    int CurrentUpgradeDealay = slot.weaponData.GetLevelData(WeaponData.UpgradeType.AttackSpeed, 1);
                    int maxTargets = slot.weaponData.MaxTargets;
                    card.image = slot.weaponData.UIImage;
                    card.name = slot.weaponData.DisplayName;
                    card.Explanation = "무기 선택하기!" + "\n" + "공격력 : " + CurrentUpgradeNum + "\n" + "공격 딜레이 : " + CurrentUpgradeDealay + "\n" + "피격 가능수 : " + maxTargets;
                    card.UpgradeType = WeaponData.UpgradeType.NewWeapon;
                    Lock = false;
                });
            }
            else
            {
                card.image = slot.weaponData.UIImage;
                card.name = slot.weaponData.DisplayName;
                int randomUpgrade = upgradeOverlab.Next(1, (int)WeaponData.UpgradeType.end);
                string discriptions = "";

                WeaponData.UpgradeType upgradetype = WeaponData.UpgradeType.end;
                switch ((WeaponData.UpgradeType)randomUpgrade)
                {
                    case WeaponData.UpgradeType.AttackDamage:
                        NextUpgradeNum = slot.weaponData.GetLevelData(WeaponData.UpgradeType.AttackDamage, slot.atklevel + 1);
                        CurrentUpgradeNum = slot.weaponData.GetLevelData(WeaponData.UpgradeType.AttackDamage, slot.atklevel);
                        discriptions = " 공격력 증가 :" + (NextUpgradeNum - CurrentUpgradeNum).ToString() + "\n" + " 현재 공격력 : " + CurrentUpgradeNum;
                        upgradetype = WeaponData.UpgradeType.AttackDamage;
                        break;
                    case WeaponData.UpgradeType.AttackSpeed:
                        NextUpgradeNum = slot.weaponData.GetLevelData(WeaponData.UpgradeType.AttackSpeed, slot.atklevel + 1);
                        CurrentUpgradeNum = slot.weaponData.GetLevelData(WeaponData.UpgradeType.AttackSpeed, slot.atklevel);
                        discriptions = " 공격딜레이 감소 :" + (NextUpgradeNum - CurrentUpgradeNum).ToString() + "\n" + " 현재 딜레이 : " + CurrentUpgradeNum;
                        upgradetype = WeaponData.UpgradeType.AttackSpeed;
                        break;

                }
                card.UpgradeType = upgradetype;
                card.Explanation = discriptions;
            }
            await UniTask.WaitUntil(() => { return Lock == false; });
            card.weaponSlot = slot;
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
        Button btn = SlotGo.GetOrAddComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => { SelectCard(card); });
    }

    private void SelectCard(WeaponCard ChooseSlotWeapon)
    {
        //closeSelectBox

            Time.timeScale = 1;
        GameObject selectObj = GetObject((int)LoadAssetGameObjects.WeaponSelect).gameObject;
        selectObj.SetActive(false);

        if (ChooseSlotWeapon.UpgradeType == WeaponData.UpgradeType.NewWeapon)
        {
            WeaponSlotController.NewWeapon(ChooseSlotWeapon.weaponSlot);
        }
        else
        {
            ChooseSlotWeapon.weaponSlot.LevelUp(ChooseSlotWeapon.UpgradeType);
        }
        SyncInventoryInfo();
        //OpenWeaponSelectBox().Forget();
    }
    #endregion
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

