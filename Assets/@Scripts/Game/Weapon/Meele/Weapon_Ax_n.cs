using Assets._Scripts.Game.Weapon;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
public class Weapon_Ax_n : WeaponMelee
{
    public override void Start()
    {
        base.Start();
        effectoDir = -1;
    }
}