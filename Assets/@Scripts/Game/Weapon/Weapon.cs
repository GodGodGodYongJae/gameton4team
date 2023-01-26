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
    protected List<GameObject> damagedMonsterList = new List<GameObject>();
    public virtual void Start()
    {
      GameObject pGO = Managers.Object.GetSingularObjet(StringData.Player);
      player = pGO.GetComponent<Player>();
     
    }

    protected virtual void FEffectFollow()
    {

    }
    public virtual async UniTaskVoid InitWeapon()
    {
        weaponData.DataInit();
        await Managers.Object.RegisterObject(weaponData.Effect.name, 5);
        Attack().Forget();
    }

    public abstract void ChangeWeaponFixedUpdateDelete();
    public abstract UniTaskVoid Attack();

}
