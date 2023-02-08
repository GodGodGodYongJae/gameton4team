using Assets._Scripts.Game.Weapon;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Weapon_Sword : WeaponMelee
{
    public override UniTaskVoid Attack()
    {
        Managers.Sound.PlaySFX("Sword");
        return base.Attack();
    }
}