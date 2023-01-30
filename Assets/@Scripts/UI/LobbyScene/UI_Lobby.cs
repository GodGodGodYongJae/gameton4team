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
    enum ButtonObject 
    {
        StartButton,
        EnergyPlus
    }

    enum Texts
    {
        EnergyText,
        //DiamondText,
        CoinText
    }
    public class UI_Lobby : UI_Scene
    {
        private CanvasScaler _canvas;
        public override bool Init()
        {
            _canvas = this.GetComponent<CanvasScaler>();
            if (base.Init() == false)
                return false;
            //Bind
            BindText(typeof(Texts));
            BindObject(typeof(ButtonObject));
            //Bind End 
    
            LoadCurrecyData();
            ButtonInit();

            MatchDisplay();
            return true;
        }

        async void LoadCurrecyData()
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
            GetObject((int)ButtonObject.StartButton).BindEvent(OnStartButton);
            GetObject((int)ButtonObject.EnergyPlus).BindEvent(OnEnergyPlus);
        }

        #region ButtonCallback

 
        bool isClickStart = false;

        async void OnStartButton()
        {
            if (isClickStart) return;
            isClickStart = true;

            try
            {
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
                    ShowNotEnoughEnergyMessage();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to get or set currency data: " + ex.Message);
            }
            finally
            {
                isClickStart = false;
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
                _canvas.matchWidthOrHeight = 1;
            }
            else if (currentAspectRatio < fixedAspectRatio)
            {
                _canvas.matchWidthOrHeight = 0;
            }
        }
    }
}
