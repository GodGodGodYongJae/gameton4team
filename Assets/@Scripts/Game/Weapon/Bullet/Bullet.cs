using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Scripts.Game.Weapon
{
    public abstract class Bullet : MonoBehaviour
    {
        protected bool isInit = false;
        protected PolygonCollider2D box;
        protected virtual void Awake()
        {
            // 자식한테 보통 이펙트가 있음 만약 해당 이펙트에 자식이 없거나 이상하다면
            // 이거 Bullet 자체를 부모 클래스로 빼서, Child를 일일히 지정해줘야할듯..?
            PolygonCollider2D childBox = this.gameObject.transform.GetChild(0)
            .gameObject.GetOrAddComponent<PolygonCollider2D>();
            childBox.isTrigger = true;
            this.box = gameObject.GetOrAddComponent<PolygonCollider2D>();
            //this.box.size = childBox.size;
            this.box.isTrigger = true;
        }
        protected abstract UniTask Duration();
        protected abstract void RemoveEffect();


    }
}