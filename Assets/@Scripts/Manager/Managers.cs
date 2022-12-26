using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class Managers : Singleton<Managers>
{

    ResourceManager _resource = new ResourceManager();
    UIManager _ui = new UIManager();
    SceneManagerEx _scene = new SceneManagerEx();
    ObjectManager _object = new ObjectManager();

    public Action _updateAction;
    public static ResourceManager Resource { get { return Instance?._resource; } }
    public static UIManager UI { get { return Instance?._ui; } }
    public static SceneManagerEx Scene { get { return Instance?._scene; } }
    public static ObjectManager Object { get { return Instance?._object; } } 

    public static Action UpdateAction { get; set; }
    protected override void Awake()
    {
        base.Awake();
    }

    public static void Init()
    {
        bool isInit = Instance;
    }

    private void Update()
    {
        UpdateAction?.Invoke();
    }
}
