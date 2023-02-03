using Assets._Scripts.Manager;
using Assets._Scripts.UI.LobbyScene;
using Cysharp.Threading.Tasks;
using PlayFab.ClientModels;
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
        //PortionBuyButton.onClick.AddListener(() => { BuyItem("Item_Portion_Normal",100); });
        //RespawnBuyButton.onClick.AddListener(() => { BuyItem("Item_RespawnCard", 200); });   
        SetShopItems().Forget();
    }

    //초기 상점 아이템 등록
    //나중에 버튼도 등록해줘야 할듯 여기서 만들어서. 우선은 하드코딩 .. 
    private async UniTaskVoid SetShopItems()
    {
        List<StoreItem> items = await Managers.PlayFab.GetStoreItems(StringData.PublicStore);
        foreach (StoreItem item in items)
        {
            Debug.Log(item.ItemId);
            Debug.Log(item.VirtualCurrencyPrices["CO"].ToString());

            if(item.ItemId == "Item_Portion_Normal")
            {
                PortionBuyButton.onClick.AddListener(() => { BuyItem(item.ItemId, (int)item.VirtualCurrencyPrices["CO"]); });
            }
            else if(item.ItemId == "Item_RespawnCard")
            {
                RespawnBuyButton.onClick.AddListener(() => { BuyItem(item.ItemId, (int)item.VirtualCurrencyPrices["CO"]); });
            }
        }
 
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
