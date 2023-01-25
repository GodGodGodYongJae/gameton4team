using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObj/Data/Weapon", fileName = "Weapon_")]
public class WeaponData : ScriptableObject
{
    [SerializeField]
    int attackDmg;
    [SerializeField]
    int attackDealay;
    [SerializeField]
    [Tooltip("이펙트 에니메이션 시간 넣기")]
    int attackDuration;
    [SerializeField]
    GameObject skilEffect;
    [SerializeField]
    Vector2 effectPos;

    public Vector2 EffectPos { get { return effectPos; } }
    public int AttackDamge { get { return attackDmg; } }
    public int AttackDealay { get { return attackDealay; } }
    public int AttackDuration { get { return attackDuration; } }
    public GameObject Effect => skilEffect;

    public async UniTask<GameObject> EffectSpawnAsync(Vector2 pos)
    {
        GameObject effect = await Managers.Object.InstantiateAsync(skilEffect.name, pos);
        return effect;
    }
}
