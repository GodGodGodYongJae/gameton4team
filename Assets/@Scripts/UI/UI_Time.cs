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

    public GameObject soundEffectIcon1;
    public GameObject soundEffectIcon2;
    public GameObject soundBgmIcon1;
    public GameObject soundBgmIcon2;


    public void StopSoundBgm()
    {
        Managers.Sound.PlaySFX("Touch");
        if (soundBgm == true)
        {
            Managers.Sound.PauseBGM();
            soundBgm = false;
            soundBgmIcon1.SetActive(false);
            soundBgmIcon2.SetActive(true);
        }
        else if(soundBgm == false)
        {
            Managers.Sound.ResumeBGM();
            soundBgm = true;
            soundBgmIcon1.SetActive(true);
            soundBgmIcon2.SetActive(false);
        }
    }

    public void StopSoundEffect()
    {
        Managers.Sound.PlaySFX("Touch");
        if (soundEffect == true)
        {
            Managers.Sound.IsSoundOn = false;
            soundEffect = false;
            soundEffectIcon1.SetActive(false);
            soundEffectIcon2.SetActive(true);
        }
        else if (soundEffect == false)
        {
            Managers.Sound.IsSoundOn = true;
            soundEffect = true;
            soundEffectIcon1.SetActive(true);
            soundEffectIcon2.SetActive(false);
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
