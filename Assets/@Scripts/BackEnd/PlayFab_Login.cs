using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayFab_Login : MonoBehaviour
{
    private bool isLogin = false;
    public bool Login => isLogin;
    public void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId)) PlayFabSettings.TitleId = "D42E0";

        var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }


    void OnLoginSuccess(LoginResult result)
    {
        isLogin = true;
    }


    void OnLoginFailure(PlayFabError error)
    {

    }
}
