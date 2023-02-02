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
        PortionBuyButton.onClick.AddListener(() => { BuyPortion("Item_Portion_Normal",100).Forget(); });
        RespawnBuyButton.onClick.AddListener(() => { BuyPortion("Item_RespawnCard", 200).Forget(); });   
    }
    bool isComplated = true;
    private async UniTaskVoid BuyPortion(string Id, int Price)
    {
        if (isComplated == false) return;
        int coin = await Managers.PlayFab.GetCurrencyData(StringData.Coin);
        
        if(coin >= Price)
        {
            isComplated = false;
            Managers.PlayFab.AddItemInventory(Id, 1, () =>
            {
                Managers.PlayFab.SetCurrecy(StringData.Coin, -Price);
                Lobby.LoadCurrecyData();
                isComplated = true;
            });
        }
        await UniTask.WaitUntil(() => { return isComplated == true; });
    }
}
