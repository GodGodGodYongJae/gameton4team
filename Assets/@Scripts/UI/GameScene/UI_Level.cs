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
        Managers.Events.AddListener(Define.GameEvent.playerExpChange, SetExp);
    }

    private void SetExp(Define.GameEvent eventType, Component Sender, object param)
    {

        if(eventType == Define.GameEvent.playerExpChange )
        {
            Player p = (Player)Sender;
            PlayerData data = p.PlayerData;
            string expText = "";
            float currentExp = (float)param;
            if(data.ExperiencePoint == int.MaxValue)
            {
                expText = "MAX!";
            }
            else
            {
                expbar.value = currentExp / (float)data.ExperiencePoint;
                expText = currentExp + " / " + data.ExperiencePoint;
            }
            Exp.text = expText;
            Lv.text = "LV." + data.Level;
            
        }
    }

}

