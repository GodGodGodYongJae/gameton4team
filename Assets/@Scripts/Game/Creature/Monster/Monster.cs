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
    protected CreatureHPBar creatureHPBar;
    protected override void Awake()
    {
        base.Awake();
        _type = Type.Monster;
        target = GameObject.Find(StringData.Player).GetComponent<Player>();
        CreateHpBar(EventRegister).Forget();

     
    }

    protected virtual void SpawnListen(Define.GameEvent eventType, Component Sender, object param = null)
    {
        if (eventType == Define.GameEvent.SpawnMonster && (Creature)param == this)
        {
            _hp = _creatureData.MaxHP;
            moveAction?.Invoke();
            creatureHPBar.Damage(_hp, _creatureData.MaxHP);
        }
    }

    private void EventRegister()
    {
        Managers.Events.AddListener(Define.GameEvent.SpawnMonster, SpawnListen);
        //초기에는 자기 자신을 불러줘야한다.
        Managers.Events.PostNotification(Define.GameEvent.SpawnMonster, this, this);
    }

    private async UniTaskVoid CreateHpBar(Action callback)
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        Vector2 pos = Vector2.zero;
        GameObject HPCanvas = await Managers.Object.InstantiateAsync(StringData.HealthBar, pos);
        pos += new Vector2(box.bounds.extents.x + box.bounds.center.x, box.bounds.extents.y + box.bounds.center.y);
        HPCanvas.transform.parent = this.transform;
        HPCanvas.transform.position = pos;
        RectTransform rect = HPCanvas.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;

        creatureHPBar = HPCanvas.GetComponent<CreatureHPBar>();
        
        callback?.Invoke();

    }
}

