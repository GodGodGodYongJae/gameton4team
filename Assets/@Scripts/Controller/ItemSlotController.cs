using System.Collections;
using System.Collections.Generic;
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
            }
            public ItemData ItemData;
            public int Ammount = 0;

            public bool Run()
            {
                if (Ammount <= 0) return false;
                ItemData.Run();
                return true;
            }
        }

        List<ItemSlot> _slots = new List<ItemSlot>();
        public IReadOnlyList<ItemSlot> Slots => _slots;

        public readonly int SlotSize = 2;
        public void CreateSlotItem(ItemSlot slot)
        {
            _slots.Add(slot);
        }
        public bool UseItem(int idx)
        {
           return _slots[idx].Run();
        }
    }
}