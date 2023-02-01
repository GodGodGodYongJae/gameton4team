using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObj/Data/Item", fileName = "Item_")]
public class ItemData : ScriptableObject
{
    [SerializeField]
    [Tooltip("ȭ�鿡 ������ �̸�")]
    private string displayName;
    [SerializeField]
    private Item item;


    public string Name => displayName;
    public Item Items => item;

    public void Run()
    {
        BackEnd();
        item.Run();
    }

    private void BackEnd()
    {

    }
}
