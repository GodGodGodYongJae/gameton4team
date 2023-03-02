using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundChangeController : MonoBehaviour
{
    [Header("BackgroundNum 0 -> 3")]
    public int backgroundNum = 0;
    public Sprite[] Layer_Sprites;
    private GameObject[] Layer_Object = new GameObject[5];
    private int max_backgroundNum = 5;

    void Start()
    {
        max_backgroundNum = 0;
        for (int i = 0; i < Layer_Object.Length; i++)
        {
            Layer_Object[i] = GameObject.Find("Layer_" + i);
        }

        Managers.Events.AddListener(Define.GameEvent.stageClear, ClearEvents);
        //ChangeSprite();
    }

    private void ClearEvents(Define.GameEvent eventType, Component Sender, object param)
    {
        if (Define.GameEvent.stageClear == eventType && Utils.EqualSender<GameScene>(Sender))
        {

            GameScene gameScene = (GameScene)Sender;
            int currentStage = gameScene.StageIndex;

            if (currentStage == 1)
                StageChange();
            else if (currentStage == 2)
                StageChange();
            else if (currentStage == 4)
                StageChange();
            else if (currentStage == 6)
                StageChange();

        }
    }

    void ChangeSprite()
    {
        Debug.Log(Layer_Sprites[backgroundNum * 5].name);
        Layer_Object[0].GetComponent<SpriteRenderer>().sprite = Layer_Sprites[backgroundNum * 5];

        for (int i = 1; i < Layer_Object.Length; i++)
        {
            Sprite changeSprite = Layer_Sprites[backgroundNum * 5 + i];
            Layer_Object[i].GetComponent<SpriteRenderer>().sprite = changeSprite;
            Layer_Object[i].transform.Find("1").GetComponent<SpriteRenderer>().sprite = changeSprite;
            Layer_Object[i].transform.Find("2").GetComponent<SpriteRenderer>().sprite = changeSprite;
        }
    }


    public void StageChange()
    {
        backgroundNum = backgroundNum + 1;
        ChangeSprite();
    }
}

