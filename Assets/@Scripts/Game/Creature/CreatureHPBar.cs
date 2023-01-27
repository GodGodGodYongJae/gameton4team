using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureHPBar : MonoBehaviour
{
    [SerializeField]
    Image HpImg;
    
    public void Damage(float curhp, float maxhp) => HpImg.fillAmount = curhp / maxhp;
}
