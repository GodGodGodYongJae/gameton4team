using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField]
    protected WeaponData weaponData;
    protected bool isAttack = false;
    protected Player player;
    public virtual void Start()
    {
        player = this.gameObject.transform.parent.GetComponent<Player>();
        Attack().Forget();
    }
    public abstract UniTaskVoid Attack();
}
