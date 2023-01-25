using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField]
    public WeaponData weaponData;
    protected bool isAttack = false;
    protected Player player;
    protected CancellationTokenSource cts = new CancellationTokenSource();
    //이미 피해를 입은 몬스터 리스트 * 중복 공격이 일어나면 안되기 때문에.
    protected List<GameObject> damagedMonsterList = new List<GameObject>();
    public virtual void Start()
    {
      GameObject pGO = Managers.Object.GetSingularObjet(StringData.Player);
      player = pGO.GetComponent<Player>();
     
    }

    protected virtual void FEffectFollow()
    {

    }
    public virtual async UniTaskVoid RegsiterEffect()
    {
        await Managers.Object.RegisterObject(weaponData.Effect.name, 5);
        Attack().Forget();
    }

    public abstract void ChangeWeaponFixedUpdateDelete();
    public abstract UniTaskVoid Attack();


}
