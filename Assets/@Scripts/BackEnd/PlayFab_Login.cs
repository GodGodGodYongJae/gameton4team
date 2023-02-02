using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class PlayFab_Login : MonoBehaviour
{
    //Read Me : https://github.com/PlayFab/PlayFab-Samples/blob/master/VideoTutorialSamples/PlayFabAuthentication/Assets/PlayFabUI/Demo/Scripts/LoginWindowView.cs
    private bool isLogin = false;
    public bool Login => isLogin;
    public void Start()
    {
        LoginAsync().Forget();
    }

    private async UniTaskVoid LoginAsync()
    {
        Managers.PlayFab.AuthService.Authenticate(Authtypes.Silent);
        await UniTask.WaitUntil(() => { return Managers.PlayFab.AuthService.SessionTicket != null; });

        Managers.PlayFab.GetUserInventory(() => {
            Debug.Log("Login Success");
            isLogin = true;
        });


    }

}
