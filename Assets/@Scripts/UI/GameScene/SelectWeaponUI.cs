using Assets._Scripts.Controller;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Random = System.Random;

namespace Assets._Scripts.UI.GameScene
{
    public class SelectWeaponUI
    {
        UI_GameScene gameScene;

        public SelectWeaponUI(UI_GameScene gameScene)
        {
            this.gameScene = gameScene;
            WeaponSelect = gameScene.WeaponSelectObj;
            CardSlot = gameScene.CardSlotList;
            WeaponSlotController = gameScene.WeaponSlotController;
            Managers.Events.AddListener(Define.GameEvent.stageClear, StageClearEvent);
         
        }

        public void Test()
        {
            OpenWeaponSelectBox().Forget();
        }
        #region Listener
        private void StageClearEvent(Define.GameEvent eventType, Component Sender, object param)
        {
            if (eventType == Define.GameEvent.stageClear)
            {
                OpenWeaponSelectBox().Forget();
            }
        }
        #endregion

        #region Property

        private GameObject WeaponSelect;
        private List<GameObject> CardSlot;
        private WeaponSlotController WeaponSlotController;
        private readonly int _selectCardNum = 3;

        #endregion

        #region Private Class

        public class WeaponCard
        {
            public Sprite Image { get; }
            public string Name { get; }
            public string Explanation { get; }
            public WeaponSlotController.WeaponSlot WeaponSlot { get; }
            public WeaponData.UpgradeType UpgradeType { get; }

            public WeaponCard(Sprite sprite, string name, string explanation,
                WeaponSlotController.WeaponSlot weaponSlot,
                WeaponData.UpgradeType upgradeType)
            {
                Image = sprite;
                Name = name;
                Explanation = explanation;
                WeaponSlot = weaponSlot;
                UpgradeType = upgradeType;
            }
        }

        class RandomNumberGenerator<T>
        {
            private List<T> _generatedNumbers;
            private List<T> _upgradSlotList;
            private Random _random;
            private int _End;

            public RandomNumberGenerator( int End)
            {
                    _generatedNumbers = new List<T>();
                _upgradSlotList = new List<T>();
                    _End = End;
            }
            public void AddException(T type)
            {
                _generatedNumbers.Add(type);
            }
            public void AddUpgrade(T type)
            {
                _upgradSlotList.Add(type);   
            }
            public T Run()
            {
                if( _generatedNumbers.Count == _End) return default(T);
                _random = new Random(Guid.NewGuid().GetHashCode());
                int randomNumber = _random.Next(_End);
                if(_upgradSlotList.Count == 0)
                {
                    while (_generatedNumbers.Contains((T)(object)randomNumber))
                    {
                        randomNumber = _random.Next(_End);
                    }
                    _generatedNumbers.Add((T)(object)randomNumber);
                    return (T)(object)randomNumber;
                }

                while (!_upgradSlotList.Contains((T)(object)randomNumber))
                {
                    randomNumber = _random.Next(_End);
                    return (T)(object)randomNumber;
                }
                return (T)(object)randomNumber;
            }
        }

        #endregion

        #region OpenCard 
        GameObject CurrentSlotCard;
        bool isCheck = false;
        private async UniTaskVoid OpenWeaponSelectBox()
        {
            Time.timeScale = 0;
            WeaponSelect.SetActive(true);

            RandomNumberGenerator<Define.WeaponType> WeaponRandom = new RandomNumberGenerator<Define.WeaponType>((int)Define.WeaponType.End);
            WeaponRandom.AddException(Define.WeaponType.None);

            //if(WeaponSlotController.SlotList.Count == WeaponSlotController.SlotSize)
            //{
            //    for (int i = 0; i < WeaponSlotController.SlotList.Count; i++)
            //    {
            //        WeaponRandom.AddException(WeaponSlotController.SlotList[i].Type);
            //    }
            //}
            bool isMaxSlot = (WeaponSlotController.SlotList.Count >= WeaponSlotController.SlotSize) ? true : false;

            if(isMaxSlot)
            {
                for (int i = 0; i < WeaponSlotController.SlotSize; i++)
                {
                    WeaponRandom.AddUpgrade(WeaponSlotController.SlotList[i].Type);
                }
             
            }

            for (int i = 0; i < _selectCardNum; i++)
            {
                CurrentSlotCard = CardSlot[i];
                isCheck = false;
                
                WeaponSlotController.WeaponSlot GetSlot = await RandWeaponSelect(WeaponRandom.Run());
                if (isCheck == true)
                {
                    RandUpgradeSelect(GetSlot);
                }

                WeaponRandom.AddException(GetSlot.Type);
            }


        }

        private async UniTask<WeaponSlotController.WeaponSlot> RandWeaponSelect(Define.WeaponType WeaponType)
        {
            bool Lock = false;
            int CurrentDamage = 0;
            int CurrentUpgradeDealay = 0;
            int MaxTargets = 0;
            string explantion = "";
            WeaponSlotController.WeaponSlot slot = WeaponSlotController.GetWeapon(WeaponType);
            if(slot == null)
            {
                WeaponCard card = null;
                Lock = true;
                Managers.Resource.LoadAsync<ScriptableObject>((WeaponType).ToString() + "_data", (succss) =>
                {

                    slot = new WeaponSlotController.WeaponSlot((WeaponData)succss);
                    slot.Type = WeaponType;
                    CurrentDamage = slot.weaponData.GetLevelData(WeaponData.UpgradeType.AttackDamage, 1);
                    CurrentUpgradeDealay = slot.weaponData.GetLevelData(WeaponData.UpgradeType.AttackSpeed, 1);
                    MaxTargets = slot.weaponData.MaxTargets;
                    explantion = "무기 선택하기!" + "\n" + "공격력 : " + CurrentDamage + "\n" + "공격 딜레이 : " + CurrentUpgradeDealay + "\n" + "피격 가능수 : " + MaxTargets;
                   
                        card = new WeaponCard(
                        slot.weaponData.UIImage,
                        slot.weaponData.DisplayName,
                         explantion,
                         slot,
                         WeaponData.UpgradeType.NewWeapon
                        );

                    Lock = false;
                });
                await UniTask.WaitUntil(() => { return Lock == false; });
                SlotCardUISet(card);
                return slot;
            }
            isCheck = true;
            return slot;
        }

        private void RandUpgradeSelect(WeaponSlotController.WeaponSlot Slot)
        {

            Random random = new Random(Guid.NewGuid().GetHashCode());
            int randomUpgrade = random.Next(1, (int)WeaponData.UpgradeType.end);

            int CurrentUpgradeNum = 0;
            int NextUpgradeNum = 0;
            string explantion = "";
            WeaponData.UpgradeType upgradeType = WeaponData.UpgradeType.AttackDamage;

            switch ((WeaponData.UpgradeType)randomUpgrade)
            {
                case WeaponData.UpgradeType.AttackDamage:
                    NextUpgradeNum = Slot.weaponData.GetLevelData(WeaponData.UpgradeType.AttackDamage, Slot.atklevel + 1);
                    CurrentUpgradeNum = Slot.weaponData.GetLevelData(WeaponData.UpgradeType.AttackDamage, Slot.atklevel);
                    explantion = " 공격력 증가 :" + (NextUpgradeNum - CurrentUpgradeNum).ToString() + "\n" + " 현재 공격력 : " + CurrentUpgradeNum;
                    upgradeType = WeaponData.UpgradeType.AttackDamage;
                    break;
                case WeaponData.UpgradeType.AttackSpeed:
                    NextUpgradeNum = Slot.weaponData.GetLevelData(WeaponData.UpgradeType.AttackSpeed, Slot.atklevel + 1);
                    CurrentUpgradeNum = Slot.weaponData.GetLevelData(WeaponData.UpgradeType.AttackSpeed, Slot.atklevel);
                    explantion = " 공격딜레이 감소 :" + (NextUpgradeNum - CurrentUpgradeNum).ToString() + "\n" + " 현재 딜레이 : " + CurrentUpgradeNum;
                    upgradeType = WeaponData.UpgradeType.AttackSpeed;
                    break;
            }

            WeaponCard card = new WeaponCard(Slot.weaponData.UIImage,
                Slot.weaponData.DisplayName,
                explantion,
                Slot,
                upgradeType);

            SlotCardUISet(card);
        }

        private void SlotCardUISet(WeaponCard card)
        {
            if (card == null) Debug.LogError("Card 없음");
            Text NameTxt = CurrentSlotCard.transform.Find("Name").GetComponent<Text>();
            Image image = CurrentSlotCard.transform.Find("Image").GetComponent<Image>();
            Text ExplanationTxt = CurrentSlotCard.transform.Find("Explanation").GetComponent<Text>();

            NameTxt.text = card.Name;
            image.sprite = card.Image;
            ExplanationTxt.text = card.Explanation;

            Button btn = CurrentSlotCard.GetOrAddComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => { SelectCard(card); });
        }

        private void SelectCard(WeaponCard ChooseSlotWeapon)
        {
            Time.timeScale = 1;
            this.WeaponSelect.SetActive(false);

            if (ChooseSlotWeapon.UpgradeType == WeaponData.UpgradeType.NewWeapon)
            {
                WeaponSlotController.NewWeapon(ChooseSlotWeapon.WeaponSlot);
            }
            else
            {
                ChooseSlotWeapon.WeaponSlot.LevelUp(ChooseSlotWeapon.UpgradeType);
            }

           gameScene.SyncInventoryInfo();
            OpenWeaponSelectBox().Forget();
        }
        #endregion
    }

}