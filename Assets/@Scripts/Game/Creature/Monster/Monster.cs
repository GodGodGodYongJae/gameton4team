using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : Creature
{
    protected Creature target;
    protected delegate UniTaskVoid MoveAction();
    protected MoveAction moveAction;


    protected GameObject HPCanvas;
    protected CreatureHPBar creatureHPBar;

    protected MonsterData monsterData;
    public MonsterData MonsterData => monsterData;


    protected override void Awake()
    {
        base.Awake();
        _type = Type.Monster;
        target = Managers.Object.GetSingularObjet(StringData.Player).GetComponent<Player>();
        Managers.Events.AddListener(Define.GameEvent.SpawnMonster, SpawnListen);
    }

    protected override void Death()
    {
        cts.Cancel();
        Managers.Object.ReturnToParent(HPCanvas);
        Managers.Events.PostNotification(Define.GameEvent.monsterDestroy, this);
        base.Death();
    }

    protected virtual void SpawnListen(Define.GameEvent eventType, Component Sender, object param = null)
    {
        if (eventType == Define.GameEvent.SpawnMonster && (Creature)param == this)
        {
            _hp = _creatureData.MaxHP;
            _sprite.enabled = true; // blink 때문에 UniTask Cancle 시 Pooling 되었을 시 꺼짐.
            moveAction?.Invoke();
            CreateHpBar(() => { creatureHPBar.Damage(_hp, _creatureData.MaxHP); }).Forget();
        }
    }


    protected async UniTaskVoid CreateHpBar(Action action = null)
    {
        Collider2D box = GetComponent<Collider2D>();
        Vector2 pos = Vector2.zero;
        HPCanvas = await Managers.Object.InstantiateAsync(StringData.HealthBar, pos);
        //pos += new Vector2(box.bounds.extents.x + box.bounds.center.x, box.bounds.extents.y + box.bounds.center.y);
        HPCanvas.transform.parent = this.transform;
        HPCanvas.transform.position = pos;
        RectTransform rect = HPCanvas.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, pos.y + (box.bounds.extents.y*2));
        creatureHPBar = HPCanvas.GetComponent<CreatureHPBar>();
        action?.Invoke();
    }
}