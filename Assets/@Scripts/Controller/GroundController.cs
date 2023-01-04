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

    Ground _ground;
    // 생성할 그라운드 리스트.
    IReadOnlyList<GameObject> GroundList;
    GameObject prevWall;
    GameObject frontWall;

    // 현재 생성된 그라운드 링크드 리스트 처음 끝 삭제가 빈번히 발생하기 때문에 링크드 리스트 활용.
    LinkedList<GameObject> _Grounds = new LinkedList<GameObject>();
    // 이터레이터 같은 역할.
  
    // 생성자 GameScene DI 주입.
    public GroundController(GameScene gameScene, IReadOnlyList<GameObject> GroundList)
    {
        _GameScene = gameScene;
        this.GroundList = GroundList;

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
    public async UniTaskVoid Init()
    {
        int idx = 0;
        Vector2 pos = new Vector2(0, Define.GroundPosY);
        await CreateGround(pos, GroundList[idx].name);
        LinkedListNode<GameObject> iter = _Grounds.First;

        GameObject go;
        for (int i = 1; i < Define.PoolGroundSize; i++)
        {
            go = await CreateGround(pos, GroundList[idx].name);
            pos = new Vector2(SpawnPosMath(iter.Value, iter.Next.Value),pos.y);
            go.transform.position = pos;
            iter = iter.Next;
            //idx = Random.Range(0, GroundList.Count);
        }
        WallPosSet();
        CurrentGroundIdx();
        // 새로 오브젝트를 할당해주는 액션 트리거.
        Managers.FixedUpdateAction += CheckNextBound;
    }

    void CheckNextBound()
    {
        float extendSize = ExtendSize(_NextGround);
        float pointCheck = _NextGround.transform.position.x + extendSize - 1;
        if (pointCheck <= _GameScene.PlayerGo.transform.position.x)
        {
       
            _ground = _NextGround.GetComponent<Ground>();
            if (_ground.MonsterCount == 0 || _ground.isHaveGenerator.Equals(false))
            {
                PushNextGround().Forget();
            }
        }

        
    }
    async UniTaskVoid PushNextGround()
    {
        CurrentGroundIdx();
        _PreviousGround.SetActive(false);
        _Grounds.RemoveFirst();


        int idx = Random.Range(0, GroundList.Count);
        LinkedListNode<GameObject> iter = _Grounds.Last;
        Vector2 pos = iter.Value.transform.position;
        GameObject go = await CreateGround(pos, GroundList[idx].name);
        pos = new Vector2(SpawnPosMath(iter.Value, iter.Next.Value), pos.y);
        go.transform.position = pos;

        WallPosSet();
        _ground = _NextGround.GetComponent<Ground>();
        Debug.Log(_ground.gameObject.name);
        _ground.SpawnMonster().Forget();
    }

    void WallPosSet()
    {
        CurrentGroundIdx();
        Vector2 pos = Vector2.zero;

       frontWall = _GameScene.WallObjects[(int)GameScene.Wall.Front];
        pos = new Vector2(SpawnPosMath(_NextGround, frontWall), pos.y);
        frontWall.transform.position = pos;
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
        _Grounds.AddLast(go);
        callback?.Invoke(UniTask.CompletedTask);
        return go;
    }
    #endregion




}
