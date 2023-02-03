using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PlayFab;
using PlayFab.ClientModels;

using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

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
            InitCurrencyData();
        }

        // ==================== Client Currecy Data =================
        #region .
 
        private Dictionary<string, int> _userCurrecy = new Dictionary<string, int>();
       /// <summary>
       /// 최초 1회 딕셔너리에 게임내 모든 재화 key 등록.
       /// </summary>
        private void InitCurrencyData()
        {
            _userCurrecy.Add( StringData.Coin , 0 );
            _userCurrecy.Add( StringData.Energy, 0 );
            _userCurrecy.Add( StringData.DailyAdmob, 0 );
        }
        /// <summary>
        /// 서버에서 데이터를 동기화 해야할 때.
        /// </summary>
        public void SyncCurrencyDataFromServer()
        {
             GetCurrencyDataBackEnd(_userCurrecy).Forget();
        }

        /// <summary>
        /// 현재 재화 가져오기
        /// </summary>
        /// <param name="id">Key값</param>
        /// <returns></returns>
        public int GetCurrencyData(string id)
        {
            return _userCurrecy[id];
        }
        #endregion
        //=============================================================

        //================== Server Currecy Data ======================
        #region .
        private async UniTaskVoid GetCurrencyDataBackEnd(Dictionary<string, int> CurrencyList)
        {
            bool isResult = false;
            PlayFabClientAPI.GetUserInventory(new PlayFab.ClientModels.GetUserInventoryRequest(),
            (result) =>
            {
                List<string> Keys = new List<string>(CurrencyList.Keys);
                foreach (var item in Keys)
                {
                    CurrencyList[item] = result.VirtualCurrency[item];
                }
             
                isResult = true;
            },
            (error) => { ErrorLog(error); });
            await UniTask.WaitUntil(() => { return isResult == true; });
            
        }

        /// <summary>
        /// 아이템 구매 이거 써야 API콜 호출을 백번해도 괜찮음.
        /// </summary>
        /// <param name="itemId">아이템 ID</param>
        /// <param name="price">가격은 Catalog에 있는 가격으로 해야함</param>
        /// <param name="vc">재화 String</param>
        /// <param name="SuccesCallback">성공시 실행시킬 액션</param>
        public void PurchaseItem(string itemId,int price,string vc,Action SuccesCallback)
        {
            _userCurrecy[vc] -= price;
            PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest()
            {
                ItemId = itemId,
                Price = price,
                VirtualCurrency = vc
            }, success => { SuccesCallback.Invoke(); }, error => ErrorLog(error));
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

        #endregion
        //=============================================================

        //================ Client Inventory Data ==================
        #region .
        /// <summary>
        /// 해당 아이템 소지 갯수 가져오기.
        /// </summary>
        /// <param name="itemid"></param>
        /// <returns></returns>
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
            catch
            {
                return 0;
            }

            return rtn;
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

        /// <summary>
        /// 아이템 소모시키기
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="Ammount"></param>
        /// <param name="callback"></param>
        public void ConsumItem(string itemCode, int Ammount, Action callback = null)
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
        #endregion
        //=========================================================

        // ============ Server Inventory Data ===================
        #region .
        /// <summary>
        /// 아이템 추가 일반적이지 않음, 정말 간혹가다 사용할 때 분당 호출회수 10회 미만일 때 쓸 것.
        /// 만약, 아이템 구매를 해야한다면 PurchaseItem을 사용할 것.
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="ammount"></param>
        /// <param name="callback"></param>
        public void AddItemInventory(string itemCode, int ammount, Action callback = null)
        {
            List<string> items = new List<string>();
            for (int i = 0; i < ammount; i++)
            {
                items.Add(itemCode);
            }


            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "AddInventory",
                FunctionParameter = new { ItemCode = items.ToArray() },
                GeneratePlayStreamEvent = true
            }, clouedResult => {
                //TestDebugLog(clouedResult);
                GetUserInventory(callback);
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
            if (rtnInstance == "NULL") Debug.LogError("해당 아이템 찾을 수 없음" + itemid);
            return rtnInstance;

        }
        #endregion
        // ======================================================



     

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