using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObj/Data/Weapon", fileName = "Weapon_")]
public class WeaponData : ScriptableObject
{
    [SerializeField]
    int attackDmg;
    [SerializeField]
    int attackDealay;
    [SerializeField]
    int attackDuration;
    [SerializeField]
    GameObject skilEffect;
    [SerializeField]
    Vector2 effectPos;
    public Vector2 EffectPos { get { return effectPos; } }
    public int AttackDamge { get { return attackDmg; } }
    public int AttackDealay { get { return attackDealay; } }
    public int AttackDuration { get { return attackDuration; } }

    public async UniTask<GameObject> Effect()
    {
        GameObject effect = await Managers.Object.InstantiateAsync(skilEffect.name, effectPos);
        //effect가 검에 계속 따라가야하는데 어떻게 해야할까? 
        //effect가 검에 계속 따라가는 건 해결 했는데, 방향을 틀어버리면 어떻게 해야할까?
        return effect;
    }
}
