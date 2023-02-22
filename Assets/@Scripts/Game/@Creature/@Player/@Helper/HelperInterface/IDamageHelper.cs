using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._Scripts.Game._Creature._Player._Helper.HelperInterface
{
    internal interface IDamageHelper
    {
        public void Damage(float Dmg, Creature Sender);
        public void Respawn();

        public void AddHp(float hp);
    }
}
