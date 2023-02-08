using Assets._Scripts.Game.Weapon;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Weapon_Bow_n : LongDistanceWeapon
{
    int range = 10;
    public override UniTaskVoid Attack()
    {
        Managers.Sound.PlaySFX("Bow");
        return base.Attack();
    }
}