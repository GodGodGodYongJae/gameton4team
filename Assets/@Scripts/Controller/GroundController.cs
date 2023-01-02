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

    // ������ �׶��� ����Ʈ.
    IReadOnlyList<GameObject> _GroundList;

    // ���� ������ �׶��� ��ũ�� ����Ʈ ó�� �� ������ ����� �߻��ϱ� ������ ��ũ�� ����Ʈ Ȱ��.
    LinkedList<GameObject> _Grounds = new LinkedList<GameObject>();
    // ���ͷ����� ���� ����.
  
    // ������ GameScene DI ����.
    public GroundController(GameScene gameScene)
    {
        _GameScene = gameScene;
    }
    //���� �׶��� ������. ��ũ�� ����Ʈ�� 0 ��°�� Prev �̱� ������, +1 ��.
    public float GetCurrentGroundPos() {
        CurrentGroundIdx();
        if (_CurrentGround == null || _PreviousGround == null || _NextGround == null) return 0;
        return _CurrentGround.transform.position.x;
    } 
    // Prev , Current , Next Box Size�� ���ؼ�, ī�޶� ���ѿ��� ǥ�ø� ���� �޼���.
    public float GetExtendSize()
    {
        CurrentGroundIdx();
        if (_CurrentGround == null || _PreviousGround == null || _NextGround == null) return 0;
        return ExtendSize(_PreviousGround)+ExtendSize(_CurrentGround)+ExtendSize(_NextGround);
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
    public async UniTaskVoid Init(IReadOnlyList<GameObject> GroundList)
    {

        //_GroundList = GroundList;
        ////���Ƿ� idx�� �޾ƿ´�.
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
            // �ٽ� �������� ����.
            idx = Random.Range(0, GroundList.Count);
        }
        //�ʹ� Prev,Current�� �׳� ������.
        LinkedListNode<GameObject> iter = _Grounds.First.Next;
        Vector2 pos = new Vector2(0, GroundPosY);
        iter.Value.transform.position = pos;
        iter.Value.SetActive(true);

        //�ʹ� Prev,Current�� �׳� ������.
        while (iter.Next != null)
        {
            //������ ���� ����ְ�, iter�� �������� �ű��, iter ���� ���� ����.
            pos = new Vector2(SpawnPosMath(iter.Value, iter.Next.Value), pos.y);
            iter = iter.Next;
            iter.Value.transform.position = pos;
            iter.Value.SetActive(true);
        }

        // ���� ������Ʈ�� �Ҵ����ִ� �׼� Ʈ����.
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

        //�׽�Ʈ�� ���� ����.
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
