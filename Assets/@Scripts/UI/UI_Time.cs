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
            Time.timeScale = 0;
            isTime = false;
            Managers.Sound.PauseBGM();
        }
        else if(isTime == false)
        {
            Time.timeScale = 1;
            isTime = true;
            Managers.Sound.ResumeBGM();
        }
    }
}
