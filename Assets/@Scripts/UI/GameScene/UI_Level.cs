using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class UI_Level : MonoBehaviour 
{

    [SerializeField]
    private Slider expbar;
    [SerializeField]
    private Text Lv;
    [SerializeField]
    private Text Exp;


    public void Start()
    {
        Debug.Log("EventRegis");
        Managers.Events.AddListener(Define.GameEvent.playerExpChange, SETExp);
    }

    private void SETExp(Define.GameEvent eventType, Component Sender, object param)
    {

        if(eventType == Define.GameEvent.playerExpChange && Utils.EqualSender<Player>(Sender))
        {
            Player p = (Player)Sender;
            PlayerData data = p.PlayerData;
            string expText = "";
            if(data.ExperiencePoint == int.MaxValue)
            {
                expText = "MAX!";
            }
            else
            {
                expbar.value = (float)p.CurrentExp / (float)data.ExperiencePoint;
                expText = p.CurrentExp + " / " + data.ExperiencePoint;
            }
            Exp.text = expText;
            Lv.text = "LV." + data.Level;
            
        }
    }

}

