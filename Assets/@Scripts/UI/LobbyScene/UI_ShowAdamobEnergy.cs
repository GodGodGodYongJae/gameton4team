using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ShowAdamobEnergy : MonoBehaviour
{
    public void OnClickAdmob()
    {
        OnReward().Forget();
    }

    bool isRewardLoad = false;
    public async UniTaskVoid OnReward()
    {
        if (isRewardLoad == true) return;
        isRewardLoad = true;
        await Managers.Admob.RequestAndLoadRewardedAd(StringData.AdMob.Energy, () =>
        {
            Managers.PlayFab.SetCurrecy(StringData.Energy, 5);
            isRewardLoad = false;
            Managers.Events.PostNotification(Define.GameEvent.LobbyCurrency, this);
        });
    }
}
