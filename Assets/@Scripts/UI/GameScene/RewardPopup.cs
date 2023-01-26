using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardPopup : MonoBehaviour
{
    public GameObject RewardPopus;
    public void WeaponChange(int weaponNum)
    {
        RewardPopus.SetActive(false);
    }
}
