using Assets._Scripts.Manager;
using Assets._Scripts.UI.LobbyScene;
using Cysharp.Threading.Tasks;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Shop : MonoBehaviour
{
    [SerializeField]
    GameObject Slot;
    [SerializeField]
    GameObject Content;

    [SerializeField]
    TextMeshProUGUI PortionQunatityText;
    [SerializeField]
    TextMeshProUGUI RespawnQunatityText;


    //List<ItemInstance> userInventory;



    private void Start()
    {
        LoadShopItems().Forget();
        //userInventory = Managers.PlayFab.userInventory;

    }
  

    private async UniTaskVoid LoadShopItems()
    {
        List<StoreItem> items = await Managers.PlayFab.GetStoreItems(StringData.PublicStore);
        foreach (var item in items)
        {
           ItemData data = await Managers.PlayFab.FindGetClientItem(item.ItemId);
           UI_ShopSlot shopSlot = Instantiate(Slot, Content.transform).GetComponent<UI_ShopSlot>();
            shopSlot.CreateInit(item.ItemId,item.VirtualCurrencyPrices["CO"].ToString(), data.Name,data.ItemAdmobCode ,data.IconSprite);
        }
    }

  
}
