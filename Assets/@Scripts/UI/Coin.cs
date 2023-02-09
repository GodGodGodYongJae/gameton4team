using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class Coin : MonoBehaviour
{
    Player player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int CoinAmmount = UnityEngine.Random.Range(10, 21);
        CoinCount.coinAmount += CoinAmmount;
        Managers.PlayFab.SetCurrecy(StringData.Coin, CoinAmmount);
        CoinCount.getCoin();
        Managers.Sound.PlaySFX("coin");
        gameObject.SetActive(false);
    }

    private void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void OnEnable()
    {
        player = Managers.Object.GetSingularObjet(StringData.Player).GetComponent<Player>();
        this.transform.position = new Vector2(player.transform.position.x + 3, player.transform.position.y + 1f);
    }


}
