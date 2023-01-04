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
    // ������ �׶��� ����Ʈ.
    IReadOnlyList<GameObject> GroundList;
    GameObject prevWall;
    GameObject frontWall;

    // ���� ������ �׶��� ��ũ�� ����Ʈ ó�� �� ������ ����� �߻��ϱ� ������ ��ũ�� ����Ʈ Ȱ��.
    LinkedList<GameObject> _Grounds = new LinkedList<GameObject>();
    // ���ͷ����� ���� ����.
  
    // ������ GameScene DI ����.
    public GroundController(GameScene gameScene, IReadOnlyList<GameObject> GroundList)
    {
        _GameScene = gameScene;
        this.GroundList = GroundList;

    }
   
    // Linked List�� ��ϵ� Element���� �� Ground�� ����ȭ ���ִ� �޼���.
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
 

    // ó�� ���� ����.
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
        // ���� ������Ʈ�� �Ҵ����ִ� �׼� Ʈ����.
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
