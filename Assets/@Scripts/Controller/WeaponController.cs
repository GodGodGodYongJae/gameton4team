using Cysharp.Threading.Tasks;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    //어드레서블, Weapon 스크립트 이름 enum값에 맞춰서 생성.
    //데이터 enum +_data 이름 값으로 생성.
    public enum WeaponType
    {
        Weapon_Sword,
        Weapon_Aex
    }

    public SPUM_SpriteList root { get; set; }
    private GameObject rHandGo;

    public void Init()
    {
        rHandGo =  root._weaponList[0].gameObject;
        
    }
    //
    public async UniTaskVoid WeaponChange(WeaponType type)
    {

        // await Managers.Object.InstantiateSingle("Sword", Vector2.zero, rHandGo);
        bool registered = false;
        SpriteRenderer spriteRenderer = rHandGo.GetComponent<SpriteRenderer>();
        BoxCollider2D box = rHandGo.GetComponent<BoxCollider2D>();
        if (box != null) Destroy(box);

        Weapon weapon = rHandGo.GetComponent<Weapon>();
        if (weapon != null) Destroy(weapon);

        Managers.Resource.LoadAsync<Sprite>(type.ToString(), (success) =>
        {
            spriteRenderer.sprite = success;
            box = rHandGo.AddComponent<BoxCollider2D>();
            registered = true;
        });
        await UniTask.WaitUntil(() => { return registered == true; });
        registered = false;
        //스크립트 이름만 수정 하자... 
        weapon = rHandGo.AddComponent<Sword>();

        Managers.Resource.LoadAsync<ScriptableObject>(type.ToString()+"_data", (succss) =>
        {
            weapon.weaponData = (WeaponData)succss;
            registered = true;
            
        });
        await UniTask.WaitUntil(() => { return registered == true; });
        weapon.Attack().Forget();
    }
}