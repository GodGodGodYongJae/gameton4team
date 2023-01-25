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

        Social.localUser.Authenticate((success) =>
        {
            if (success) {  PlayFabLogin();  }
            else {  }

        });
        //var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true };
        //PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    public void PlayFabLogin()
    {
        Debug.Log("d");
        var request = new LoginWithEmailAddressRequest { Email = Social.localUser.id + "@rand.com", Password = Social.localUser.id + "@rand.com" };
        PlayFabClientAPI.LoginWithEmailAddress(request, (result) => { OnLoginSuccess(result); }, (error) => PlayFabRegister());
    }

    public void PlayFabRegister()
    {
        Debug.Log("d2");
        var request = new RegisterPlayFabUserRequest { Email = Social.localUser.id + "@rand.com", Password = Social.localUser.id + "@rand.com", Username = Social.localUser.userName };
        PlayFabClientAPI.RegisterPlayFabUser(request, (result) => { PlayFabLogin(); }, (error) => { Debug.Log(Social.localUser.id);  Debug.LogError(error.GenerateErrorReport());  });
    }
    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("로그인완료!");
        isLogin = true;
    }


    void OnLoginFailure(PlayFabError error)
    {

    }
}
