using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab.MultiplayerModels;
using System.Diagnostics;
using Assets._Scripts.UI.LobbyScene;
using System;
using Cysharp.Threading.Tasks;

public class UI_ShopSlot : MonoBehaviour
{
    public TextMeshProUGUI BuyButtonText;
    public TextMeshProUGUI haveQuantityText;
    public Text TitleText;
    public Image Image;
    private int price;
    private string itemId;

    [SerializeField]
    Button rewardButton;
    [SerializeField]
    bool isRewardButton;
    [SerializeField]
    int rewardQuantity;

    private string admobCode;
    public void CreateInit(string itemId,string price, string title,string AdmobCode ,Sprite sprite)
    {
        this.itemId = itemId;
        BuyButtonText.text = price;
        this.price = int.Parse(price);
        TitleText.text = title;
        Image.sprite = sprite;
        admobCode = AdmobCode;
        haveQuantityText.text = Managers.PlayFab.FindItemQuantity(itemId).ToString();
        if(isRewardButton == true)
        {
            rewardButton.onClick.AddListener(() => { OnReward().Forget(); });
        }
    }

    bool isRewardLoad = false;
    public async UniTaskVoid OnReward()
    {
        if (isRewardLoad == true) return;
        isRewardLoad = true;
        await Managers.Admob.RequestAndLoadRewardedAd(admobCode, () => {
             Managers.PlayFab.AddItemInventory(itemId, rewardQuantity ,() => {
                 haveQuantityText.text = ( 1 + Managers.PlayFab.FindItemQuantity(itemId)).ToString();
                 isRewardLoad = false;
             });
        });
        //await UniTask.WaitUntil(() => isRewardLoad == false);

    }

    public void OnBuyButton()
    {
        int coin = Managers.PlayFab.GetCurrencyData(StringData.Coin);
        if (coin >= price)
        {
            Managers.PlayFab.PurchaseItem(itemId, price, StringData.Coin, () => {
                haveQuantityText.text = Managers.PlayFab.FindItemQuantity(itemId).ToString();
            });
            Managers.Events.PostNotification(Define.GameEvent.LobbyCurrency, this);

            
        }
    }

}
