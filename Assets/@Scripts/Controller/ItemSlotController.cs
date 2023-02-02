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
            public ItemSlot(Item itemData)
            {
                item = itemData;
            }
            public Item item;

        }

        List<ItemSlot> _slots = new List<ItemSlot>();
        public IReadOnlyList<ItemSlot> Slots => _slots;

        /// <summary>
        /// 아이템 사용할 슬롯 index 
        /// </summary>
        /// <param name="index"></param>
        public bool Use(int index)
        {
            if (_slots[index] == null) return false;
            //사용 가능한 아이템인 경우 
            if (_slots[index].item is IUsableItem uItem)
            {
                return uItem.Use();
                //if(successded)
                //{
                //    UpdateSlot();
                //}
            }
            return false;
        }

        public readonly int SlotSize = 2;
        public void CreateSlotItem(ItemSlot slot)
        {
            slot.item.Data.CreateItem();
            _slots.Add(slot);
        }

 

    }
}