using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Scripts.Game.Weapon
{
    public class Bullet : MonoBehaviour
    {
        //이미 피해를 입은 몬스터 리스트 * 중복 공격이 일어나면 안되기 때문에.
        private List<GameObject> damagedMonsterList = new List<GameObject>();
        private int Damage = 0;
        private int AttackDuration = 0;
        private Player player;
        private bool isInit = false;
        private BoxCollider2D box;
        Weapon_Wand_n wand;
        public float speed = 10;

        private void Awake()
        {
            wand = GameObject.Find("L_Weapon").GetComponent<Weapon_Wand_n>();
            // 자식한테 보통 이펙트가 있음 만약 해당 이펙트에 자식이 없거나 이상하다면
            // 이거 Bullet 자체를 부모 클래스로 빼서, Child를 일일히 지정해줘야할듯..?
            BoxCollider2D childBox = this.gameObject.transform.GetChild(0)
            .gameObject.GetOrAddComponent<BoxCollider2D>();
            childBox.isTrigger = true;
            this.box = gameObject.GetOrAddComponent<BoxCollider2D>();
            this.box.size = childBox.size;
            this.box.isTrigger = true;
        }
        void Update()
        {
            float moveX = speed * Time.deltaTime;
            Debug.Log(wand.direction);
            transform.Translate(wand.direction * moveX, 0, 0);
        }
        public void InitBulletData(WeaponData weaponData, Player player)
        {
            damagedMonsterList.Clear();
            this.Damage = weaponData.AttackDamge + player.GetPlayerDamage();
            this.player = player;
            this.AttackDuration = weaponData.AttackDuration;
            this.isInit = true;
            Duration().Forget();
        }

        private async UniTask Duration()
        {
            await UniTask.Delay(AttackDuration);
            RemoveEffect();
        }
        private void RemoveEffect()
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