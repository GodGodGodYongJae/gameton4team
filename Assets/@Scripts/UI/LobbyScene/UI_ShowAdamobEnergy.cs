using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UI_ShowAdamobEnergy : MonoBehaviour
{
    public void OnClickAdmob()
    {
        OnReward().Forget();
    }

    bool isRewardLoad = false;
    private string rewardId;
    private int remainig;
    private const int maxRemanig = 3;
    private const string AppId = "EnergyReward";
    private string PlacementId;
    public TextMeshProUGUI remaningText;
    private void Awake()
    {
        Managers.PlayFab.GetAdPlacements(AppId, (success) =>
        {
            rewardId = success.AdPlacements[0].RewardId;
            remainig = (int)success.AdPlacements[0].PlacementViewsRemaining;
            PlacementId = success.AdPlacements[0].PlacementId;
            ChangeRemainingText();

        });
    }
    public async UniTaskVoid OnReward()
    {
        if (isRewardLoad == true || remainig <= 0 ) return;
        isRewardLoad = true;
        await Managers.Admob.RequestAndLoadRewardedAd(StringData.AdMob.Energy, () =>
        {
            Managers.PlayFab.RewardAdActivity(PlacementId, rewardId, (success) => {
                Managers.PlayFab.SetClientCurrencyData(StringData.Energy, Managers.PlayFab.GetCurrencyData(StringData.Energy) + success.RewardResults.GrantedVirtualCurrencies[StringData.Energy]);
                isRewardLoad = false;
                ChangeRemainingText();
                Managers.Events.PostNotification(Define.GameEvent.LobbyCurrency, this);
            });

        });
    }
    private void ChangeRemainingText()
    {
        remaningText.text = remainig + " / " + maxRemanig;
    }
}
