using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PlayFab;
using PlayFab.ClientModels;

using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using PlayFab.PfEditor.Json;
#endif

namespace Assets._Scripts.Manager
{
    public class PlayFabManager
    {
        private PlayFabAuthService authSerivce;
        public List<ItemInstance> userInventory;
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

        /// <summary>
        /// 아이템 추가
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="ammount"></param>
        /// <param name="callback"></param>
        public void AddItemInventory(string itemCode, int ammount,Action callback = null)
        {
            List<string> items = new List<string>();
            for (int i = 0; i < ammount; i++)
            {
                items.Add(itemCode);
            }
           

            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "AddInventory",
                FunctionParameter = new {ItemCode = items.ToArray() },
                GeneratePlayStreamEvent = true
            }, clouedResult => {
                //TestDebugLog(clouedResult);
                GetUserInventory(callback);
            }, error => ErrorLog(error));
        }
       
        /// <summary>
        /// 현재 인벤토리 가져오기.
        /// </summary>
        /// <param name="callback"></param>
        public void GetUserInventory(Action callback = null)
        {
            
            PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest { },
                result =>
                {
                    userInventory = result.Inventory;
                    //foreach(var item in userInventory)
                    //{
                    //    Debug.Log(item.ItemClass + "," + item.RemainingUses);
                    //}
                    callback?.Invoke();
             

                }, error => ErrorLog(error));
        }

        private string FindItemInstanceItem(string itemid)
        {
            string rtnInstance = "NULL";
    
            foreach (var item in userInventory)
            {
                if (item.ItemId == itemid)
                { 
                    rtnInstance = item.ItemInstanceId;
                    break;
                }
                   
            }
            if (rtnInstance == "NULL") Debug.LogError("해당 아이템 찾을 수 없음"+itemid);
          return rtnInstance;

        }

        public int FindItemQuantity(string itemid)
        {
            int rtn = 0;
            try
            {
                foreach (var item in userInventory)
                {
                    if (item.ItemId == itemid)
                    {
                        rtn = (int)item.RemainingUses;
                        break;
                    }

                }
            }
            catch {
                return 0;
            }
        
            return rtn;
        }
        /// <summary>
        /// 아이템 소모시키기
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="Ammount"></param>
        /// <param name="callback"></param>
        public void ConsumItem(string itemCode,int Ammount,Action callback = null)
        {
            string ItemInstanceId = FindItemInstanceItem(itemCode);

            PlayFabClientAPI.ConsumeItem(new ConsumeItemRequest
            {
                ItemInstanceId = ItemInstanceId,
                ConsumeCount = Ammount,
            
            },
                Success =>
                {
                    GetUserInventory(callback);
                }, error => { ErrorLog(error); });

        }

        private void ErrorLog(PlayFabError error)
        {
            Debug.LogError(error.GenerateErrorReport());
        }

        #region SampleCode
            #if UNITY_EDITOR
        private static void TestDebugLog(ExecuteCloudScriptResult result)
        {
            // CloudScript (Legacy) returns arbitrary results, so you have to evaluate them one step and one parameter at a time
            Debug.Log(JsonWrapper.SerializeObject(result.FunctionResult));
            JObject jsonResult = JObject.Parse(result.FunctionResult.ToString());

            foreach (var item in jsonResult)
            {
                Debug.Log(item);
            }
            //object messageValue;
            //jsonResult.TryGetValue("data", out messageValue); // note how "messageValue" directly corresponds to the JSON values set in CloudScript (Legacy)
            //Debug.Log((string)messageValue);

            //object messageValue2;
            //jsonResult.TryGetValue("id", out messageValue2); // note how "messageValue" directly corresponds to the JSON values set in CloudScript (Legacy)
            //Debug.Log((string)messageValue2);

            //object messageValue3;
            //jsonResult.TryGetValue("test", out messageValue3); // note how "messageValue" directly corresponds to the JSON values set in CloudScript (Legacy)
            //Debug.Log((string)messageValue3);
        }

            #endif
        #endregion
    }
}