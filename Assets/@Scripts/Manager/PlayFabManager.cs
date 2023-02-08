using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PlayFab;
using PlayFab.ClientModels;

using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using Rito.InventorySystem;
using UniRx;
using static Define;

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
            InitPlayerDatas();
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
            _userCurrecy.Add(StringData.DailyReward, 0);
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
        public void PurchaseItem(string itemId,int price,string vc,Action SuccesCallback, string StoreId = null)
        {
   
            _userCurrecy[vc] -= price;
            if (StoreId == null) StoreId = StringData.PublicStore;
            PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest()
            {
                ItemId = itemId,
                Price = price,
                VirtualCurrency = vc,
                StoreId = StoreId
            }, success => {
                for (int i = 0; i < success.Items.Count; i++)
                {
                    int haveQuantity = FindItemQuantity(success.Items[i].ItemId);
                    if(haveQuantity == 0)
                    {
                        userInventory.Add(success.Items[i]);
                    }
                    else
                    {
                        int idx = FindItemIdx(success.Items[i].ItemId);
                        userInventory[idx].RemainingUses = success.Items[i].RemainingUses;
                    }
                    //if (userInventory.)
                    //{
                    //    int idx = userInventory.IndexOf(success.Items[i]);
                    //    userInventory[idx].RemainingUses += success.Items[i].RemainingUses;
                    //}
                    //else
                    //{
                    //    userInventory.Add(success.Items[i]);
                    //}
                }
                SuccesCallback?.Invoke(); }, error => ErrorLog(error));
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

        public int FindItemIdx(string itemid)
        {
            int rtn = -1;
            for (int i = 0; i < userInventory.Count; i++)
            {
                if (userInventory[i].ItemId == itemid)
                {
                    rtn = i;
                    break;
                }
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
                    //ClienetUserInventorySet()
                    callback?.Invoke();
                }, error => { ErrorLog(error); });

        }

        #endregion
        //=========================================================

        // ============ Server Inventory Data ===================
        #region .

        /// <summary>
        /// 상점의 아이템 불러오기
        /// </summary>
        /// <param name="storeId">상점 ID </param>
        /// <returns></returns>
       public async UniTask<List<StoreItem>> GetStoreItems(string storeId)
        {
            List<StoreItem> storeItems = new List<StoreItem>();
            bool isComplated = false;
            PlayFabClientAPI.GetStoreItems(new GetStoreItemsRequest() {
           StoreId = storeId
           }, Success => {
               storeItems = Success.Store;
               isComplated = true;
           },error=>ErrorLog(error));
            await UniTask.WaitUntil(() => { return isComplated == true; });
            return storeItems;
        }
        
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
                callback?.Invoke();
            }, error => ErrorLog(error));
        }


        /// <summary>
        /// 아이템 여러개 추가할때 한번에.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void AddItemInventory2(Dictionary<ItemData,int> items, Action callback = null)
        {
            List<string> SendItemList = new List<string>();
            foreach (var item in items)
            {
                for (int i = 0; i < item.Value; i++)
                {
                    ItemData data = item.Key;
                    SendItemList.Add(data.Key);
                }
            }


            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "AddInventory",
                FunctionParameter = new { ItemCode = SendItemList.ToArray() },
                GeneratePlayStreamEvent = true
            }, clouedResult =>
            {
                //TestDebugLog(clouedResult);
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
            if (rtnInstance == "NULL") Debug.LogError("해당 아이템 찾을 수 없음" + itemid);
            return rtnInstance;

        }
        #endregion
        // ======================================================

        //*************** Get Player Data **************************
        #region.

        //------- Public Methods --------------
         #region.
        /// <summary>
        /// 현재 PlayerData Key값
        /// </summary>
        public enum PlayerData { maxScore,dailyReward,end }
        /// <summary>
        /// Client에 저장해둔 PlayerData정보들
        /// </summary>
        public Dictionary<PlayerData,string> PlayerDatas = new Dictionary<PlayerData, string>();
        /// <summary>
        /// 플레이어 정보를 Server에서 받아온다.
        /// </summary>
        public void GetServerUserData(Action callback = null)
        {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest() { 
            PlayFabId = PlayFabAuthService.PlayFabId,
            Keys = null
            }, result => {
                PlayerData pdata;
                //Result 정보가 현재 Client Data 갯수와 다르다면 없는 Data가 존재하므로 서버에 추가해줘야 한다.
                if (result.Data.Count != (int)PlayerData.end)
                {
                    for (int i = 0; i < (int)PlayerData.end; i++)
                    {
                        pdata = (PlayerData)i;
                        UserDataRecord ResultData = null;
                        if (result.Data.TryGetValue(pdata.ToString(), out ResultData))
                        {
                            //해당 데이터가 있으면,클라이언트에 등록해준다.
                            SetPlayerData((PlayerData)i, ResultData.Value);
                        }
                        else
                        {
                            SetData.Enqueue(new Tuple<PlayerData, string>(pdata, "0"));
                        }
                    }
                    //해당 분별이 다 끝나면, Deqeue를 사용해준다. SetServerPlayerData 메서드를 사용하지 않고 직접적으로 SetData에 박는 이유는
                    // SetServer Player Data는 Deqeue를 내부적으로 호출하기 때문에 불필요한 중복호출을 막기 위해서이다.
                    DataFromServer();
                }
                else
                {
                    //데이터가 모두 있을 경우 그냥 Client에 모두 박아주면 된다.
                    for (int i = 0; i < (int)PlayerData.end; i++)
                    {
                        pdata = (PlayerData)i;
                        SetPlayerData(pdata, result.Data[pdata.ToString()].Value);
                    }
                    /// 아마 해당 로직은 초기, 벨류값을 정해주는 것 같다. 
                    //if (events != null)
                    //{
                    //    bool isCreate = (result.Data[Stat.Name.ToString()].Value != "0");
                    //    events.Invoke(isCreate);
                    //}
                }
            callback?.Invoke();
            }, error => ErrorLog(error));;
        }

        /// <summary>
        /// Client에 저장된 플레이어의 데이터를 받아온다. string으로 받아오기 때문에 차후 형변환 필요.
        /// </summary>
        public string GetUserData(PlayerData Getdata)
        {
            return PlayerDatas[Getdata];
        }

        Queue<Tuple<PlayerData, string>> SetData = new Queue<Tuple<PlayerData, string>>();
        /// <summary>
        /// 우선 서버에 데이터를 바꿔줄려면  먼저 Server에 담아준다. 한 번에 전송할 수 있는 양이 10개 이하로 조정되어있기 때문에. 큐에만 담아준다. 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="value"></param>
        public void SetServerPlayerData(PlayerData data, string value)
        {
            Tuple<PlayerData, string> enqeueData = new Tuple<PlayerData, string>(data, value);
            SetData.Enqueue(enqeueData);
            DataFromServer();
        }


        #endregion
        //----------------------------------
        //-------- Private Methods -------------
        #region.
        private void InitPlayerDatas()
        {
            for (int i = 0; i < (int)PlayerData.end; i++)
            {
                PlayerDatas.Add((PlayerData)i, null);
            }
        }

   
        Dictionary<string, string> QueueDataDictionary = new Dictionary<string, string>();

        private void DataFromServer()
        {
            var item = SetData.Dequeue();
            //만약에 전송전 해당 데이터가 있다면.
            if(QueueDataDictionary.ContainsKey(item.Item1.ToString()))
            {
                //우선 정수 = Float으로 받아서 합쳐주고, Try 해서 안되면 String으로 받아서 덮어씌어준다.
                try
                {
                    float sumData = float.Parse(QueueDataDictionary[item.Item1.ToString()]) + float.Parse(item.Item2);
                    QueueDataDictionary[item.Item1.ToString()] = sumData.ToString();
                }
                catch
                {
                    QueueDataDictionary[item.Item1.ToString()] = item.Item2;
                }
                
            }
            else
            {
                QueueDataDictionary.Add(item.Item1.ToString(), item.Item2);
            }
            //큐가 한 번에 9개 이상이 되면, DataFromServer를 돌면서 중복되는 값들은 빼준다.
            if(QueueDataDictionary.Count < 9)
            {
                //해당 큐가 마지막이라면 서버로 보내준다. 
                if (SetData.Count == 0) SetUserData();
                else DataFromServer(); // 아니라면, 다시 재귀를 돌아서 Queue가 모두 돌기 까지 기다린다. 겹치는게 있으면 더해주거나, String인 경우 덮어씌워줌.

            }
            else
            {
                //큐가 9개 이하라면 그냥 바로 서버로 보내준뒤, Queue를 비워준다.
                SetUserData();
                QueueDataDictionary.Clear();
            }

        }

        /// <summary>
        /// 최종적으로 해당 메소드에서 서버에 데이터를 등록해준다.
        /// </summary>
        private void SetUserData()
        {
            
            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
            {
                Data = QueueDataDictionary

            }, result => {
                GetServerUserData(); //
                if (SetData.Count != 0) DataFromServer();
            },
            error => { ErrorLog(error); });
        }
    
        /// <summary>
        /// Client쪽에 데이터를 바꿔준다.
        /// </summary>
        private void SetPlayerData(PlayerData data, string value)
        {
            PlayerDatas[data] = value;
        }
        #endregion
    //--------------------------------------

    #endregion
        //**********************************************************

        /// <summary>
        /// 클라이언트에 해당 아이템이 있는지 체크한후 Scripatble Object ItemData 불러옴
        /// </summary>
        /// <param name="itemid"></param>
        /// <returns></returns>
        public async UniTask<ItemData> FindGetClientItem(string itemid)
        {
            object rtnData = new object();
            bool isSuccess = false;
            Managers.Resource.LoadAsync<ScriptableObject>(itemid,
                success =>
                {
                    rtnData = (ItemData)success;
                    isSuccess = true;
                });
            await UniTask.WaitUntil(() => { return isSuccess == true; });
            return (ItemData)rtnData;
        }

    private void ErrorLog(PlayFabError error)
        {
            Debug.LogError(error.GenerateErrorReport());
        }

        #region SampleCode
#if UNITY_EDITOR

        public void TestDebuge()
        {
            PlayFabClientAPI.ExecuteCloudScript(new PlayFab.ClientModels.ExecuteCloudScriptRequest()
            {
                FunctionName = "DailyRewardAdd",
                GeneratePlayStreamEvent = true
            }, cloudResult => {
                TestDebugLog(cloudResult);
            },
            error => { ErrorLog(error); }
            );
        }
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