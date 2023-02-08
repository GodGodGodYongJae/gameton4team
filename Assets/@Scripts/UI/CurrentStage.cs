using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CurrentStage : MonoBehaviour
{
    Text text;
    public static int currentStage = 1;
    public static Action stage;
    private void Start()
    {
        text = GetComponent<Text>();
        stage = () => { GetStage(); };

    }

    public void GetStage()
    {
        currentStage += 1;
        text.text =  "Stage " + currentStage.ToString();
        StageBgmChange();
        StageBackGroundChange();
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

    public void StageBackGroundChange()
    {
        if(currentStage == 2) 
            BackGroundChangeController.stage();
        else if (currentStage == 3)
            BackGroundChangeController.stage();
        else if(currentStage == 5)
            BackGroundChangeController.stage();
        else if(currentStage == 7)
            BackGroundChangeController.stage();
    }
}
