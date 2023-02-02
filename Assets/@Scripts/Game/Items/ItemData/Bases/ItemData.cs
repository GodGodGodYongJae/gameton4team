using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu(menuName = "ScriptableObj/Data/Item", fileName = "Item_")]
public abstract class ItemData : ScriptableObject
{
    public int ID => _id;
    public string Key => _key;
    public string Name => _name;
    public string Tooltip => _tooltip;
    public Sprite IconSprite => _iconSprite;

    [SerializeField] private int _id;
    [SerializeField] private string _key; //서버에서 불러올 Key 값 
    [SerializeField] private string _name;    // 아이템 이름
    [Multiline]
    [SerializeField] private string _tooltip; // 아이템 설명
    [SerializeField] private Sprite _iconSprite; // 아이템 아이콘

    /// <summary> 타입에 맞는 새로운 아이템 생성 </summary>
    public abstract Item CreateItem();
}
