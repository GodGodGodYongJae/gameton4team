using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using static UnityEditor.PlayerSettings;
using Random = UnityEngine.Random;

public class GroundController
{

    GameObject _PreviousGround;
    GameObject _CurrentGround;
    GameObject _NextGround;

    GameObject _ReservationGround;
    GameScene _GameScene;

    IReadOnlyList<GameObject> _GroundList;

    public float GetCurrentGroundPos() {
        if (_CurrentGround == null || _PreviousGround == null || _NextGround == null) return 0;
        return _CurrentGround.transform.position.x; 
    } 
    public float GetExtendSize()
    {
        if (_CurrentGround == null || _PreviousGround == null || _NextGround == null) return 0;
        return ExtendSize(_PreviousGround)+ExtendSize(_CurrentGround)+ExtendSize(_NextGround);
    }
   

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


        Managers.FixedUpdateAction += CheckBoundTest;
    }

    void CheckBoundTest()
    {
        float extendSize = ExtendSize(_NextGround);
        float pointCheck = _NextGround.transform.position.x + extendSize - 1;
        if(pointCheck <= _GameScene.PlayerGo.transform.position.x)
        {
            PushNextGround().Forget();    
        }
    }

    async UniTaskVoid PushNextGround()
    {
        _PreviousGround.SetActive(false);
        _PreviousGround = _CurrentGround;
        _CurrentGround = _NextGround;
        _NextGround = null;

        int idx = Random.Range(0, _GroundList.Count);
        Vector2 pos = new Vector2(0, GroundPosY);

        _NextGround = await CreateGround(pos, _GroundList[idx].name);
        pos = new Vector2(SpawnPosMath(_CurrentGround.GetComponent<BoxCollider2D>(), _NextGround.GetComponent<BoxCollider2D>()), Define.GroundPosY);
        _NextGround.transform.position = pos;

        GameObject prevWall = _GameScene.WallObjects[(int)GameScene.Wall.Prev];
        pos = new Vector2(-SpawnPosMath(_PreviousGround.GetComponent<BoxCollider2D>(), prevWall.GetComponent<BoxCollider2D>(), -1), 0);
        prevWall.transform.position = pos;

        GameObject frontWall = _GameScene.WallObjects[(int)GameScene.Wall.Front];
        pos = new Vector2(SpawnPosMath(_NextGround.GetComponent<BoxCollider2D>(), frontWall.GetComponent<BoxCollider2D>()), 0);
        frontWall.transform.position = pos;

        //제거 previous 
        //temp
        // 교체 previous = current
        // 교체 current = next
        //새로생성
        // next 새로생성 
        //제거.

    }




    #region private
    private float ExtendSize(GameObject go)
    {
        BoxCollider2D box = go.GetComponent<BoxCollider2D>();

        return box.bounds.extents.x;
    }
    float SpawnPosMath(BoxCollider2D a, BoxCollider2D b, float back = 1)
    {

        float x = a.transform.position.x;
        x = (a.bounds.extents.x + b.bounds.extents.x) + (x * back);
        return x;
    }

    async UniTask<GameObject> CreateGround(Vector2 pos, string name, Action<UniTask> callback = null)
    {
        GameObject go = await Managers.Object.InstantiateAsync(name, pos);
        callback?.Invoke(UniTask.CompletedTask);
        return go;
    }
    #endregion




}
