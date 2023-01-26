using Assets._Scripts.Controller;
using Assets._Scripts.Game.Weapon;
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
        CharacterStatusSelect
    }

    enum WeaponSelectSlot 
    {
        Slot1,
        Slot2,
        Slot3,
    }
 

    [SerializeField]
    GameObject btnMain;

    public WeaponSlotController WeaponSlotController;
    private Canvas canvas;
    public override bool Init()
    {
        canvas = this.GetComponent<Canvas>();
        if (base.Init() == false)
            return false;

        canvas.worldCamera = Camera.main;

        #region Bind
        BindObject(typeof(LoadAssetGameObjects));
        BindObject(typeof(WeaponSelectSlot));
        #endregion

        Managers.Events.AddListener(Define.GameEvent.playerEvents, UIEvent);

        return true;
    }
    private const int SelectCardNum = 3;
    private void OpenWeaponSelectBox()
    {
        Time.timeScale = 0;
        GameObject selectObj =  GetObject((int)LoadAssetGameObjects.WeaponSelect).gameObject;
        selectObj.SetActive(true);
        for (int i = 0; i < SelectCardNum; i++)
        {
            int RandomSlotWeapon = Random.Range((int)Define.WeaponType.None + 1, (int)Define.WeaponType.End - 1);
            WeaponSlot slot = WeaponSlotController.GetWeapon((Define.WeaponType)RandomSlotWeapon);
            if ( slot == null)
            {
                
                break;
            }
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

