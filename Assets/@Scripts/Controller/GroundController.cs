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

    GameScene _GameScene;

    // 생성할 그라운드 리스트.
    IReadOnlyList<GameObject> _GroundList;

    // 현재 생성된 그라운드 링크드 리스트 처음 끝 삭제가 빈번히 발생하기 때문에 링크드 리스트 활용.
    LinkedList<GameObject> _Grounds = new LinkedList<GameObject>();
    // 이터레이터 같은 역할.
  
    // 생성자 GameScene DI 주입.
    public GroundController(GameScene gameScene)
    {
        _GameScene = gameScene;
    }
    //현재 그라운드 포지션. 링크드 리스트의 0 번째는 Prev 이기 때문에, +1 임.
    public float GetCurrentGroundPos() {
        CurrentGroundIdx();
        if (_CurrentGround == null || _PreviousGround == null || _NextGround == null) return 0;
        return _CurrentGround.transform.position.x;
    } 
    // Prev , Current , Next Box Size를 구해서, 카메라 제한영역 표시를 위한 메서드.
    public float GetExtendSize()
    {
        CurrentGroundIdx();
        if (_CurrentGround == null || _PreviousGround == null || _NextGround == null) return 0;
        return ExtendSize(_PreviousGround)+ExtendSize(_CurrentGround)+ExtendSize(_NextGround);
    }
   
    // Linked List에 등록된 Element들을 각 Ground에 동기화 해주는 메서드.
    private void CurrentGroundIdx()
    {
        if (_Grounds.Count < 3) return;
        LinkedListNode<GameObject> _GroundNode = _Grounds.First;
        _PreviousGround  = _GroundNode.Value;

        _GroundNode      = _GroundNode.Next;
        _CurrentGround   = _GroundNode.Value;

        _GroundNode      = _GroundNode.Next;
        _NextGround      = _GroundNode.Value;
    }
 

    // 처음 지형 생성.
    public async UniTaskVoid Init(IReadOnlyList<GameObject> GroundList)
    {

        //_GroundList = GroundList;
        ////임의로 idx를 받아온다.
        //int idx = Random.Range(0, GroundList.Count);

        //Vector2 pos = new Vector2(0, GroundPosY);
        // _CurrentGround = await CreateGround(pos, GroundList[idx].name);

        //_PreviousGround = await CreateGround(pos, GroundList[idx].name);
        //pos = new Vector2(-SpawnPosMath(_CurrentGround.GetComponent<BoxCollider2D>(), _PreviousGround.GetComponent<BoxCollider2D>()), Define.GroundPosY);
        //_PreviousGround.transform.position = pos;

        //GameObject prevWall = _GameScene.WallObjects[(int)GameScene.Wall.Prev];
        //pos = new Vector2(-SpawnPosMath(_PreviousGround.GetComponent<BoxCollider2D>(), prevWall.GetComponent<BoxCollider2D>(),-1), 0);
        //prevWall.transform.position = pos;

        //_NextGround = await CreateGround(pos, GroundList[idx].name);
        //pos = new Vector2(SpawnPosMath(_CurrentGround.GetComponent<BoxCollider2D>(), _NextGround.GetComponent<BoxCollider2D>()), Define.GroundPosY);
        //_NextGround.transform.position = pos;

        //GameObject frontWall = _GameScene.WallObjects[(int)GameScene.Wall.Front];
        //pos = new Vector2(SpawnPosMath(_NextGround.GetComponent<BoxCollider2D>(), frontWall.GetComponent<BoxCollider2D>()), 0);
        //frontWall.transform.position = pos;
        int idx = Random.Range(0, GroundList.Count);
        for (int i = 0; i < 5; i++)
        {
            GameObject go = await CreateGround(Vector2.zero, GroundList[idx].name);
            go.SetActive(false);
            _Grounds.AddLast(go);
            // 다시 랜덤으로 생성.
            idx = Random.Range(0, GroundList.Count);
        }
        //초반 Prev,Current는 그냥 무시함.
        LinkedListNode<GameObject> iter = _Grounds.First.Next;
        Vector2 pos = new Vector2(0, GroundPosY);
        iter.Value.transform.position = pos;
        iter.Value.SetActive(true);

        //초반 Prev,Current는 그냥 무시함.
        while (iter.Next != null)
        {
            //포스를 먼저 잡아주고, iter를 다음으로 옮기고, iter 다음 값을 실행.
            pos = new Vector2(SpawnPosMath(iter.Value, iter.Next.Value), pos.y);
            iter = iter.Next;
            iter.Value.transform.position = pos;
            iter.Value.SetActive(true);
        }

        // 새로 오브젝트를 할당해주는 액션 트리거.
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
        //_PreviousGround.SetActive(false);
        //_PreviousGround = _CurrentGround;
        //_CurrentGround = _NextGround;
        //_NextGround = null;

        //int idx = Random.Range(0, _GroundList.Count);
        //Vector2 pos = new Vector2(0, GroundPosY);

        //_NextGround = await CreateGround(pos, _GroundList[idx].name);
        //pos = new Vector2(SpawnPosMath(_CurrentGround.GetComponent<BoxCollider2D>(), _NextGround.GetComponent<BoxCollider2D>()), Define.GroundPosY);
        //_NextGround.transform.position = pos;

        //GameObject prevWall = _GameScene.WallObjects[(int)GameScene.Wall.Prev];
        //pos = new Vector2(-SpawnPosMath(_PreviousGround.GetComponent<BoxCollider2D>(), prevWall.GetComponent<BoxCollider2D>(), -1), 0);
        //prevWall.transform.position = pos;

        //GameObject frontWall = _GameScene.WallObjects[(int)GameScene.Wall.Front];
        //pos = new Vector2(SpawnPosMath(_NextGround.GetComponent<BoxCollider2D>(), frontWall.GetComponent<BoxCollider2D>()), 0);
        //frontWall.transform.position = pos;

        //테스트용 몬스터 생성.
        //await Managers.Object.InstantiateAsync("TestMonster", _NextGround.transform.position);

    }




    #region private
    private float ExtendSize(GameObject go)
    {
        BoxCollider2D box = go.GetComponent<BoxCollider2D>();

        return box.bounds.extents.x;
    }
    float SpawnPosMath(GameObject a, GameObject b, float back = 1)
    {
        BoxCollider2D _a = a.GetComponent<BoxCollider2D>();
        BoxCollider2D _b = b.GetComponent<BoxCollider2D>();

        float x = a.transform.position.x;
        x = (_a.bounds.extents.x + _b.bounds.extents.x) + (x * back);
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
