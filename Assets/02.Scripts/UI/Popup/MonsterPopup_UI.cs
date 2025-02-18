using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterPopup_UI : MonoBehaviour
{
    [Header("MonsterPopup")]
    public GameObject m_MonsterPopup;
    public Button m_CloseBtn;
    public Text m_MonsterName;
    public Text m_MonsterHp;
    public Image m_HpBar;

    #region Singleton
    public static MonsterPopup_UI Inst;
    void Awake()
    {
        Inst = this;
    }
    #endregion

    void Start()
    {
        m_MonsterPopup.gameObject.SetActive(false);
        m_CloseBtn.onClick.AddListener(() => { m_MonsterPopup.SetActive(false); });

        MonsterStat[] monsters = GameObject.FindObjectsOfType<MonsterStat>();
        if (monsters.Length > 0)
        {
            m_MonsterName.text = monsters[0].name;
        }
    }

    public void ShowPopup(MonsterStat monsterStat)
    {
        Monster_Ctrl a_Monster = monsterStat.GetComponent<Monster_Ctrl>();
        if (a_Monster != null && a_Monster.m_MonsterType == Define_S.MonsterType.Boss) return; // 보스 몬스터일 경우 팝업을 띄우지 않음
        

        m_MonsterName.text = monsterStat.m_Name;
        m_MonsterHp.text = $"{monsterStat.CurHp} / {monsterStat.MaxHp}";
        m_HpBar.fillAmount = (float)monsterStat.CurHp / monsterStat.MaxHp;
        m_MonsterPopup.SetActive(true);
    }
}
