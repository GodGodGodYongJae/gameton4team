using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using Random = UnityEngine.Random;

public class GroundController
{

    GameObject _PreviousGround;
    GameObject _CurrentGround;
    GameObject _NextGround;

    GameObject _ReservationGround;
    GameScene _GameScene;

    IReadOnlyList<GameObject> _GroundList;

    public GroundController(GameScene gameScene)
    {
        _GameScene = gameScene;
    }

    public async UniTaskVoid Init(IReadOnlyList<GameObject> GroundList)
    {

        _GroundList = GroundList;
        //임의로 idx를 받아온다.
        int idx = Random.Range(0, GroundList.Count);
    
        Vector2 pos = new Vector2(0, GroundPosY);
         _CurrentGround = await CreateGround(pos, GroundList[idx].name);

        _PreviousGround = await CreateGround(pos, GroundList[idx].name);
        pos = new Vector2(-SpawnPosMath(_CurrentGround.GetComponent<BoxCollider2D>(), _PreviousGround.GetComponent<BoxCollider2D>()), Define.GroundPosY);
        _PreviousGround.transform.position = pos;
        
        GameObject prevWall = _GameScene.WallObjects[(int)GameScene.Wall.Prev];
        pos = new Vector2(-SpawnPosMath(_PreviousGround.GetComponent<BoxCollider2D>(), prevWall.GetComponent<BoxCollider2D>(),-1), 0);
        prevWall.transform.position = pos;

        _NextGround = await CreateGround(pos, GroundList[idx].name);
        pos = new Vector2(SpawnPosMath(_CurrentGround.GetComponent<BoxCollider2D>(), _NextGround.GetComponent<BoxCollider2D>()), Define.GroundPosY);
        _NextGround.transform.position = pos;

        GameObject frontWall = _GameScene.WallObjects[(int)GameScene.Wall.Front];
        pos = new Vector2(SpawnPosMath(_NextGround.GetComponent<BoxCollider2D>(), frontWall.GetComponent<BoxCollider2D>()), 0);
        frontWall.transform.position = pos;
   

    
    }

    float SpawnPosMath(BoxCollider2D a, BoxCollider2D b,float back = 1)
    {

        float x = a.transform.position.x; 
        x = (a.bounds.extents.x + b.bounds.extents.x) + ( x * back );
        return x;
    }

    async UniTask<GameObject> CreateGround( Vector2 pos, string name,Action<UniTask> callback = null)
    {
        GameObject go = await Managers.Object.InstantiateAsync(name,pos);
        callback?.Invoke(UniTask.CompletedTask);
        return go;
    }

    private void MUpdate()
    {
       
    }

}
