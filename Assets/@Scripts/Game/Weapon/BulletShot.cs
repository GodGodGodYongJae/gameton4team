using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Scripts.Game.Weapon
{
    public class BulletShot : Bullet
    {
        Weapon_Wand_n wand;
        public float speed = 10;
        void Awake()
        {
            wand = GameObject.Find("L_Weapon").GetComponent<Weapon_Wand_n>();
        }
        void Update()
        {
            float moveX = speed * Time.deltaTime;
            Debug.Log(wand.direction);
            transform.Translate(wand.direction * moveX, 0, 0);
        }

    }
}
