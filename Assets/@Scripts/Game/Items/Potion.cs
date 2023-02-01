using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets._Scripts.Game.Items
{
    [CreateAssetMenu(menuName = "ScriptableObj/Item/Potion", fileName = "Potion_")]

    public class Potion : Item
    {
        [SerializeField]
        [Tooltip("얼마만큼 회복할 것 인지 최대체력의 %로 감.")]
        [Range(0,100)]
        private float healAmmount; 

        public override void Run()
        {
            Player player = new Player(); // 이거 꼭 바꿔야 함.
            float heal = player.PlayerData.MaxHP * (healAmmount*0.01f);
            player.AddHp(healAmmount);
        }
    }
}