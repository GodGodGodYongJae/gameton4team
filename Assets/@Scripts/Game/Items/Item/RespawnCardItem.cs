using Assets._Scripts.Game.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.Game.Items
{
    class RespawnCardItem : CountableItem, IUsableItem
    {
        public RespawnCardItem(CountableItemData data, int amount = 1) : base(data, amount)
        {
        }

        public bool Use()
        {
            if (Amount <= 0) return false;
            Amount--;
            BackEnd();

            GameObject playerObj = Managers.Object.GetSingularObjet(StringData.Player);
            Player player = playerObj.GetComponent<Player>();

            player.Respawn();

            return true;
        }

        protected override CountableItem Clone(int amount)
        {
            return new RespawnCardItem(CountableData as RespawnCardItemData, amount);
        }
    }
}
