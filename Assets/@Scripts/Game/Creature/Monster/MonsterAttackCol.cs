using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackCol : MonoBehaviour
{

    Monster monster;
    private void Awake()
    {
        monster = transform.parent.GetComponent<Monster>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Creature creature = collision.gameObject.GetComponent<Creature>();
        if (creature == null) return;

        if (creature.GetType == Creature.Type.Player)
        {
            creature.Damage(monster.MonsterData.AttackDamage, monster);
        }

    }

}
