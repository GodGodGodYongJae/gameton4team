using Assets._Scripts.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Scripts.UI.GameScene
{
    class ItemInventoryButton : MonoBehaviour
    {
        public ItemSlotController.ItemSlot Slot;
        public Image Fillimage;
        private float coolTime = 30f;
        private float CurrentCoolTime = 0f;
        private TextMeshProUGUI coolText;  
        public void ItemUse()
        {
            CurrentCoolTime = coolTime;
            StartCoroutine(CoolTime());
        }

       
        public bool IsCoolTime => CurrentCoolTime > 1.0f;
       
        
        IEnumerator CoolTime()
        {
            Fillimage.gameObject.SetActive(true);
            if (coolText == null)
                coolText = Fillimage.gameObject.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            
            while(CurrentCoolTime > 1.0f)
            {
                CurrentCoolTime -= Time.deltaTime;
                coolText.text = Math.Round(CurrentCoolTime).ToString();
                Fillimage.fillAmount = (CurrentCoolTime / coolTime);
                yield return new WaitForFixedUpdate();
            }

            Fillimage.gameObject.SetActive(false);
        }
    }
}
