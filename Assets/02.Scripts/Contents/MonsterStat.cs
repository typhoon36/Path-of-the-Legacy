using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

/*몬스터 기본 정보*/
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

    public int m_Id { get { return Id; } set { Id = value; } } //몬스터 ID
    public int CurHp { get { return m_CurHp; } set { m_CurHp = Mathf.Clamp(value, 0, MaxHp); } } //몬스터 현재 체력
    public int MaxHp { get { return m_MaxHp; } set { m_MaxHp = value; CurHp = MaxHp; } } //몬스터 최대 체력
    public int Attack { get { return m_Att; } set { m_Att = value; } } //몬스터 공격력
    public int Exp { get { return m_Exp; } set { m_Exp = value; } } //몬스터 잡았을시의 경험치
    public int Gold { get { return m_Gold; } set { m_Gold = value; } } //몬스터가 떨어트릴 골드값
    public int ItemId { get { return Item_Id; } set { Item_Id = value; } }  //몬스터가 떨어트릴 아이템ID
    public float Speed { get { return m_Speed; } set { m_Speed = value; } } //몬스터 이동속도
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

    //공격 받았을때
    Monster_Ctrl m_Monster;
    public virtual void OnAttacked(int a_Damage)
    {
        //일반 몬스터만 피격 상태 진행
        if (m_Monster.m_MonsterType == Define_S.MonsterType.Normal)
        {
            m_Monster.State = Define_S.AllState.Hit;
        }

        //몬스터 HP바와 HPBar 활성화
        m_Monster.HPBar.fillAmount = (float)m_CurHp / m_MaxHp;
        m_Monster.HPBack.SetActive(true);
        m_Monster.HPBar.gameObject.SetActive(true);

        CurHp -= a_Damage; // 체력 감소

        if (m_CurHp <= 0)
        {
            m_CurHp = 0;
            OnDie();
        }
    }

    protected virtual void OnDie()
    {
        m_Monster.State = Define_S.AllState.Die;

        // 보상 반영
        if (UI_Mgr.Inst != null)
            UI_Mgr.Inst.m_ExpBar.fillAmount += (float)Exp / 800;

        if (InvenPopup_UI.Inst != null)
            InvenPopup_UI.Inst.AddGold(Gold);

        // 퀘스트 정보 반영


        // 아이템 드랍
        OnDropItem();

        //전투 종료
        m_Monster.BattleEnd();
    }

    //아이템 드랍
    void OnDropItem()
    {
        List<int> ItemList = Data_Mgr.DropItem[Id];

        int a_MaxCnt = Mathf.Clamp(Data_Mgr.m_StartData.Luk, 0, 2);

        for (int i = 0; i < a_MaxCnt; i++)
        {
            //랜덤 id 설정
            int a_RandId = Random.Range(0, ItemList.Count);

            //아이템 소환
            int itemId = ItemList[a_RandId];
            GameObject itemObj;
            //예외처리
            if (!Data_Mgr.ItemObjects.TryGetValue(itemId, out itemObj) || itemObj == null)
            {
                Debug.LogError($"Item object with Id {itemId} is null or not found.");
                continue;
            }

            GameObject a_Obj = Instantiate(itemObj);

            //ItemPickUp 컴포넌트 추가
            ItemPickUp a_Data = a_Obj.AddComponent<ItemPickUp>();
            a_Data.m_Item = Data_Mgr.CallItem(itemId);

            //아이템 위치 설정
            float a_RandPos = Random.Range(-0.5f, 0.5f);
            a_Obj.transform.position = new Vector3(transform.position.x + a_RandPos, 0, transform.position.z + a_RandPos);
        }
    }
}
