using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.Game.Weapon
{
    public class PlayerBullet : Bullet
    {
        private List<GameObject> damagedMonsterList = new List<GameObject>();
        private Player player;
        private int Damage = 0;
        private int AttackDuration = 0;


        public virtual void InitBulletData(WeaponData weaponData, Player player, object obj = null)
        {
            damagedMonsterList.Clear();
            this.Damage = weaponData.AttackDamge + player.GetPlayerDamage();
            this.player = player;
            this.AttackDuration = weaponData.AttackDuration;
            this.isInit = true;
            Duration().Forget();
        }

        protected override async UniTask Duration()
        {
            await UniTask.Delay(AttackDuration);
            RemoveEffect();
        }

        protected override void RemoveEffect()
        {
            isInit = false;
            Damage = 0;
            player = null;
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            this.gameObject.SetActive(false);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            //이닛이 이루어졌을 때만
            if (isInit == false) return;
            //이미 해당 몬스터가 한번 공격을 받았으면 리턴
            if (damagedMonsterList.Contains(collision.gameObject))
                return;

            Creature creature = collision.GetComponent<Creature>();
            if (creature == null) return;

            if (creature.GetType == Creature.Type.Monster)
            {
                damagedMonsterList.Add(collision.gameObject);
                creature.Damage(Damage, player);
            }
        }
    }
}