using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Time : MonoBehaviour
{
    bool isTime = true;
    bool soundBgm = true;
    bool soundEffect = true;
    public GameObject settingUI;


    public void StopButton()
    {
        Managers.Sound.PlaySFX("Touch");
        if(isTime == true)
        {
            Time.timeScale = 0;
            isTime = false;
            settingUI.SetActive(true);
        }
    }

    public void Play()
    {
        Managers.Sound.PlaySFX("Touch");
        if (isTime == false)
        {
            Time.timeScale = 1;
            isTime = true;
            settingUI.SetActive(false);
        }
    }
    public void StopSoundBgm()
    {
        Managers.Sound.PlaySFX("Touch");
        if (soundBgm == true)
        {
            Managers.Sound.PauseBGM();
            soundBgm = false;
        }
        else if(soundBgm == false)
        {
            Managers.Sound.ResumeBGM();
            soundBgm = true;
        }
    }

    public void StopSoundEffect()
    {
        Managers.Sound.PlaySFX("Touch");
        if (soundEffect == true)
        {
            Managers.Sound.IsSoundOn = false;
            soundEffect = false;
        }
        else if (soundEffect == false)
        {
            Managers.Sound.IsSoundOn = true;
            soundEffect = true;
        }
    }

    public void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
