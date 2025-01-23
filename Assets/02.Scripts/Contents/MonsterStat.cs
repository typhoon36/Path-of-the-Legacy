using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

/*���� �⺻ ����*/
public class MonsterStat : MonoBehaviour
{
    //Stats
    #region Stats
    [SerializeField] protected int Id;
    [SerializeField] protected int m_CurHp;
    [SerializeField] protected int m_MaxHp;
    [SerializeField] protected int m_Att;
    [SerializeField] protected int m_Exp;
    [SerializeField] protected int m_Gold;
    [SerializeField] protected int Item_Id = 1;
    [SerializeField] protected float m_Speed;

    public int m_Id { get { return Id; } set { Id = value; } } //���� ID
    public int CurHp { get { return m_CurHp; } set { m_CurHp = Mathf.Clamp(value, 0, MaxHp); } } //���� ���� ü��
    public int MaxHp { get { return m_MaxHp; } set { m_MaxHp = value; CurHp = MaxHp; } } //���� �ִ� ü��
    public int Attack { get { return m_Att; } set { m_Att = value; } } //���� ���ݷ�
    public int Exp { get { return m_Exp; } set { m_Exp = value; } } //���� ��������� ����ġ
    public int Gold { get { return m_Gold; } set { m_Gold = value; } } //���Ͱ� ����Ʈ�� ��尪
    public int ItemId { get { return Item_Id; } set { Item_Id = value; } }  //���Ͱ� ����Ʈ�� ������ID
    public float Speed { get { return m_Speed; } set { m_Speed = value; } } //���� �̵��ӵ�
    #endregion

    void Start()
    {
        m_Monster = GetComponent<Monster_Ctrl>();
        GameObject Obj = this.gameObject;

        MonsterStat a_Stat = Obj.GetComponent<MonsterStat>();
        MaxHp = a_Stat.MaxHp;
        Attack = a_Stat.Attack;
        Exp = a_Stat.Exp;
        Gold = a_Stat.Gold;
        ItemId = a_Stat.ItemId;
        Speed = a_Stat.Speed;
    }

    //���� �޾�����
    Monster_Ctrl m_Monster;
    public virtual void OnAttacked(int a_Damage)
    {
        //�Ϲ� ���͸� �ǰ� ���� ����
        if (m_Monster.m_MonsterType == Define_S.MonsterType.Normal)
        {
            m_Monster.State = Define_S.AllState.Hit;
        }

        //���� HP�ٿ� HPBar Ȱ��ȭ
        m_Monster.HPBar.fillAmount = (float)m_CurHp / m_MaxHp;
        m_Monster.HPBack.SetActive(true);
        m_Monster.HPBar.gameObject.SetActive(true);

        CurHp -= a_Damage; // ü�� ����

        if (m_CurHp <= 0)
        {
            m_CurHp = 0;
            OnDie();
        }
    }

    protected virtual void OnDie()
    {
        m_Monster.State = Define_S.AllState.Die;

        // ���� �ݿ�
        if (UI_Mgr.Inst != null)
            UI_Mgr.Inst.m_ExpBar.fillAmount += (float)Exp / 800;

        if (InvenPopup_UI.Inst != null)
            InvenPopup_UI.Inst.AddGold(Gold);

        // ����Ʈ ���� �ݿ�


        // ������ ���
        OnDropItem();

        //���� ����
        m_Monster.BattleEnd();
    }

    //������ ���
    void OnDropItem()
    {
        List<int> ItemList = Data_Mgr.DropItem[Id];

        int a_MaxCnt = Mathf.Clamp(Data_Mgr.m_StartData.Luk, 0, 2);

        for (int i = 0; i < a_MaxCnt; i++)
        {
            //���� id ����
            int a_RandId = Random.Range(0, ItemList.Count);

            //������ ��ȯ
            int itemId = ItemList[a_RandId];
            GameObject itemObj;
            //����ó��
            if (!Data_Mgr.ItemObjects.TryGetValue(itemId, out itemObj) || itemObj == null)
            {
                Debug.LogError($"Item object with Id {itemId} is null or not found.");
                continue;
            }

            GameObject a_Obj = Instantiate(itemObj);

            //ItemPickUp ������Ʈ �߰�
            ItemPickUp a_Data = a_Obj.AddComponent<ItemPickUp>();
            a_Data.m_Item = Data_Mgr.CallItem(itemId);

            //������ ��ġ ����
            float a_RandPos = Random.Range(-0.5f, 0.5f);
            a_Obj.transform.position = new Vector3(transform.position.x + a_RandPos, 0, transform.position.z + a_RandPos);
        }
    }
}
