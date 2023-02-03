using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System;

public class PlayFab_Login : MonoBehaviour
{
    //Read Me : https://github.com/PlayFab/PlayFab-Samples/blob/master/VideoTutorialSamples/PlayFabAuthentication/Assets/PlayFabUI/Demo/Scripts/LoginWindowView.cs
    private bool isLogin = false;
    public bool Login => isLogin;


    public void OnLogin(Action callback = null)
    {
        LoginAsync(callback).Forget();
    }

    private async UniTaskVoid LoginAsync(Action callback = null)
    {
        Managers.PlayFab.AuthService.Authenticate(Authtypes.Silent);
        await UniTask.WaitUntil(() => { return Managers.PlayFab.AuthService.SessionTicket != null; });

        //최초 1회, 현재 서버에 있는 CurrencyData 받아옴.
        Managers.PlayFab.SyncCurrencyDataFromServer();

        Managers.PlayFab.GetUserInventory(() => {
            Debug.Log("Login Success");
            isLogin = true;
            callback?.Invoke();
        });


    }


}
