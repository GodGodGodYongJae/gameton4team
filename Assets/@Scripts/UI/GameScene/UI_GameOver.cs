using Assets._Scripts.Game.Items;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Scripts.UI.GameScene
{
    public class UI_GameOver : MonoBehaviour
    {
        [SerializeField]
        Button RespawnButton;
        [SerializeField]
        Button AdMobButton;

        TextMeshProUGUI RespawnQuantityText;
        TextMeshProUGUI AdmobQuantityText;
        // 부활권 불러옴 이것도 추후 수정 해야함.

        public RespawnCardItemData RespawnData;
        private RespawnCardItem respawnCardItem;
        int Quantity;
        int AdmobQuantity;
        private void Start()
        {
            RespawnButton.onClick.AddListener(() => OnRespawnButton());
            AdMobButton.onClick.AddListener(() => OnAdMob().Forget());
            RespawnQuantityText = RespawnButton.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            AdmobQuantityText = RespawnButton.transform.Find("Text").GetComponent<TextMeshProUGUI>();


            RespawnData.CreateItem();
            Quantity = Managers.PlayFab.FindItemQuantity(RespawnData.Key);
            respawnCardItem = new RespawnCardItem(RespawnData, Quantity);

            RespawnQuantityText.text = Quantity.ToString();

            this.gameObject.SetActive(false);
        }

        private async UniTaskVoid OnAdMob()
        {
          await Managers.Admob.RequestAndLoadRewardedAd(StringData.AdMob.Respawn,()=> {
              Player player = Managers.Object.GetSingularObjet(StringData.Player).GetComponent<Player>();
              player.Respawn();
              this.gameObject.SetActive(false);
          });
        }

        private void OnRespawnButton()
        {
            if (Quantity <= 0) return;
            respawnCardItem.Use();
            RespawnButton.gameObject.SetActive(false);
            this.gameObject.SetActive(false);
        }
    }
}
