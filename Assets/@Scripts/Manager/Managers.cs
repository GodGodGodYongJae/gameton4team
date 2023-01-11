using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class Managers : Singleton<Managers>
{

    ResourceManager _resource = new ResourceManager();
    UIManager _ui = new UIManager();
    SceneManagerEx _scene = new SceneManagerEx();
    ObjectManager _object = new ObjectManager();
    EventManager _event = new EventManager();
    MonsterManager _monster = new MonsterManager();
    public static ResourceManager Resource { get { return Instance?._resource; } }
    public static UIManager UI { get { return Instance?._ui; } }
    public static SceneManagerEx Scene { get { return Instance?._scene; } }
    public static ObjectManager Object { get { return Instance?._object; } }
    public static EventManager Events { get { return Instance?._event; } }
    public static MonsterManager Monster { get { return Instance?._monster; } }
    public static Action UpdateAction { get; set; }
    public static Action FixedUpdateAction { get; set; }

    protected override void Awake()
    {
        base.Awake();
        this._monster.Init();
    }

    public static void Init()
    {
        bool isInit = Instance;
    }

    private void Update()
    {
        UpdateAction?.Invoke();
    }
    private void FixedUpdate()
    {
        FixedUpdateAction?.Invoke();
    }
}
