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
    //이미 피해를 입은 몬스터 리스트 * 중복 공격이 일어나면 안되기 때문에.
    protected List<GameObject> damagedMonsterList = new List<GameObject>();
    public virtual void Start()
    {
        player = this.gameObject.transform.parent.GetComponent<Player>();
        Attack().Forget();
    }
    public abstract UniTaskVoid Attack();
}
