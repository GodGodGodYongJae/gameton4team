using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Random = UnityEngine.Random;

public class GroundController
{
    Player player;

    GameObject previousGround;
    GameObject currentGround;
    GameObject nextGround;

    GameScene gameScene;

    Ground ground;
    // 생성할 그라운드 리스트.
    IReadOnlyList<GameObject> groundList;
    public static float chatperSize = 0;
    GameObject frontWall;
    // 현재 생성된 그라운드 링크드 리스트 처음 끝 삭제가 빈번히 발생하기 때문에 링크드 리스트 활용.
    LinkedList<GameObject> grounds = new LinkedList<GameObject>();

    GroundGenerator GroundGenerator;
    bool isBossSpawned = false;

    // 생성자 GameScene DI 주입.
    public GroundController(GameScene gameScene, GroundGenerator GroundGenerator)
    {
        this.gameScene = gameScene;
        this.GroundGenerator = GroundGenerator;
        ChangeGroundGenerator(GroundGenerator);
        Managers.Events.AddListener(Define.GameEvent.stageClear, StageClear);
    }


    // 처음 지형 생성.
    public async UniTaskVoid Init(Action callback = null)
    {
        int idx = 0;
        Vector2 pos = new Vector2(0, Define.GroundPosY);
        await CreateGround(pos, groundList[idx].name);
        LinkedListNode<GameObject> iter = grounds.First;

        GameObject go;
        for (int i = 1; i < Define.PoolGroundSize; i++)
        {
            go = await CreateGround(pos, groundList[idx].name);
            pos = new Vector2(SpawnPosMath(iter.Value, iter.Next.Value),pos.y);
            go.transform.position = pos;
            iter = iter.Next;
        }
        
        WallPosSet();
        CurrentGroundIdx();
        foreach (var item in grounds)
        {
           chatperSize -= ExtendSize(item) * 2;
        }

        float initPosX = this.grounds.First.Next.Next.Value.transform.position.x;
        float initPosY = this.grounds.First.Next.Next.Value.transform.position.y;
        gameScene.Player.InitPosition(initPosX,initPosY+2);
        CameraController cam = Camera.main.GetComponent<CameraController>();
        cam.SetPositionX(initPosX);
        Managers.FixedUpdateAction += CheckNextBound;

        callback?.Invoke();
    }



    void CheckNextBound()
    {
        float extendSize = ExtendSize(nextGround);
        float pointCheck = nextGround.transform.position.x + extendSize - Define.NextWallSence;
        if (pointCheck <= gameScene.PlayerGo.transform.position.x)
        {
            if (chatperSize > 0)
            {
                PushNextGround().Forget();
                return;
            }
  
            else if (Managers.Monster.GetSpawnMonsterCount == 0 )
            {
                if(isBossSpawned == false)
                {
                    isBossSpawned = true;
                    PushNextGround().Forget();
                }
                else
                {
                    isBossSpawned = false;
                    WallPosSet();
                    GameObject go = grounds.Last.Value;
                    go.GetComponent<Ground>().SpawnMonster().Forget();
                    Managers.FixedUpdateAction -= CheckNextBound;
                    
                }

            }
        }
  
    }
    async UniTaskVoid PushNextGround()
    {
        CurrentGroundIdx();
        previousGround.SetActive(false);
        grounds.RemoveFirst();

        GameObject.Find("ScoreText").GetComponent<Score>().GetDistanceScore();

        int idx = (isBossSpawned == false) ?RandomGroundIdx() : FindBossGroundIdx();

        LinkedListNode<GameObject> iter = grounds.Last;
        Vector2 pos = iter.Value.transform.position;
        GameObject go = await CreateGround(pos, groundList[idx].name);
        pos = new Vector2(SpawnPosMath(iter.Value, iter.Next.Value), pos.y);
        go.transform.position = pos;
        chatperSize -= ExtendSize(go);

        WallPosSet();
        ground = grounds.Last.Previous.Value.GetComponent<Ground>();
        ground.SpawnMonster().Forget();
    }

    void WallPosSet()
    {
        CurrentGroundIdx();
        Vector2 pos = Vector2.zero;
        frontWall = gameScene.WallObjects[(int)GameScene.Wall.Front];
        LinkedListNode<GameObject> iter =(isBossSpawned == false) ? grounds.Last : grounds.Last.Previous;
        pos = new Vector2(SpawnPosMath(iter.Value, frontWall), pos.y);
        frontWall.transform.position = pos;
    }
    public void ClearPrevStageData()
    {
        grounds.Clear();
        foreach (var item in this.GroundGenerator.Grounds)
        {
            Managers.Object.RemoveObjectPool(item.name);
        }
    }
    #region private

    private void ChangeGroundGenerator(GroundGenerator GroundGenerator)
    {
        this.groundList = GroundGenerator.Grounds;
        chatperSize = GroundGenerator.ChatperSize;
    }

    // Linked List에 등록된 Element들을 각 Ground에 동기화 해주는 메서드.
    private void CurrentGroundIdx()
    {
        if (grounds.Count < 3) return;
        LinkedListNode<GameObject> GroundNode = grounds.First;
        previousGround = GroundNode.Value;

        GroundNode = GroundNode.Next;
        currentGround = GroundNode.Value;

        GroundNode = GroundNode.Next;
        nextGround = GroundNode.Value;
    }

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

    int RandomGroundIdx()
    {
        int idx = 0;
        Ground ground = null;
        do
        {
            idx = Random.Range(0, groundList.Count - 1);
            ground = groundList[idx].GetComponent<Ground>();
        }
        while (ground.isBossGround.Equals(true));
        return idx;
    }

    int FindBossGroundIdx()
    {
        for (int i = 0; i  < groundList.Count; i++)
        {
            Ground ground = groundList[i].GetComponent<Ground>();
            if(ground.isBossGround == true)
            {
                return i;
            }
        }
        return 0;
    }

    async UniTask<GameObject> CreateGround(Vector2 pos, string name, Action<UniTask> callback = null)
    {
        GameObject go = await Managers.Object.InstantiateAsync(name, pos);
        grounds.AddLast(go);
        callback?.Invoke(UniTask.CompletedTask);
        return go;
    }

    //스테이지 클리어 리스너
    private void StageClear(GameEvent eventType, Component Sender, object param)
    {
        if (Define.GameEvent.stageClear == eventType && Utils.EqualSender<GameScene>(Sender))
        {
            GroundGenerator = (GroundGenerator)param; 
            this.ChangeGroundGenerator(GroundGenerator);
            CurrentStage.stage();
            Managers.Sound.PlaySFX("Weapon");
            Init(() => { Managers.Object.GetSingularObjet("coin").gameObject.SetActive(true); }).Forget();  

        }
        
    }

 
    #endregion




}
