using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestMonster : Creature
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
        //Managers.FixedUpdateAction += FUpdate;
        MoveDelay().Forget();
    }

    GameObject _HPCanvas;
    Image _hpImg;

    [System.Obsolete]
    async UniTaskVoid CreateHpBar()
    {
        //�ϵ� �ڵ��� �κ� ���� �ٲ������. ����� ������Ÿ�� �׽�Ʈ�� ���� �ӽ������� ���� �̰� ���߿� CreatureHPBar ���� �����ؾ� ��.
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        Vector2 pos = new Vector2(box.bounds.extents.x + box.bounds.center.x, box.bounds.extents.y + box.bounds.center.y);
        _HPCanvas = await Managers.Object.InstantiateSingle(StringData.HealthBar, pos);
        _HPCanvas.transform.parent = this.transform;
        _HPCanvas = _HPCanvas.transform.FindChild("Health Bar Fill").gameObject;
        _hpImg = _HPCanvas.GetComponent<Image>();

    }
    public override void Damage(int dmg, Creature Target)
    {
        base.Damage(dmg, Target);
        _hpImg.fillAmount = (float)_hp / (float)_creatureData.MaxHP;
    }
   
    void FUpdate()
    {
        // transform.Translate(_target.transform.position * _speed * Time.deltaTime);
        //_rigid.MovePosition();
    }
    private void OnCollisionStay2D(Collision2D collision)
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
}
