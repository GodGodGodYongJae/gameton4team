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

    public GameObject SoundEffect1;
    public GameObject SoundEffect2;
    public GameObject SoundBgm1;
    public GameObject SoundBgm2;


    public void StopSoundBgm()
    {
        Managers.Sound.PlaySFX("Touch");
        if (soundBgm == true)
        {
            Managers.Sound.PauseBGM();
            soundBgm = false;
            SoundBgm1.SetActive(false);
            SoundBgm2.SetActive(true);
        }
        else if(soundBgm == false)
        {
            Managers.Sound.ResumeBGM();
            soundBgm = true;
            SoundBgm1.SetActive(true);
            SoundBgm2.SetActive(false);
        }
    }

    public void StopSoundEffect()
    {
        Managers.Sound.PlaySFX("Touch");
        if (soundEffect == true)
        {
            Managers.Sound.IsSoundOn = false;
            soundEffect = false;
            SoundEffect1.SetActive(false);
            SoundEffect2.SetActive(true);
        }
        else if (soundEffect == false)
        {
            Managers.Sound.IsSoundOn = true;
            soundEffect = true;
            SoundEffect1.SetActive(true);
            SoundEffect2.SetActive(false);
        }
    }

    public void Lobby()
    {
        Time.timeScale = 1;
        Managers.OnDestorys();
        Managers.Scene.ChangeScene(Define.SceneType.Lobby);
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
