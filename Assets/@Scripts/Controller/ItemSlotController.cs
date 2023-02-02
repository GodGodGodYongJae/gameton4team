using Assets._Scripts.Game.Interface;
using Assets._Scripts.Game.Items;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets._Scripts.Controller
{
    public class ItemSlotController
    {
        public class ItemSlot
        {
            public ItemSlot(ItemData itemData, int ammount)
            {
                ItemData = itemData;
                Ammount = ammount;
                ItemData.CreateItem();
                item = ItemData;
            }
            public ItemData ItemData;
            public CountableItem item;
            public int Ammount = 0;
            
        }

        List<ItemSlot> _slots = new List<ItemSlot>();
        public IReadOnlyList<ItemSlot> Slots => _slots;

        /// <summary>
        /// 아이템 사용할 슬롯 index 
        /// </summary>
        /// <param name="index"></param>
        public void Use(int index)
        {
            if (_slots[index] == null) return;
            //사용 가능한 아이템인 경우 
            if (_slots[index].item is IUsableItem uItem)
            {
                bool successded = uItem.Use();
                //if(successded)
                //{
                //    UpdateSlot();
                //}
            }
        }

        public readonly int SlotSize = 2;
        public void CreateSlotItem(ItemSlot slot)
        {
            _slots.Add(slot);
        }

 

    }
}