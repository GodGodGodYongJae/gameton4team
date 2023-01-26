using System.Collections;
using UnityEngine;

namespace Assets._Scripts.Game.Weapon
{
    public class WeaponSlot
    {
        private Define.WeaponType type;
        private int atk;
        private int atkspeed;
        private int level;

        public Define.WeaponType Type => type;
        public enum UpgradType
        {
            atk,
            atkspeed,
        }


        public WeaponSlot(Define.WeaponType type)
        {
            this.type = type;
            level = 1;
            atk = 0;
            atkspeed = 1000;
        }

        public void LevelUp(UpgradType upgradType)
        {
            level++;
            //TODO 파싱해서 type 넘겨주고 Dictionary로 UpgradType,int 해서
            // 데이터값 바꿔주면 될듯 임시적으로 atk +1 speed - 100 해주면 될 듯?
            atk++;
            atkspeed -= 100;
        }
    }
}