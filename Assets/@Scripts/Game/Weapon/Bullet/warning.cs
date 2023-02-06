using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace Assets._Scripts.Game.Weapon
{
    public class warning : Bullet
    {
        private Monster monster;
        private float Damage = 0;
        private float AttackDuration = 100;
        private float direction = 0;
        float range = 5;

        Player player;
        public void InitBulletData(Monster monster)
        {
            this.monster = monster;
            this.Damage = monster.MonsterData.AttackDamage;
            this.isInit = true;
            this.direction = Mathf.Clamp(monster.transform.localScale.x, 1, -1);
            if (monster.MonsterData.ProjectileSpeed != 0) this.range = monster.MonsterData.ProjectileSpeed;
            Duration().Forget();

        }
        protected override async UniTask Duration()
        {
            await UniTask.Delay((int)AttackDuration);
            RemoveEffect();
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

        private void Update()
        {
            transform.position = monster.transform.position;
        }
    }
}
