using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField]
    public WeaponData weaponData;
    protected bool isAttack = false;
    protected Player player;
    protected CancellationTokenSource cts = new CancellationTokenSource();
    //�̹� ���ظ� ���� ���� ����Ʈ * �ߺ� ������ �Ͼ�� �ȵǱ� ������.
    protected List<GameObject> damagedMonsterList = new List<GameObject>();
    public virtual void Start()
    {
      GameObject pGO = Managers.Object.GetSingularObjet(StringData.Player);
      player = pGO.GetComponent<Player>();

    }

    protected abstract void FEffectFollow();
    public abstract void ChangeWeaponFixedUpdateDelete();
    public abstract UniTaskVoid Attack();


}
