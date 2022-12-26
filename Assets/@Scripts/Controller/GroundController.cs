using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
public class GroundController : MonoBehaviour
{

    Ground _PreviousGround;
    Ground _CurrentGround;
    Ground _FrontGround;
    Ground _NextGround;

    private void Awake()
    {
        Managers.UpdateAction += MUpdate;
    }

    private void MUpdate()
    {
       
    }

}
