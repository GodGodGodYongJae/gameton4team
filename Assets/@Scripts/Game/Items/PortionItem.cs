using Assets._Scripts.Game.Interface;
using System.Collections;
using UnityEngine;

namespace Assets._Scripts.Game.Items
{
    /// <summary> 수량 아이템 - 포션 아이템 </summary>
    public class PortionItem : CountableItem, IUsableItem
    {
        float HealValue = 0;
        public PortionItem(PortionItemData data, int amount = 1) : base(data, amount) 
        {
            HealValue = data.Value;
        }

        public bool Use()
        {
            if (Amount <= 0) return false;
            // 임시 : 개수 하나 감소
            Amount--;
            BackEnd();

            GameObject playerObj = Managers.Object.GetSingularObjet(StringData.Player);
            Player player = playerObj.GetComponent<Player>();
            float heal = player.PlayerData.MaxHP * (HealValue * 0.01f);

            player.AddHp(heal);
            return true;
        }

        protected override CountableItem Clone(int amount)
        {
            return new PortionItem(CountableData as PortionItemData, amount);
        }
    }
}