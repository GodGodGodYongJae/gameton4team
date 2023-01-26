using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class WeaponController
{
    //어드레서블, Weapon 스크립트 이름 enum값에 맞춰서 생성.
    //데이터 enum +_data 이름 값으로 생성.
    // 2023-01-26 WeaponType은 Define으로 빠짐.

    public SPUM_SpriteList root { get; set; }
    private GameObject rHandGo;
    private GameScene gameScene;
    public WeaponController(GameScene gameScene)
    {
        this.gameScene = gameScene;
        root = gameScene.Player._root;
        rHandGo =  root._weaponList[2].gameObject;
        WeaponChange(Define.WeaponType.Weapon_Sword).Forget();
        Managers.Events.AddListener(Define.GameEvent.ChangeWeapon, ChangeWeapon);
    }

    private void ChangeWeapon(Define.GameEvent eventType, Component Sender, object param)
    {
        if(eventType == Define.GameEvent.ChangeWeapon)
        {
            int weaponParam = (int)param;
            WeaponChange((Define.WeaponType)weaponParam).Forget();
        }

    }

    public async UniTaskVoid WeaponChange(Define.WeaponType type)
    {
        bool registered = false;
        SpriteRenderer spriteRenderer = rHandGo.GetComponent<SpriteRenderer>();
        BoxCollider2D box = rHandGo.GetComponent<BoxCollider2D>();
        if (box != null) Object.Destroy(box);

        Weapon weapon = rHandGo.GetComponent<Weapon>();
        if (weapon != null)
        {
            weapon.ChangeWeaponFixedUpdateDelete();
            Managers.Resource.Release(type.ToString());
            Managers.Resource.Release(type.ToString()+"_data");
            Object.Destroy(weapon);
        } 

        Managers.Resource.LoadAsync<Sprite>(type.ToString(), (success) =>
        {
            spriteRenderer.sprite = success;
            box = rHandGo.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            registered = true;
        });
        await UniTask.WaitUntil(() => { return registered == true; });
        registered = false;
        

        Type t = Type.GetType(type.ToString());
        rHandGo.AddComponent(t);

        weapon = rHandGo.GetComponent<Weapon>();

        Managers.Resource.LoadAsync<ScriptableObject>(type.ToString()+"_data", (succss) =>
        {
            weapon.weaponData = (WeaponData)succss;
            registered = true;
            
        });
        await UniTask.WaitUntil(() => { return registered == true; });
         weapon.RegsiterEffect().Forget();
    }
}