
using System.Collections;
using UnityEngine;

namespace Assets._Scripts.Game.Items
{
    /// <summary> 소비 아이템 정보 </summary>
    [CreateAssetMenu(fileName = "Item_RespawnCard_", menuName = "Inventory System/Item Data/RespawnCard", order = 3)]
    public class RespawnCardItemData : CountableItemData
    {
        public override Item CreateItem()
        {
            return new RespawnCardItem(this);
        }
    }
}