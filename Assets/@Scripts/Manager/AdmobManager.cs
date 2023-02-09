using UnityEngine;
using UnityEngine.Events;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Assets._Scripts.Manager
{
    public class AdmobManager : MonoBehaviour
    {
        public UnityEvent OnAdLoadedEvent;
        public UnityEvent OnAdFailedToLoadEvent;
        public UnityEvent OnAdOpeningEvent;
        public UnityEvent OnAdFailedToShowEvent;
        public UnityEvent OnUserEarnedRewardEvent;
        public UnityEvent OnAdClosedEvent;

        private RewardedAd rewardedAd;

        public void Start()
        {
            MobileAds.SetiOSAppPauseOnBackground(true);
            List<String> deviceIds = new List<String>() { AdRequest.TestDeviceSimulator };
            ExceptionTestDevice(deviceIds);

            RequestConfiguration requestConfiguration =
            new RequestConfiguration.Builder()
            .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.Unspecified)
            .SetTestDeviceIds(deviceIds).build();
            MobileAds.SetRequestConfiguration(requestConfiguration);

            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize(HandleInitCompleteAction);

            // Listen to application foreground / background events.
            //AppStateEventNotifier.AppStateChanged += OnAppStateChanged;

        }

        private void HandleInitCompleteAction(InitializationStatus initstatus)
        {
            Debug.Log("Initialization complete.");

            // Callbacks from GoogleMobileAds are not guaranteed to be called on
            // the main thread.
            // In this example we use MobileAdsEventExecutor to schedule these calls on
            // the next Update() loop.
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {

            });
        }
        private void ExceptionTestDevice(List<String> deviceIds)
        {
#if UNITY_IPHONE
                        deviceIds.Add("96e23e80653bb28980d3f40beb58915c");
#elif UNITY_ANDROID
            deviceIds.Add("75EF8D155528C04DACBBA6F36F433035");
#endif
        }
        private AdRequest CreateAdRequest()
        {
            return new AdRequest.Builder()
                .AddKeyword("unity-admob-sample")
                .Build();
        }


        #region REWARD ADS
        public async UniTask<bool> RequestAndLoadRewardedAd(string id, Action callback = null)
        {
            bool isRegister = false;
            RewardedAd.Load(id, CreateAdRequest(),
                (RewardedAd ad, LoadAdError loadError) =>
                {
                    //Exception 처리
                    if (loadError != null)
                    {
                        Debug.LogError("Rewarded ad failed to load with error: " +
                                    loadError.GetMessage());
                        isRegister = true;
                        return;
                    }
                    else if (ad == null)
                    {
                        Debug.LogError("Rewarded ad failed to load.");
                        isRegister = true;
                        return;
                    }
                    rewardedAd = ad;

                    ad.OnAdFullScreenContentOpened += () =>
                    {
                        Debug.Log("Rewarded ad opening.");
                        OnAdOpeningEvent.Invoke();
                    };

                    ad.OnAdFullScreenContentFailed += (AdError error) =>
                    {
                        Debug.LogError("Rewarded ad failed to show with error: " +
                                   error.GetMessage());
                    };
                    isRegister = true;
                });
            await UniTask.WaitUntil(() => { return isRegister == true; });
            ShowRewardedAd(callback);
            return true;
        }

        private void ShowRewardedAd(Action callback = null)
        {
            if (rewardedAd != null)
            {
                rewardedAd.Show((Reward reward) =>
                {
                    Debug.Log("Rewarded ad granted a reward: " + reward.Amount);
                    Debug.Log("Rewarded ad granted a Type: " + reward.Type);
                    callback?.Invoke();
                });
            }
            else
            {
                Debug.Log("Rewarded ad is not ready yet.");
            }
        }

        #endregion
    }
}