using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundChangeController : MonoBehaviour
{
    [Header("BackgroundNum 0 -> 3")]
    public static int backgroundNum = 0;
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
                Stage1();
            else if (currentStage == 2)
                Stage1();
            else if (currentStage == 4)
                Stage1();
            else if (currentStage == 6)
                Stage1();

        }
    }

    void ChangeSprite()
    {
        Debug.Log("test1");
        Debug.Log(Layer_Sprites[backgroundNum * 5].name);
        Layer_Object[0].GetComponent<SpriteRenderer>().sprite = Layer_Sprites[backgroundNum * 5];

        Debug.Log("test2"+ "," + backgroundNum * 5);
        for (int i = 1; i < Layer_Object.Length; i++)
        {
            Debug.Log("test3"+","+ Layer_Object.Length);
            Sprite changeSprite = Layer_Sprites[backgroundNum * 5 + i];

            Debug.Log("test4" + "," + Layer_Object.Length);
            Layer_Object[i].GetComponent<SpriteRenderer>().sprite = changeSprite;

            Debug.Log("test5" + "," + Layer_Object.Length);
            Debug.Log("test5-1" + "," + Layer_Object[i].transform.Find("1").name);
            Layer_Object[i].transform.Find("1").GetComponent<SpriteRenderer>().sprite = changeSprite;

            Debug.Log("test6" + "," + Layer_Object.Length);
            Layer_Object[i].transform.Find("2").GetComponent<SpriteRenderer>().sprite = changeSprite;
        }
    }

    public void Stage1()
    {
        backgroundNum = backgroundNum + 1;
        ChangeSprite();
    }
}

