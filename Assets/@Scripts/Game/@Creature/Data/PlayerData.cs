using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObj/Data/Player", fileName = "Player_")]
public class PlayerData : CreatureData
{


    [SerializeField]
    [Tooltip("Excel데이터")]
    private TextAsset asset;

    [SerializeField]
    [Tooltip("점프력")]
    private float _jumpForce;
    [SerializeField]
    [Tooltip("피격후 무적시간")]
    private int _invinvibilityDuration;

    private int experiencePoint;
    private int level;
    private float criticalDamage;
    private float criticalProbaility;
    public float JumpForce => _jumpForce;
    public int InvinvibilityDuration => _invinvibilityDuration;

    public int Level => level;
    public int ExperiencePoint => experiencePoint;
    public float CriticalDamage => criticalDamage;
    public float CriticalProbaility => criticalProbaility;

    #region DataLoad

    public void LevelUp()
    {
        if (level + 1 > maxLevel) return;
        level++;
        this.attackDamage = CSVData[level].AttackDamage;
        this._maxhp = (int)CSVData[level].HealthPoint;
        this.criticalDamage = CSVData[level].CriticalDamage;
        this.criticalProbaility = CSVData[level].CriticalProbaility;
        this.experiencePoint = CSVData[level].ExperiencePoint;
        // 최대 경험치 예외처리 필요
        if (experiencePoint == -1)
        {
            this.experiencePoint = int.MaxValue;
        }
            
    }

    private struct CharacterCSVDATA 
    {
        public int ExperiencePoint;
        public float CriticalDamage;
        public float CriticalProbaility;
        public float HealthPoint;
        public float AttackDamage;
    }
    private int maxLevel = 0;
    private List<Dictionary<string, object>> data;
    private Dictionary<int, CharacterCSVDATA> CSVData = new Dictionary<int, CharacterCSVDATA>();
    public void AssetLoad()
    {
        CSVData.Clear();
        data = CSVReader.Read(asset);

        for (int i = 0; i < data.Count; i++)
        {
            CharacterCSVDATA characterData = new CharacterCSVDATA();
            characterData.ExperiencePoint = (int)data[i]["Char_Experience_Point"];
            characterData.HealthPoint = float.Parse(data[i]["Char_Health_Point"].ToString());
            characterData.AttackDamage = float.Parse(data[i]["Char_Attack"].ToString());
            characterData.CriticalProbaility = float.Parse(data[i]["Char_Critical_Probability"].ToString());
            CSVData.Add(i + 1, characterData);
        }
        maxLevel = (int)data[data.Count - 1]["Char_Level"];
        level = 0;
    }
    #endregion
}
