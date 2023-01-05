using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestMonster : Creature, IListener
{
    Creature _target;
    Rigidbody2D _rigid;

    [System.Obsolete]
    private void Start()
    {
        _type = Type.Monster;
        _rigid = GetComponent<Rigidbody2D>();
        _target = GameObject.Find(StringData.Player).GetComponent<Player>();
        CreateHpBar().Forget();
        Managers.Events.AddListener(Define.GameEvent.SpawnMonster, Listene);
    }

    GameObject _HPCanvas;
    Image _hpImg;

    [System.Obsolete]
    async UniTaskVoid CreateHpBar()
    {
        //하드 코딩된 부분 추후 바꿔줘야함. 현재는 프로토타입 테스트를 위해 임시적으로 만듬 이거 나중에 CreatureHPBar 에서 관리해야 함.
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        Vector2 pos = Vector2.zero;
        _HPCanvas = await Managers.Object.InstantiateAsync(StringData.HealthBar, pos);
        pos += new Vector2(box.bounds.extents.x + box.bounds.center.x, box.bounds.extents.y + box.bounds.center.y);
        _HPCanvas.transform.parent = this.transform;
        _HPCanvas.transform.position = pos;
        RectTransform rect = _HPCanvas.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        
        _HPCanvas = _HPCanvas.transform.FindChild("Health Bar Fill").gameObject;
        _hpImg = _HPCanvas.GetComponent<Image>();

    }
    public override void Damage(int dmg, Creature Target)
    {
        base.Damage(dmg, Target);
        if (transform.gameObject.activeSelf == false) return;
        _hpImg.fillAmount = (float)_hp / (float)_creatureData.MaxHP;
    }
    public override void Death()
    {
        Managers.Events.PostNotification(Define.GameEvent.monsterDestroy, this);
        base.Death();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Creature creature = collision.gameObject.GetComponent<Creature>();
        if (creature == null) return;

        if (creature.GetType() == Creature.Type.Player)
        {
            creature.Damage(_creatureData.AttackDamage, this);
        }

    }
  
    async UniTaskVoid MoveDelay()
    {
        float moveTime = 0;
        while (moveTime < 0.8f)
        {
            if (transform.gameObject.activeSelf == false) return;
            float remainigDistance = (transform.position - _target.gameObject.transform.position).sqrMagnitude;
            Vector3 newPos = Vector3.MoveTowards(_rigid.position, _target.gameObject.transform.position, _creatureData.Speed * Time.deltaTime);
            _rigid.MovePosition(newPos);
            remainigDistance = (transform.position - _target.gameObject.transform.position).sqrMagnitude;

            await UniTask.WaitForFixedUpdate();
            moveTime += Time.deltaTime;

        }
        await UniTask.Delay(1500);
        MoveDelay().Forget();
    }
    async UniTaskVoid Move()
    {
        if (transform.gameObject.activeSelf == false) return;
        float remainigDistance = (transform.position - _target.gameObject.transform.position).sqrMagnitude;
        while(remainigDistance > float.Epsilon)
        {
            Vector3 newPos = Vector3.MoveTowards(_rigid.position,_target.gameObject.transform.position, _creatureData.Speed * Time.deltaTime);
            _rigid.MovePosition(newPos);
            remainigDistance = (transform.position - _target.gameObject.transform.position).sqrMagnitude;

            await UniTask.WaitForFixedUpdate();
        }
        Move().Forget();
    }


    public void Listene(Define.GameEvent eventType, Component Sender, object param = null)
    {
        if(eventType == Define.GameEvent.SpawnMonster && (Creature)param == this)
        {
            MoveDelay().Forget();
            _hpImg.fillAmount = (float)_hp / (float)_creatureData.MaxHP;
        }

    }
}
