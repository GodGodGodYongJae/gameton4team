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


    protected  GameObject HPCanvas;
    protected CreatureHPBar creatureHPBar;
    Score score;
    Level level;
    protected override void Awake()
    {
        base.Awake();
        _type = Type.Monster;
        target = Managers.Object.GetSingularObjet(StringData.Player).GetComponent<Player>();
        Managers.Events.AddListener(Define.GameEvent.SpawnMonster, SpawnListen);
        score = FindObjectOfType<Score>();
        level = FindObjectOfType<Level>();  
    }

    protected override void Death()
    {
        score.GetKillScore();
        level.GetExp();
        cts.Cancel();
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


    private async UniTaskVoid CreateHpBar(Action action = null)
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        Vector2 pos = Vector2.zero;
         HPCanvas = await Managers.Object.InstantiateAsync(StringData.HealthBar, pos);
        pos += new Vector2(box.bounds.extents.x + box.bounds.center.x, box.bounds.extents.y + box.bounds.center.y);
        HPCanvas.transform.parent = this.transform;
        HPCanvas.transform.position = pos;
        RectTransform rect = HPCanvas.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;

        creatureHPBar = HPCanvas.GetComponent<CreatureHPBar>();
        action?.Invoke();
    }
}

