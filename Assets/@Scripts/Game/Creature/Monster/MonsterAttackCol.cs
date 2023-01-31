using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackCol : MonoBehaviour
{

    Monster monster;
    public void CreateAttackCol(Monster monster)
    {
        this.monster = monster;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Creature creature = collision.gameObject.GetComponent<Creature>();
        Debug.Log("test");
        if (creature == null) return;
        Debug.Log("test2");
        if (creature.GetType == Creature.Type.Player)
        {
            Debug.Log("test3");
            creature.Damage(monster.MonsterData.AttackDamage, monster);
        }

    }

}
