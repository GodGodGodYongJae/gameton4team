using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.Game.Weapon
{
    public class MonsterBulletStunShot : Bullet
    {
        private Monster monster;
        private float Damage = 0;
        private float AttackDuration = 3000;
        private float direction = 0;
        float range = 5;
        Player player;

        public void InitBulletData(Monster monster)
        {
            this.monster = monster;
            this.Damage = monster.MonsterData.AttackDamage;
            this.isInit = true;
            this.direction = Mathf.Clamp(monster.transform.localScale.x, 1, -1);
            if (monster.MonsterData.Duration != 0) AttackDuration = (float)this.monster.MonsterData.Duration * 1000;
            if (monster.MonsterData.ProjectileSpeed != 0) this.range = monster.MonsterData.ProjectileSpeed;
            Duration().Forget();
        }

        protected override async UniTask Duration()
        {
            await UniTask.Delay((int)AttackDuration);
            RemoveEffect();
            player.a = 1;

        }

        protected override void RemoveEffect()
        {
            isInit = false;
            Damage = 0;
            direction = 0;
            monster = null;
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            this.gameObject.SetActive(false);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            //이닛이 이루어졌을 때만
            if (isInit == false) return;

            Creature creature = collision.GetComponent<Creature>();
            if (creature == null) return;

            if (creature.GetType == Creature.Type.Player)
            {
                player = Managers.Object.GetSingularObjet(StringData.Player).GetComponent<Player>();
                creature.Damage(Damage, monster);
                player.a = 0;
            }
        }
    }
}