using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Creature : MonoBehaviour, IDamageble
{
    public enum Type { 
        Player,
        Monster
    }
    [SerializeField]
    protected CreatureData _creatureData;
    protected Type _type; 
    protected int _hp;

    private void Awake()
    {
        _hp = _creatureData.MaxHP;
    }
    public virtual Type GetType()
    { return _type; }
    public virtual void Damage(int dmg, Creature Target)
    {
        _hp -= dmg;
        if(_hp <= 0)
        {
            Death();
        }
        Debug.Log("데미지 입음");
    }
    
    public virtual void Death()
    {
        
        transform.gameObject.SetActive(false);
    }
}
