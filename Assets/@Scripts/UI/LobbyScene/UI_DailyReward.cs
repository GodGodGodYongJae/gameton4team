using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Scripts.UI.LobbyScene
{
    public class UI_DailyReward : MonoBehaviour
    {
        //public class DailyReward {
        //    int dailynum;
        //    Image image;
        //    public bool isComplated()
        //    {
        //        int currentDaily = int.Parse(Managers.PlayFab.GetUserData(Manager.PlayFabManager.PlayerData.dailyReward));
        //        bool DailyCoin = (Managers.PlayFab.GetCurrencyData(StringData.DailyReward) > 0)? true :false;   
        //        if (DailyCoin && currentDaily >= dailynum)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    //public DailyReward(int dailynum,Image image)
        //    //{
        //    //    this.dailynum = dailynum;
        //    //    this.image = image;

        //    //    if (isComplated())
        //    //    {

        //    //    }
        //    //}
        //}

        int _currentDaily = 0; 
        void Start()
        {
            _currentDaily = Managers.PlayFab.GetCurrencyData(StringData.DailyReward);
        }

    }
}