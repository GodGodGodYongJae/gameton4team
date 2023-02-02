using Assets._Scripts.Game.Interface;
using System.Collections;
using UnityEngine;

namespace Assets._Scripts.Game.Items
{
    /// <summary> 수량 아이템 - 포션 아이템 </summary>
    public class PortionItem : CountableItem, IUsableItem
    {
        public PortionItem(PortionItemData data, int amount = 1) : base(data, amount) { }

        public bool Use()
        {
            // 임시 : 개수 하나 감소
            Amount--;
            Debug.Log("포션사용!");
            return true;
        }

        protected override CountableItem Clone(int amount)
        {
            return new PortionItem(CountableData as PortionItemData, amount);
        }
    }
}