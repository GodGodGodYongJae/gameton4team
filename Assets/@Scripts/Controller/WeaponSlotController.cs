using Assets._Scripts.Game.Weapon;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Scripts.Controller
{
    public class WeaponSlotController
    {
        public class WeaponSlot 
        {
            public WeaponSlot(WeaponData weaponData )
            {
                this.weaponData = weaponData;
                if(this.weaponData.data == null)
                    this.weaponData.AssetLoad();
            }
             
            public Define.WeaponType Type;
            public int level = 1;
            public int atklevel = 1;
            public int atkspeedlevel = 1;
            public WeaponData weaponData;
            public void LevelUp(WeaponData.UpgradeType upgradeType)
            {
                ++level;
                switch (upgradeType)
                {
                    case WeaponData.UpgradeType.AttackDamage:
                        weaponData.LevelUpData(upgradeType, ++atklevel);
                        break;
                    case WeaponData.UpgradeType.AttackSpeed:
                        weaponData.LevelUpData(upgradeType, ++atkspeedlevel);
                        break;
                   
                }
            }
        }


        List<WeaponSlot> Slot = new List<WeaponSlot>();
        public IReadOnlyList<WeaponSlot> SlotList => Slot;

        private int SlotSize = 3;
        public int CurrentWeaponSlot = 0;
        public void NewWeapon(WeaponSlot slot)
        {
            if (Slot.Count < SlotSize)
               Slot.Add(slot);
            else
                ChangeSlotWeapon(CurrentWeaponSlot, slot);
            Debug.Log(Slot.Count + "SLot Count");
        }
        public void testFunc()
        {
            for (int i = 0; i < Slot.Count; i++)
            {
                Debug.Log(Slot[i].Type+"CurrentSlot ");
            }
        }
        /// <summary>
        /// 해당 슬롯에 해당 Type의 무기로 교체
        /// </summary>
        /// <param name="slotNum"></param>
        /// <param name="slot"></param>
        public void ChangeSlotWeapon(int slotNum,WeaponSlot slot)
        {
            Slot[CurrentWeaponSlot] = slot;
            // UI Change Todo
        }

        public WeaponSlot GetWeapon(Define.WeaponType type)
        {
            for (int i = 0; i < Slot.Count; i++)
            {
                if (Slot[i].Type == type)
                    return Slot[i];
            }
            return null;
        }
    }
}