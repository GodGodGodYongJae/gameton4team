using Assets._Scripts.Manager;
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
    PlayFabManager _playfab = new PlayFabManager();
    AdmobManager _admob;
    SoundManager _sound;
    public static ResourceManager Resource { get { return Instance?._resource; } }
    public static UIManager UI { get { return Instance?._ui; } }
    public static SceneManagerEx Scene { get { return Instance?._scene; } }
    public static ObjectManager Object { get { return Instance?._object; } }
    public static EventManager Events { get { return Instance?._event; } }
    public static MonsterManager Monster { get { return Instance?._monster; } }
    public static PlayFabManager PlayFab { get { return Instance?._playfab; } }
    public static Action UpdateAction { get; set; }
    public static Action FixedUpdateAction { get; set; }
    public static SoundManager Sound { get { return Instance?._sound; } }
    public static AdmobManager Admob { get { return Instance?._admob; } }
    public static void OnDestorys() 
    {
        FixedUpdateAction = null;
        UpdateAction = null;
        Object.RemoveAll();
        Events.RemoveAll();
        Monster.Clear();
        //Resource.Clear();


    }

    protected override void Awake()
    {
        base.Awake();
        //사운드 매니저 할당 & Admob은 Mono로 만들어졌기 때문에.
        _sound = gameObject.GetOrAddComponent<SoundManager>();
        _admob = gameObject.GetOrAddComponent<AdmobManager>();
    }

    public static void Init()
    {
        bool isInit = Instance;
    }

    private void Update()
    {
        if(Time.timeScale > 0)
            UpdateAction?.Invoke();
    }
    private void FixedUpdate()
    {
        if (Time.timeScale > 0)
            FixedUpdateAction?.Invoke();
    }

}
