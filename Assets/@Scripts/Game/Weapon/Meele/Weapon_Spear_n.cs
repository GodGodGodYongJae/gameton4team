using Assets._Scripts.Game.Weapon;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Weapon_Spear_n : WeaponMelee
{
    public override UniTaskVoid Attack()
    {
        Managers.Sound.PlaySFX("Sword_1");
        return base.Attack();
    }
}