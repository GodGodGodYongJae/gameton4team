using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
public class GroundController
{

    GameObject _PreviousGround;
    GameObject _CurrentGround;
    GameObject _FrontGround;
    GameObject _NextGround;

    private void Awake()
    {
        Managers.UpdateAction += MUpdate;
    }

    public void Init(IReadOnlyList<GameObject> GroundList)
    {
        int idx = Random.Range(0, GroundList.Count);
        BoxCollider2D CurGroundCol;
        Vector2 pos = new Vector2(0, -5f);

        GameObject _CurrentGround = Managers.Object.Get(GroundList[idx].name);
        _CurrentGround.transform.position = pos;
        _CurrentGround.SetActive(true);
        CurGroundCol = _CurrentGround.GetComponent<BoxCollider2D>();


        Vector3 CurExtents = CurGroundCol.bounds.extents;

         idx = Random.Range(0, GroundList.Count);
        _PreviousGround = Managers.Object.Get(GroundList[idx].name);
        _PreviousGround.SetActive(true);
         CurGroundCol = _PreviousGround.GetComponent<BoxCollider2D>();

        Vector3 PrevExtents = CurGroundCol.bounds.extents;
        pos.x -= CurExtents.x +PrevExtents.x;
        Debug.Log(pos.x);
        _PreviousGround.transform.position = pos;

    //    Vector2 pos = new Vector2(GroundList[idx].transform.lo)
    //    _PreviousGround = Managers.Object.Instantiate(GroundList[idx].name,)
    }

    private void MUpdate()
    {
       
    }

}
