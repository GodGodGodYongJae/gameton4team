using System.Collections;
using UnityEngine;

namespace Assets._Scripts.Game.Items
{
    public abstract class CountableItemData : ItemData
    {
        public int MaxAmount => _maxAmount;
        [SerializeField] private int _maxAmount = 99;
    }
}