using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerHP : MonoBehaviour
{
    Slider slider;

    float maxVal, curVal;
    void Start()
    {
        slider = GetComponent<Slider>();
        Managers.Events.AddListener(Define.GameEvent.playerHealthChange, ChangePlayerHP);
    }

    private void ChangePlayerHP(Define.GameEvent eventType, Component Sender, object param)
    {
        if (eventType == Define.GameEvent.playerHealthChange)
        {
            if (Utils.EqualSender<Player>(Sender))
            {
                Define.PlayerEvent_HPData data = (Define.PlayerEvent_HPData)param;
                maxVal = data.maxHp;
                curVal = data.curHp;
                float fHp = (float)curVal / (float)maxVal;
                ChangeSliderValue(fHp);
            }

        }

    }
    void ChangeSliderValue(float curHp) => slider.value = curHp;
}
