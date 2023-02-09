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
    public static Action stagechange;

    void Start()
    {
        max_backgroundNum = 0;
        for (int i = 0; i < Layer_Object.Length; i++)
        {
            Layer_Object[i] = GameObject.Find("Layer_" + i);
        }
        ChangeSprite();
    }

    private void Awake()
    {
        stagechange = () => { Stage1(); };
    }

    void ChangeSprite()
    {
        Layer_Object[0].GetComponent<SpriteRenderer>().sprite = Layer_Sprites[backgroundNum * 5];
        for (int i = 1; i < Layer_Object.Length; i++)
        {
            Sprite changeSprite = Layer_Sprites[backgroundNum * 5 + i];
            Layer_Object[i].GetComponent<SpriteRenderer>().sprite = changeSprite;
            Layer_Object[i].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = changeSprite;
            Layer_Object[i].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = changeSprite;
        }
    }

    public void Stage1()
    {
        backgroundNum = backgroundNum + 1;
        ChangeSprite();
    }
}

