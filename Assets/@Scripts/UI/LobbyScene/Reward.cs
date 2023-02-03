using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Scripts.UI.LobbyScene
{
    public class Reward : MonoBehaviour
    {
        Button btn;
        private void Start()
        {
          btn = GetComponent<Button>();
        }
        public void Init(bool isCheck, ScriptableObject rewards)
        {
            
        }

    }
}