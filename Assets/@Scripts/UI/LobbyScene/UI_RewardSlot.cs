using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Scripts.UI.LobbyScene
{
    public class UI_RewardSlot : MonoBehaviour
    {
        Button btn;
        
        [SerializeField]
        SerializableDictionary<ItemData,int> RewardItems;

        public int NeedReward;
        public int RewardCoin;

        [SerializeField]
        GameObject ComplatedImg;
        private void Awake()
        {
            btn = GetComponent<Button>();
            
        }
        private void OnEnable()
        {
            int current_Reward = int.Parse(Managers.PlayFab.GetUserData(Manager.PlayFabManager.PlayerData.dailyReward));
            
            if (current_Reward >= NeedReward)
            {
                ComplatedImg.SetActive(true);
            }

            else if (current_Reward +1 == NeedReward)
            {
                btn.onClick.AddListener(() => { OnClickReward(); });
            }
        }

        private void OnClickReward()
        {

            if(Managers.PlayFab.GetCurrencyData(StringData.DailyReward) >= 1)
            {
                ComplatedImg.SetActive(true);
                this.btn.onClick.RemoveAllListeners();
                Managers.PlayFab.SetCurrecy(StringData.DailyReward, -1, () =>
                {       if (RewardItems.Count > 0)
                    
                    Managers.PlayFab.AddItemInventory2(RewardItems);
                    Managers.PlayFab.SetCurrecy(StringData.Coin, RewardCoin);
                    Managers.PlayFab.SetServerPlayerData(Manager.PlayFabManager.PlayerData.dailyReward, NeedReward.ToString());
                    Managers.Events.PostNotification(Define.GameEvent.LobbyCurrency, this);
                });

            }
        }

    }
}