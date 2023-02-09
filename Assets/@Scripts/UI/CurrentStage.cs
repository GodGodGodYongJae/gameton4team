using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CurrentStage : MonoBehaviour
{
    Text text;
    public int currentStage = 1;
    private void Start()
    {
        text = GetComponent<Text>();
        Managers.Events.AddListener(Define.GameEvent.stageClear, StageClear);

    }

    private void StageClear(Define.GameEvent eventType, Component Sender, object param)
    {
        if (Define.GameEvent.stageClear == eventType && Utils.EqualSender<GameScene>(Sender))
        {
            GetStage();
        }
    }

    public void GetStage()
    {
        currentStage += 1;
        text.text =  "Stage " + currentStage.ToString();
        StageBgmChange();
    }
    
    public void StageBgmChange()
    {
        if(currentStage == 4)
        {
            Managers.Sound.StopBGM();
            Managers.Sound.PlayBGM("InGame2");
        }

        if (currentStage == 7)
        {
            
            Managers.Sound.StopBGM();
            Managers.Sound.PlayBGM("InGame3");
        }
    }

    
}
