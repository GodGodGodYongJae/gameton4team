using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CoinCount : MonoBehaviour
{
    Text text;
    public static int coinAmount = 0;
    public static Action getCoin;


    private void Awake()
    {
        coinAmount = 0;
    }
    private void Start()
    {
        text = GetComponent<Text>();
        getCoin = () => { GetCoin(); };
    }

    public void GetCoin()
    {
        text.text = coinAmount.ToString();
    }

}
