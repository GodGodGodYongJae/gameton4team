using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameScene : UI_Scene
{

    [SerializeField]
    GameObject btnMain;
    private Canvas canvas;
    public override bool Init()
    {
        canvas = this.GetComponent<Canvas>();
        if (base.Init() == false)
            return false;

        canvas.worldCamera = Camera.main;

        Managers.Events.AddListener(Define.GameEvent.playerEvents, UIEvent);

        return true;
    }

    //나중에 없애야 함.
    public void OnClickWeapon(int weaponNum)
    {
        Managers.Events.PostNotification(Define.GameEvent.ChangeWeapon, null, weaponNum);
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
                Managers.Scene.ChangeScene(Define.SceneType.TitleScene);
               Managers.OnDestorys();
            });
        }
    }
}

