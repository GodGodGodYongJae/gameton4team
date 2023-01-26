using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObj/Data/Weapon", fileName = "Weapon_")]
public class WeaponData : ScriptableObject
{

    [SerializeField]
    [Tooltip("Excel데이터")]
    private TextAsset asset;
    [SerializeField]
    [Tooltip("UI에서 보여질 Item 이미지")]
    private Sprite uiImage;
    [SerializeField]
    [Tooltip("UI에서 보여질 이름")]
    private string displayName;


    [SerializeField]
    int attackDmg;
    [SerializeField]
    int attackDealay;
    [SerializeField]
    [Tooltip("이펙트 에니메이션 시간 넣기")]
    int attackDuration;
    [SerializeField]
    GameObject skilEffect;
    [SerializeField]
    Vector2 effectPos;
        public Vector2 EffectPos { get { return effectPos; } }
    public int AttackDamge { get { return attackDmg; } }
    public int AttackDealay { get { return attackDealay; } }
    public int AttackDuration { get { return attackDuration; } }
    public GameObject Effect => skilEffect;

    public Sprite UIImage => uiImage;
    public string DisplayName => displayName;
    public async UniTask<GameObject> EffectSpawnAsync(Vector2 pos)
    {
        GameObject effect = await Managers.Object.InstantiateAsync(skilEffect.name, pos);
        return effect;
    }




    #region DataLoad


    public enum UpgradeType { NewWeapon,AttackDamage, AttackSpeed, end }

    public void LevelUpData(UpgradeType type, int level)
    {
        switch (type)
        {
            case UpgradeType.AttackDamage:
                attackDmg = CSVData[level].AttackDamage;
                break;
            case UpgradeType.AttackSpeed:
                attackDealay = CSVData[level].AttackSpeed;
                break;
        }
    }
    public int GetLevelData(UpgradeType type, int level)
    {
        switch (type)
        {
            case UpgradeType.AttackDamage:
               return CSVData[level].AttackDamage;
            case UpgradeType.AttackSpeed:
                return CSVData[level].AttackSpeed; 
        }

        return 0;
    }
    public void DataInit()
    {
        if (data != null) return;
        AssetLoad();
        for (int i = 0; i < (int)UpgradeType.end; i++)
        {
            LevelUpData((UpgradeType)i, 1);
        }
    }


    public struct WeaponCSVDATA
    {
        public int AttackDamage;
        public int AttackSpeed;
    }
    public List<Dictionary<string, object>> data;
    private Dictionary<int, WeaponCSVDATA> CSVData = new Dictionary<int, WeaponCSVDATA>();
    public void AssetLoad()
    {
        data = CSVReader.Read(asset);
        for (int i = 0; i < data.Count; i++)
        {
            WeaponCSVDATA weaponData = new WeaponCSVDATA();

            weaponData.AttackDamage = (int)data[i]["Item_Attack_Point"];
            weaponData.AttackSpeed = (int)data[i]["Item_Attack_Speed"];

            CSVData.Add(i + 1, weaponData);
        }
    }
    #endregion
}
