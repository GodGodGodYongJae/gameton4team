using Assets._Scripts.Manager;
using Assets._Scripts.UI.LobbyScene;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Shop : MonoBehaviour
{
    public Button PortionBuyButton;
    public Button RespawnBuyButton;

    public UI_Lobby Lobby;
    private void Start()
    {
        PortionBuyButton.onClick.AddListener(() => { BuyItem("Item_Portion_Normal",100).Forget(); });
        RespawnBuyButton.onClick.AddListener(() => { BuyItem("Item_RespawnCard", 200).Forget(); });   
    }
    bool isComplated = true;
    private void BuyItem(string Id, int Price)
    {
        int coin =  Managers.PlayFab.GetCurrencyData(StringData.Coin);
        if(coin >= Price)
        {

            Managers.PlayFab.PurchaseItem(Id, Price, StringData.Coin, null);
            Lobby.LoadCurrecyData();
            //Managers.PlayFab.AddItemInventory(Id, 1, () =>
            //{
            //    Managers.PlayFab.SetCurrecy(StringData.Coin, -Price);
            //    //Lobby.LoadCurrecyData();
             
            //});
        }

    }
}
