using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Time : MonoBehaviour
{
    bool isTime = true;
    public void StopButton()
    {
        if(isTime == true)
        {
            Time.timeScale = 0;
            isTime = false;
        }
        else if(isTime == false)
        {
            Time.timeScale = 1;
            isTime = true;
        }
    }
}
