using Assets._Scripts.UI.LobbyScene.InventoryUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Assets._Scripts.UI.LobbyScene
{
    enum GameObjeects 
    {
        StartButton,
        EnergyPlus,
        InventoryUI,
        ShopUI,
        ShowEnergyUI,
    }

    enum Buttons
    {
        InventoryBtn
    }

    enum Texts
    {
        EnergyText,
        //DiamondText,
        CoinText
    }

    public class UI_Lobby : UI_Scene
    {
        public Rito.InventorySystem.InventoryUI invenUI;
        private CanvasScaler _canvas;
        public override bool Init()
        {
            _canvas = this.GetComponent<CanvasScaler>();
            if (base.Init() == false)
                return false;
            //Bind
            BindText(typeof(Texts));
            //BindButton(typeof(Buttons));
            BindObject(typeof(GameObjeects));
            //Bind End 
            invenUI = GetObject((int)GameObjeects.InventoryUI).GetComponent<Rito.InventorySystem.InventoryUI>();
            GetObject((int)GameObjeects.InventoryUI).SetActive(false);
            Managers.PlayFab.SyncCurrencyDataFromServer(()=> { LoadCurrecyData(); });

            ButtonInit();

            GetObject((int)GameObjeects.ShopUI).SetActive(false);
            GetObject((int)GameObjeects.ShowEnergyUI).SetActive(false);

             initItemServer = new InitItemServer(this);

            Managers.Sound.PlayBGM("Lobby");
            MatchDisplay();
            Managers.Events.AddListener(Define.GameEvent.LobbyCurrency, CurrencySet);
            return true;
        }

        private void CurrencySet(Define.GameEvent eventType, Component Sender, object param)
        {
           if(eventType == Define.GameEvent.LobbyCurrency)
            {
                LoadCurrecyData();
            }
        }

        InitItemServer initItemServer;
       public void LoadCurrecyData()
        {
            TextMeshProUGUI energyText = GetText((int)Texts.EnergyText);
            int data = Managers.PlayFab.GetCurrencyData(StringData.Energy);
            energyText.text = data.ToString();
            //TextMeshProUGUI DiamondText = GetText((int)Texts.DiamondText);
            // data = await Managers.PlayFab.GetCurrencyData(StringData.Diamond);
            //DiamondText.text = data.ToString();

            TextMeshProUGUI CoinText = GetText((int)Texts.CoinText);
             data =  Managers.PlayFab.GetCurrencyData(StringData.Coin);
            CoinText.text = data.ToString();
        }

        void ButtonInit()
        {
            GetObject((int)GameObjeects.StartButton).BindEvent(OnStartButton);
            GetObject((int)GameObjeects.EnergyPlus).BindEvent(OnEnergyPlus);
            //GetButton((int)Buttons.InventoryBtn).onClick.AddListener(() => {
            //    GetObject((int)GameObjeects.InventoryUI).SetActive(true);
            //});
        }

        #region ButtonCallback

 
        bool isClickStart = false;

         void OnStartButton()
        {
            if (isClickStart) return;
            isClickStart = true;

                int data = Managers.PlayFab.GetCurrencyData(StringData.Energy);

                if (data >= 5)
                {
                    Managers.PlayFab.SetCurrecy(StringData.Energy, -5,
                         () =>
                        {
                            // 일단 게임 진입 전 서버에서 Inventory를 새로 받아주고 진입.
                            Managers.PlayFab.GetUserInventory(() => {
                                Managers.Scene.ChangeScene(Define.SceneType.GameScene);
                            });
                          

                        });
                }
                else
                {
                    isClickStart = false;
                    ShowNotEnoughEnergyMessage();
                }
          
        }

        private void ShowNotEnoughEnergyMessage()
        {
            GetObject((int)GameObjeects.ShowEnergyUI).SetActive(true);
        }

        void OnEnergyPlus()
        {
            //Managers.PlayFab.SetCurrecy(StringData.Energy, 5,()=> {
            //    LoadCurrecyData();
            //});
            
        }
        #endregion

        void MatchDisplay()
        {
            _canvas.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            float fixedAspectRatio = 9f / 16f;
            float currentAspectRatio = (float)Screen.width / (float)Screen.height;
            if (currentAspectRatio > fixedAspectRatio)
            {
                _canvas.matchWidthOrHeight = 1;
            }
            else if (currentAspectRatio < fixedAspectRatio)
            {
                _canvas.matchWidthOrHeight = 0;
            }
        }
    }
}
