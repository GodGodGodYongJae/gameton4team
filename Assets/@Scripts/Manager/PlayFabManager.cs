using Cysharp.Threading.Tasks;
using PlayFab;
using System;
using System.Collections;
using UnityEngine;

namespace Assets._Scripts.Manager
{
    public class PlayFabManager
    {
        private PlayFabAuthService authSerivce;
        public PlayFabAuthService AuthService => authSerivce;
        public PlayFabManager()
        {
            authSerivce = new PlayFabAuthService();
        }

        /// <summary>
        /// 가져올 재화
        /// </summary>
        /// <param name="Currency">StringData.cs BackEnd Region ref</param>
        /// <returns></returns>
        public async UniTask<int> GetCurrencyData(string Currency)
        {
            int rtn_currecy = 0;
            bool isResult = false;
            PlayFabClientAPI.GetUserInventory(new PlayFab.ClientModels.GetUserInventoryRequest(),
            (result) =>
            {
                result.VirtualCurrency.TryGetValue(Currency,out rtn_currecy);
                isResult = true;
            }, 
            (error)=> { ErrorLog(error); });
            await UniTask.WaitUntil(() => { return isResult == true; });
            return rtn_currecy;
        }
       
        /// <summary>
        /// 재화 수정, ID에는 재화 String, Num에는 음수 혹은 양수를 넣음.
        /// </summary>
        /// <param name="ID">StringData.cs BackEnd Region ref</param>
        /// <param name="num"></param>
        /// <param name="callback">call back function</param>
        public void SetCurrecy(string ID, int num, Action callback = null)
        {
            PlayFabClientAPI.ExecuteCloudScript(new PlayFab.ClientModels.ExecuteCloudScriptRequest()
            {
                FunctionName = "SetVirtualCurrency",
                FunctionParameter = new { Amount = num, type = ID },
                GeneratePlayStreamEvent = true
            }, cloudResult => {

                callback?.Invoke(); }, 
            error => { ErrorLog(error); }
            );
        }

        private void ErrorLog(PlayFabError error)
        {
            Debug.LogError(error.GenerateErrorReport());
        }
    }
}