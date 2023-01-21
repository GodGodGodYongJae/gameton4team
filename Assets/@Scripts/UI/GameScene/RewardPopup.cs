using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardPopup : MonoBehaviour
{
    public GameObject RewardPopus;
    public void WeaponChange(int weaponNum)
    {
        Managers.Events.PostNotification(Define.GameEvent.ChangeWeapon, null, weaponNum);
        RewardPopus.SetActive(false);
    }
}
