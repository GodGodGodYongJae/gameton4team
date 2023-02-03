using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orc_burglar : ProximityMonster
{

    public override void Damage(float dmg, Creature Target)
    {
        base.Damage(dmg * 0.95f, Target);
        creatureHPBar.Damage(_hp, _creatureData.MaxHP);
    }
}
