using Cysharp.Threading.Tasks;
using MonsterLove.StateMachine;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SPUM_Monster : Monster
{
   protected enum States
    {
        IDLE,
        MOVE,
        ATTACK,
        DEATH
    }


    [SerializeField]
    protected SPUM_Prefabs sPUM_Prefab;


    [SerializeField]
    //ms 단위,
    protected int attackAnimSync = 0;


    protected SPUM_SpriteList _root;
    private int _blinkCount = 3;

    protected CancellationTokenSource movects = new CancellationTokenSource();
    List<List<SpriteRenderer>> AllSpriteList;
    bool spriteEnable = true;

    protected StateMachine<States> fsm;

    protected Collider2D Attackbox;

    protected Color blinkRedColor = Color.red;
    protected Color blinkClearColor = Color.white;
    protected override void Awake()
    {
        base.Awake();
        monsterData = (MonsterData)_creatureData;
        _root = sPUM_Prefab._spriteOBj;
        AllSpriteList = _root.AllSpriteList;
   
    }
    protected void Update()
    {
        fsm.Driver.Update.Invoke();
    }


    protected override async UniTaskVoid blinkObject()
    {
        for (int i = 0; i < _blinkCount * 2; i++)
        {
            spriteEnable = !spriteEnable;
            spriteListEnable(spriteEnable);
            await UniTask.Delay(150, cancellationToken: cts.Token);
        }
        spriteListEnable(true);
    }
    protected override void SpawnListen(Define.GameEvent eventType, Component Sender, object param = null)
    {
        if (eventType == Define.GameEvent.SpawnMonster && (Creature)param == this)
        {
            _hp = _creatureData.MaxHP;
            spriteListEnable(true); // blink 때문에 UniTask Cancle 시 Pooling 되었을 시 꺼짐.
            
            if(Attackbox != null)
                Attackbox.enabled = false;
            
            CreateHpBar(() => { creatureHPBar.Damage(_hp, _creatureData.MaxHP); }).Forget();
            fsm.ChangeState(States.IDLE);
            
        }
    }
    public override void Damage(float dmg, Creature Target)
    {
        base.Damage(dmg, Target);
        creatureHPBar.Damage(_hp, _creatureData.MaxHP);
    }

    protected void spriteListEnable(bool enable)
    {
        foreach (var item in AllSpriteList)
        {
            List<SpriteRenderer> spriteList = item;
            foreach (var item2 in spriteList)
            {
                item2.color = returnToBlinkColor(enable);
            }

        }
    }

    private Color returnToBlinkColor(bool cur)
    {
        return cur ?  blinkClearColor:blinkRedColor;
    }
}

