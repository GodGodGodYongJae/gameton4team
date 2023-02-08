using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Time : MonoBehaviour
{
    bool isTime = true;
    public void StopButton()
    {
        Managers.Sound.PlaySFX("Touch");
        if(isTime == true)
        {
           Managers.Sound.PauseBGM();
            Time.timeScale = 0;
            isTime = false;
        }
        else if(isTime == false)
        {
            Managers.Sound.ResumeBGM();
            Time.timeScale = 1;
            isTime = true;
        }
    }
}
