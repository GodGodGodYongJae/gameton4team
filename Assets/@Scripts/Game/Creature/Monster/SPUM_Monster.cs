using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class SPUM_Monster : Monster
{


  [SerializeField]
    public SPUM_SpriteList _root;
    private int _blinkCount = 3;


    List<List<SpriteRenderer>> AllSpriteList;
    bool spriteEnable = true;

    protected override void Awake()
    {
        base.Awake();
        AllSpriteList = _root.AllSpriteList;
    }
    protected override async UniTaskVoid blinkObject()
    {
        for (int i = 0; i < _blinkCount * 2; i++)
        {
            spriteEnable = !spriteEnable;
            spriteListEnable(spriteEnable);
            await UniTask.Delay(150);
        }
    }
    protected override void SpawnListen(Define.GameEvent eventType, Component Sender, object param = null)
    {
        if (eventType == Define.GameEvent.SpawnMonster && (Creature)param == this)
        {
            _hp = _creatureData.MaxHP;
            spriteListEnable(true); // blink 때문에 UniTask Cancle 시 Pooling 되었을 시 꺼짐.
            moveAction?.Invoke();
            CreateHpBar(() => { creatureHPBar.Damage(_hp, _creatureData.MaxHP); }).Forget();
        }
    }

    private void spriteListEnable(bool enable)
    {
        foreach (var item in AllSpriteList)
        {
            List<SpriteRenderer> spriteList = item;
            foreach (var item2 in spriteList)
            {
                item2.enabled = enable;
            }
        }
    }
}

