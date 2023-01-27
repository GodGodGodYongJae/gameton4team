using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Scripts.Game.Weapon
{
    public class BulletShot : PlayerBullet
    {
        LongDistanceWeapon longDistance;
        int range = 0;

        protected override void Awake()
        {
            base.Awake();
        }
        public override void InitBulletData(WeaponData weaponData, Player player, object obj)
        {
            base.InitBulletData(weaponData, player, obj);
            longDistance = (LongDistanceWeapon)obj;
            range = weaponData.Range;
        }
        void Update()
        {
            float moveX = range * Time.deltaTime;
            transform.Translate(longDistance.direction * moveX, 0, 0);
        }

    }
}