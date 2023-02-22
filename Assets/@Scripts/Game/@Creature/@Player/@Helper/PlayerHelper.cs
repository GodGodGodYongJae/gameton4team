using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.Game._Creature._Player._Helper
{
    abstract class PlayerHelper
    {
        protected Player player;
        protected PlayerData playerData;
        protected Transform transform;

        public PlayerHelper(Player player)
        {
            this.player = player;
            playerData = player.PlayerData;
            transform = player.transform;
        }
    }
}
