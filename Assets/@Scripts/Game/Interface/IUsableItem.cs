using System.Collections;
using UnityEngine;

namespace Assets._Scripts.Game.Interface
{
    interface IUsableItem
    {
        // 아이템 사용 : 성공 여부 리턴
        bool Use();
    }
}