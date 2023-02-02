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
            BindButton(typeof(Buttons));
            BindObject(typeof(GameObjeects));
            //Bind End 
            invenUI = GetObject((int)GameObjeects.InventoryUI).GetComponent<Rito.InventorySystem.InventoryUI>();
            GetObject((int)GameObjeects.InventoryUI).SetActive(false);
            LoadCurrecyData();
            ButtonInit();

            GetObject((int)GameObjeects.ShopUI).GetComponent<UI_Shop>().Lobby = this;
            GetObject((int)GameObjeects.ShopUI).SetActive(false);

            InitItemServer initItemServer = new InitItemServer(this);

            MatchDisplay();
            return true;
        }

       public async void LoadCurrecyData()
        {
            TextMeshProUGUI energyText = GetText((int)Texts.EnergyText);
            int data = await Managers.PlayFab.GetCurrencyData(StringData.Energy);
            energyText.text = data.ToString();
            //TextMeshProUGUI DiamondText = GetText((int)Texts.DiamondText);
            // data = await Managers.PlayFab.GetCurrencyData(StringData.Diamond);
            //DiamondText.text = data.ToString();

            TextMeshProUGUI CoinText = GetText((int)Texts.CoinText);
             data = await Managers.PlayFab.GetCurrencyData(StringData.Coin);
            CoinText.text = data.ToString();
        }

        void ButtonInit()
        {
            GetObject((int)GameObjeects.StartButton).BindEvent(OnStartButton);
            GetObject((int)GameObjeects.EnergyPlus).BindEvent(OnEnergyPlus);
            GetButton((int)Buttons.InventoryBtn).onClick.AddListener(() => {
                GetObject((int)GameObjeects.InventoryUI).SetActive(true);
            });
        }

        #region ButtonCallback

 
        bool isClickStart = false;

        async void OnStartButton()
        {
            if (isClickStart) return;
            isClickStart = true;

                int data = await Managers.PlayFab.GetCurrencyData(StringData.Energy);

                if (data >= 5)
                {
                    Managers.PlayFab.SetCurrecy(StringData.Energy, -5,
                         () =>
                        {
                            Managers.Scene.ChangeScene(Define.SceneType.GameScene);

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
            Debug.Log("Not enough energy to start game");

        }

        void OnEnergyPlus()
        {
            Managers.PlayFab.SetCurrecy(StringData.Energy, 5,()=> {
                LoadCurrecyData();
            });
            
        }
        #endregion

        void MatchDisplay()
        {
            _canvas.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            float fixedAspectRatio = 9f / 16f;
            float currentAspectRatio = (float)Screen.width / (float)Screen.height;
            if (currentAspectRatio > fixedAspectRatio)
            {
                _canvas.matchWidthOrHeight = 0;
            }
            else if (currentAspectRatio < fixedAspectRatio)
            {
                _canvas.matchWidthOrHeight = 1;
            }
        }
    }
}
