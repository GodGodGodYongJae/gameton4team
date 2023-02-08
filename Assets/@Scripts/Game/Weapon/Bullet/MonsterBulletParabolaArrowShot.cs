using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;
using System.Collections;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Assets._Scripts.Game.Weapon
{
    public class MonsterBulletParabolaArrowShot : Bullet
    {
        private Monster monster;
        private float Damage = 0;
        private float AttackDuration = 3000;
        private float direction = 0;
        float range = 5;

        public void InitBulletData(Monster monster)
        {
            this.monster = monster;
            this.Damage = monster.MonsterData.AttackDamage;
            this.isInit = true;
            this.direction = Mathf.Clamp(monster.transform.localScale.x, 1, -1);
            if (monster.MonsterData.Duration != 0) AttackDuration = (float)this.monster.MonsterData.Duration * 1000;
            if (monster.MonsterData.ProjectileSpeed != 0) this.range = monster.MonsterData.ProjectileSpeed;
            startPos = transform.position;
            endPos = startPos + new Vector3(direction * 7, 0, 0);
            StartCoroutine("BulletMove");
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

        private void OnTriggerStay2D(Collider2D collision)
        {
            //이닛이 이루어졌을 때만
            if (isInit == false) return;

            Creature creature = collision.GetComponent<Creature>();
            if (creature == null) return;

            if (creature.GetType == Creature.Type.Player)
            {
                creature.Damage(Damage, monster);
            }
        }

        private Vector3 startPos, endPos;
        //땅에 닫기까지 걸리는 시간
        protected float timer;
        protected float timeToFloor;


        protected static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
        {
            Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

            var mid = Vector3.Lerp(start, end, t);

            return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
        }

        protected IEnumerator BulletMove()
        {
            timer = 0f;
            while (transform.position.y >= startPos.y)
            {
                timer += Time.deltaTime;
                Vector3 tempPos = Parabola(startPos, endPos, 0.5f, timer);
                transform.position = tempPos;
                Rotate().Forget();
                yield return new WaitForEndOfFrame();
            }
        }

        async UniTaskVoid Rotate()
        {
            await transform.DORotate(new Vector3(0, 0, (direction) * 10), 0.3f, 0);

            await transform.DORotate(new Vector3(0, 0, (-1 * direction) * 25), 0.2f, 0);

        }
    }
}