using Assets._Scripts.Game.Weapon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Scripts.Controller
{
    public class WeaponSlotController
    {
        public class WeaponSlot 
        {
            public WeaponSlot(Define.WeaponType Type)
            {
                this.Type = Type;
                Managers.Resource.LoadAsync<ScriptableObject>(Type.ToString() + "_data", (succss) =>
                {
                  weaponData = (WeaponData)succss;
                });
            }
            public Define.WeaponType Type;
            public int level = 1;
            public int atklevel = 1;
            public int atkspeedlevel = 1;
            public WeaponData weaponData;
        }


        List<WeaponSlot> Slot = new List<WeaponSlot>();
        private int SlotSize = 3;
        private int CurrentWeaponSlot = 0;
        public void NewWeapon(WeaponSlot slot)
        {
            if (Slot.Count < SlotSize)
               Slot.Add(slot);
            else
               ChangeSlotWeapon(CurrentWeaponSlot, slot);
            
                
        }

        /// <summary>
        /// 해당 슬롯에 해당 Type의 무기로 교체
        /// </summary>
        /// <param name="slotNum"></param>
        /// <param name="slot"></param>
        public void ChangeSlotWeapon(int slotNum,WeaponSlot slot)
        {
            Slot[slotNum] = slot;
            // UI Change Todo
        }

        /// <summary>
        /// 해당 무기를 가지고 있나? 있다면 True를 반환 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool isHaveWeapon(Define.WeaponType type)
        {
            for (int i = 0; i < Slot.Count; i++)
            {
                if (Slot[i].Type == type)
                    return true;    
            }
            return false;
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