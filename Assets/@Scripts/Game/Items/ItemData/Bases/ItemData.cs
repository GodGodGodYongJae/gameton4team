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
    public string ItemAdmobCode => _itemShopAdmobCode;

    [SerializeField] private int _id;
    [SerializeField] private string _key; //�������� �ҷ��� Key �� 
    [SerializeField] private string _name;    // ������ �̸�
    [Multiline]
    [SerializeField] private string _tooltip; // ������ ����
    [SerializeField] private Sprite _iconSprite; // ������ ������
    [SerializeField] private string _itemShopAdmobCode; // ������ ������ �ڵ�
    /// <summary> Ÿ�Կ� �´� ���ο� ������ ���� </summary>
    public abstract Item CreateItem();

}
