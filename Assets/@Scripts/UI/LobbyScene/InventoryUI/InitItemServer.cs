using Cysharp.Threading.Tasks;
using PlayFab.ClientModels;
using Rito.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Scripts.UI.LobbyScene.InventoryUI
{
    public class InitItemServer
    {
        public Inventory _inventory;
        List<ItemInstance> userInventory;
        public InitItemServer(UI_Lobby lobby)
        {
            _inventory = lobby.invenUI.Inventory;
           
            //InitItemSet();
        }
      
        /// <summary>
        /// 아이템을 초기에 서버에 불러와서 배치.
        /// </summary>
       public void InitItemSet()
        {
            _inventory.ClearItem();
            userInventory = Managers.PlayFab.userInventory;
            Debug.Log(userInventory.Count);
            for (int i = 0; i < userInventory.Count; i++)
            {
                int temp = i;
                 Managers.Resource.LoadAsync<ScriptableObject>
                (userInventory[temp].ItemId.ToString(), (success) => {
                   _inventory.Add((ItemData)success,
                       (int)userInventory[temp].RemainingUses);
                });
            }
            
        }
    }
}