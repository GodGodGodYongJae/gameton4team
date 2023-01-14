using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField]
    public WeaponData weaponData;
    protected bool isAttack = false;
    protected Player player;
    //�̹� ���ظ� ���� ���� ����Ʈ * �ߺ� ������ �Ͼ�� �ȵǱ� ������.
    protected List<GameObject> damagedMonsterList = new List<GameObject>();
    public virtual void Start()
    {
      GameObject pGO = Managers.Object.GetSingularObjet(StringData.Player);
      player = pGO.GetComponent<Player>();

    }
    public abstract UniTaskVoid Attack();


}
