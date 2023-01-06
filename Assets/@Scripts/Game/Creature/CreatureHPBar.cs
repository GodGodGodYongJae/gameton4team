using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureHPBar : MonoBehaviour
{
    [SerializeField]
    Image HpImg;
    
    public void Damage(int curhp, int maxhp) => HpImg.fillAmount = (float)curhp / (float)maxhp;
}
