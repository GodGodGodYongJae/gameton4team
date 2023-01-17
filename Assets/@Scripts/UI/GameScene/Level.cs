using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    [SerializeField]
    private Slider expbar;


    public float level = 1;
    public float curExp = 0;
    public float maxExp = 10;

    private void Start()
    {
        expbar.value = (float)curExp;
    }

    public void GetExp()
    {
        curExp += 3;
        if(curExp >= maxExp)
        {
            curExp = curExp - maxExp;
            level++;
            maxExp = maxExp * 2;
        }
        SETExp();
    }

    public void SETExp()
    {
        expbar.value = (float)curExp / (float)maxExp;
    }
}

