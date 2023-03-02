using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

    string MaxRewardQuantity;

    [SerializeField]
    Text reawrdQuantityText;

    private string admobCode;
    private string rewardId;
    private string PlacementId;
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
            Managers.PlayFab.GetAdPlacements(admobCode, (success) => {
                rewardQuantity = (int)success.AdPlacements[0].PlacementViewsRemaining;
                MaxRewardQuantity = success.AdPlacements[0].RewardDescription;
                rewardId = success.AdPlacements[0].RewardId;
                PlacementId = success.AdPlacements[0].PlacementId;
                ChangeRewardQuantityText();
                rewardButton.onClick.AddListener(() => { OnReward().Forget(); });
            });
        }
    }

    bool isRewardLoad = false;
    public async UniTaskVoid OnReward()
    {
        if (rewardQuantity <= 0) return;

        if (isRewardLoad == true) return;
        isRewardLoad = true;
        await Managers.Admob.RequestAndLoadRewardedAd(StringData.AdMob.ItemReward, () => {

            Managers.PlayFab.RewardAdActivity(PlacementId, rewardId, (success) => {
                rewardQuantity = (int)success.PlacementViewsRemaining;
                ChangeRewardQuantityText();
                Managers.PlayFab.GetUserInventory(() =>
                {
                    haveQuantityText.text = Managers.PlayFab.FindItemQuantity(itemId).ToString();
                });
                isRewardLoad = false;
            });
             //Managers.PlayFab.AddItemInventory(itemId, rewardQuantity ,() => {
             //    haveQuantityText.text = ( 1 + Managers.PlayFab.FindItemQuantity(itemId)).ToString();
             //    isRewardLoad = false;
             //});
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


    private void ChangeRewardQuantityText()
    {
        reawrdQuantityText.text ="±¤°íº¸±â"+ System.Environment.NewLine + rewardQuantity + " / " + MaxRewardQuantity;
    }
}
