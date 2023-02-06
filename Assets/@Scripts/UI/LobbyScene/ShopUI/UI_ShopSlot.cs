using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab.MultiplayerModels;
using System.Diagnostics;
using Assets._Scripts.UI.LobbyScene;

public class UI_ShopSlot : MonoBehaviour
{
    public TextMeshProUGUI BuyButtonText;
    public Text TitleText;
    public Image Image;
    private int price;
    private string itemId;

    public void CreateInit(string itemId,string price, string title, Sprite sprite)
    {
        this.itemId = itemId;
        BuyButtonText.text = price;
        this.price = int.Parse(price);
        TitleText.text = title;
        Image.sprite = sprite;
      
    }

    public void OnBuyButton()
    {
        int coin = Managers.PlayFab.GetCurrencyData(StringData.Coin);
        if (coin >= price)
        {

            Managers.PlayFab.PurchaseItem(itemId, price, StringData.Coin, null);
            Managers.Events.PostNotification(Define.GameEvent.LobbyCurrency, this);
        }
    }

}
