using Assets._Scripts.Controller;
using Assets._Scripts.Game.Items;
using Assets._Scripts.UI.GameScene;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class UI_GameScene : UI_Scene
{

    enum LoadAssetGameObjects
    {
        WeaponSelect,
        WeaponSelectSkip,
        GameOver,
        //CharacterStatusSelect
    }

    enum WeaponSelectSlot 
    {
        Weapon_Slot1,
        Weapon_Slot2,
        Weapon_Slot3,
        End
    }

    enum Buttons
    {
        Inventory_Slot1,
        Inventory_Slot2,
        Inventory_Slot3,
        Item_Inventory_Slot1,
        Item_Inventory_Slot2
    }

 
    private GameScene GameScene;
    public WeaponSlotController WeaponSlotController;
    private WeaponController WeaponController;
    private ItemSlotController ItemSlotController;
    public void InitGameScene(GameScene gameScene)
    {
        GameScene = gameScene;
        WeaponSlotController = gameScene.WeaponSlotController;
        WeaponController = gameScene.WeaponController;
        ItemSlotController = new ItemSlotController();
        // Test 
        //ItemSlotController.ItemSlot slotTest = new ItemSlotController.ItemSlot(new PortionItem(),);
        //ItemSlotController.CreateSlotItem();
    }

    //async UniTaskVoid testItem()
    //{
    //    Managers.Resource.
    //}


    private Canvas canvas;

    public GameObject WeaponSelectObj;
    public List<GameObject> CardSlotList;
    public override bool Init()
    {
        canvas = this.GetComponent<Canvas>();
        if (base.Init() == false)
            return false;

        canvas.worldCamera = Camera.main;
        CardSlotList = new List<GameObject>();
        #region Bind
        BindObject(typeof(LoadAssetGameObjects));
        BindRectTrans(typeof(WeaponSelectSlot));
        BindButton(typeof(Buttons));

        WeaponSelectObj =  GetObject((int)LoadAssetGameObjects.WeaponSelect).gameObject;
        WeaponSelectObj.SetActive(false);

        for (int i = 0; i < (int)WeaponSelectSlot.End; i++)
        {
            CardSlotList.Add(GetRectTransform(i).gameObject);
        }


        GameObject selectSkipObj = GetObject((int)LoadAssetGameObjects.WeaponSelectSkip).gameObject;
        selectSkipObj.BindEvent(OnWeaponSelectSkip);

        #endregion

        Managers.Events.AddListener(Define.GameEvent.playerEvents, UIEvent);

        ButtonSet();
        SelectWeaponUI selectWeaponUI = new SelectWeaponUI(this);
        return true;
    }
    //임시 데이터 넣기. 나중에 PlayFab 에서 아이템 이나 Equip Manager가 만들어지면 삭제 
    public ItemData PotionData;
    private void ButtonSet()
    {
        //Closure Problem 주의 할 것.
        List<Button> buttonList = new List<Button>();
        for (int i = 0; i <= (int)Buttons.Inventory_Slot3; i++)
        {
            int temp = i;
            buttonList.Add(GetButton(temp).GetComponent<Button>());

                buttonList[temp].onClick.AddListener(() => {
                    OnChangeWeapon(buttonList[temp], temp).Forget();
                });
        }

        for (int i = buttonList.Count; i <= (int)Buttons.Item_Inventory_Slot2; i++)
        {
            int temp = i;
            buttonList.Add(GetButton(temp).GetComponent<Button>());
            buttonList[temp].onClick.AddListener(() => { OnInventoryClick(); });
        }
        // 일단 나중에 고쳐야 함.
        ItemInventoryButton invenSlot1 = GetButton((int)Buttons.Item_Inventory_Slot1).gameObject.GetComponent<ItemInventoryButton>();
        PortionItemData portionData = (PortionItemData)PotionData;
        portionData.CreateItem();

        int Quantity = Managers.PlayFab.FindItemQuantity(portionData.Key);
    
        PortionItem portionItem = new PortionItem(portionData, Quantity);
        invenSlot1.Slot = new ItemSlotController.ItemSlot(portionItem);
        ItemSlotController.CreateSlotItem(invenSlot1.Slot);
        GameObject invenSlot = GetButton((int)Buttons.Item_Inventory_Slot1).gameObject;
        Image image = invenSlot.transform.Find("Image").GetComponent<Image>();
        image.sprite = portionData.IconSprite;
        Text text = invenSlot.transform.Find("Quantity").GetComponent<Text>();
            text.text = Quantity.ToString();



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
        if (slot == null || slotNum == WeaponSlotController.CurrentWeaponSlot) return;
        await GameScene.WeaponController.WeaponChange(slot.Type, slot.weaponData);
        WeaponSlotController.CurrentWeaponSlot = slotNum;
        Managers.Sound.PlaySFX("Weapon_Selection");

    }

    public void OnInventoryClick()
    {
        ItemInventoryButton slot = GetButton((int)Buttons.Item_Inventory_Slot1).gameObject.GetComponent<ItemInventoryButton>();
        if (slot.IsCoolTime) return;
        if(ItemSlotController.Slots.Count == 0 || ItemSlotController.Use(0) == false) return;
        GameObject invenSlot = GetButton((int)Buttons.Item_Inventory_Slot1).gameObject;
        Text text = invenSlot.transform.Find("Quantity").GetComponent<Text>();
        PortionItem portionItem = (PortionItem)ItemSlotController.Slots[0].item;
        text.text = portionItem.Amount.ToString();
        slot.ItemUse();
        //TODO
    }
    
    private void UIEvent(Define.GameEvent eventType, Component Sender, object param)
    {
        if(eventType == Define.GameEvent.playerEvents && param != null)
        {
            Player.PlayerActionKey key = (Player.PlayerActionKey)param;
            if (key != Player.PlayerActionKey.Death) return;
            GetObject((int)LoadAssetGameObjects.GameOver).SetActive(true);
            GameObject go = GetObject((int)LoadAssetGameObjects.GameOver).transform.Find("HomeButton").gameObject;
            go.SetActive(true);
            Button btn = go.GetOrAddComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => {
                Time.timeScale = 1;
               Managers.OnDestorys();
                Managers.Scene.ChangeScene(Define.SceneType.Lobby);

            });
        }
    }
}

