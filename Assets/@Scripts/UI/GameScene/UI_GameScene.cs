using Assets._Scripts.Controller;
using Assets._Scripts.UI.GameScene;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        End
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
    public WeaponSlotController WeaponSlotController;
    private WeaponController WeaponController;
    public void InitGameScene(GameScene gameScene)
    {
        GameScene = gameScene;
        WeaponSlotController = gameScene.WeaponSlotController;
        WeaponController = gameScene.WeaponController;
      
    }


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
        BindButton(typeof(WeaponInventory));

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

        // for 문이 안먹어서 btn을 배열로 빼야할듯

        Button btn1 = GetButton(0).GetComponent<Button>();
        btn1.onClick.AddListener(() => { OnChangeWeapon(btn1,0).Forget(); });

        Button btn2 = GetButton(1).GetComponent<Button>();
        btn2.onClick.AddListener(() => { OnChangeWeapon(btn2,1).Forget(); });

        Button btn3 = GetButton(2).GetComponent<Button>();
        btn3.onClick.AddListener(() => { OnChangeWeapon(btn3,2).Forget(); });


        SelectWeaponUI selectWeaponUI = new SelectWeaponUI(this);
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

