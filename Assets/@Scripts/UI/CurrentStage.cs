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
    }

}
