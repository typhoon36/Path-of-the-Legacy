using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

///메인 UI 매니저 : 메인 UI를 위한 스크립트
public class UI_Mgr : MonoBehaviour
{
    [Header("HUD")]
    public Image m_HPBar;
    public Image m_MPBar;
    public Image m_ExpBar;
    public Text m_HpTxt;
    public Text m_MpTxt;
    public Text m_LevelText;

    [Header("Slots")]
    public GameObject m_Slots;

    [Header("MiniMap")]
    public GameObject m_MiniMap;
    public Define_S.Scene m_TargetScene;

    [Header("Skill&Item")]
    public Image m_SkillA;
    public Image m_SkillD;
    public Image m_1stItem;
    public Image m_2ndItem;
    public Dictionary<Define_S.KeySkill, SkillData> SkillBarList
        = new Dictionary<Define_S.KeySkill, SkillData>();

    Coroutine Co_Item1Recovery;
    Coroutine Co_Item2Recovery;

    [Header("Die")]
    public GameObject m_DiePanel;
    public Button m_ConfrimBtn;

    public bool IsPressed { get; private set; } = false;

    #region Singleton
    public static UI_Mgr Inst;
    void Awake()
    {
        if (Inst == null)
            Inst = this;
    }
    #endregion


    void Start()
    {
        Data_Mgr.LoadData();

        #region HUD
        m_HPBar.fillAmount = 1;
        m_MPBar.fillAmount = 1;
        m_ExpBar.fillAmount = 0;

        m_HpTxt.text = Data_Mgr.m_StartData.CurHp.ToString() + " / " + Data_Mgr.m_StartData.MaxHp.ToString();
        m_MpTxt.text = Data_Mgr.m_StartData.CurMp.ToString() + " / " + Data_Mgr.m_StartData.MaxMp.ToString();
        #endregion

        m_DiePanel.gameObject.SetActive(false);

        if (m_ConfrimBtn != null)
        {
            m_ConfrimBtn.onClick.AddListener(() =>
            {
                IsPressed = true;
                DieOff();

                if (m_TargetScene == Define_S.Scene.Game)
                {
                    Scene_Mgr.Inst.ChangeScene(m_TargetScene);
                    Player_Ctrl a_Player = FindObjectOfType<Player_Ctrl>();
                    Vector3 a_Pos = new Vector3(147.2f, 0.064f, 22.87f);
                    a_Player.transform.position = a_Pos;
                    a_Player.CurHp = a_Player.MaxHp;
                    Data_Mgr.m_StartData.m_Pos = a_Pos; 
                    Data_Mgr.SaveData(); 
                }
            });
        }

        //모바일 플랫폼이라면 슬롯 비활성화
        if (Application.isMobilePlatform) m_Slots.gameObject.SetActive(false);

        if (m_TargetScene == Define_S.Scene.Game) m_MiniMap.gameObject.SetActive(true);
        else m_MiniMap.gameObject.SetActive(false);

        if (Data_Mgr.m_SkillData.Count > 1)
        {
            SkillBarList.Add(Define_S.KeySkill.A, Data_Mgr.m_SkillData[0]);
            SkillBarList.Add(Define_S.KeySkill.D, Data_Mgr.m_SkillData[1]);
        }
    }

    void Update()
    {
        LevelRefresh();
        CheckExpBar();
        UpdateSkillCool();
        RefreshUI();
    }

    #region HUD
    public void LevelRefresh()
    {
        m_LevelText.text = Data_Mgr.m_StartData.Level.ToString();
    }

    public void CheckExpBar()
    {
        //레벨업 
        if (1 <= m_ExpBar.fillAmount)
        {
            m_ExpBar.fillAmount = 0;//경험치바 초기화

            Data_Mgr.m_StartData.Level += 1; //레벨 증가

            LevelRefresh();//레벨 갱신(레벨 텍스트)

            Player_Ctrl a_Player = FindObjectOfType<Player_Ctrl>();

            if (a_Player != null) a_Player.LevelUpEffect();

            Data_Mgr.m_StartData.StatPoint += 5; //스탯 포인트 5증가

            Data_Mgr.SaveData();
        }
    }

    public void UpdateHPBar(float a_CurHp, float a_MaxHp)
    {
        if (a_MaxHp > 0)
        {
            m_HPBar.fillAmount = Mathf.Clamp01(a_CurHp / a_MaxHp);
            m_HpTxt.text = $"{a_CurHp} / {a_MaxHp}";
        }
        else
        {
            m_HPBar.fillAmount = 0;
            m_HpTxt.text = "0 / 0";
        }
    }

    public void UpdateMpBar(float a_CurMp, float a_MaxMp)
    {
        if (a_MaxMp > 0)
            m_MPBar.fillAmount = Mathf.Clamp01(a_CurMp / a_MaxMp);
        
        else
            m_MPBar.fillAmount = 0;
        
    }
    #endregion


    public void UpdateSkillBar(Define_S.KeySkill a_KSkill, float a_Idx)
    {
        switch (a_KSkill)
        {
            case Define_S.KeySkill.A:
                m_SkillA.fillAmount = a_Idx; //스킬 A의 fillAmount를 a_Idx로 설정
                break;
            case Define_S.KeySkill.D:
                m_SkillD.fillAmount = a_Idx;//스킬 D의 fillAmount를 a_Idx로 설정
                break;
        }
    }

    public void UpdateSkillCool()
    {
        foreach (var skill in SkillBarList)
        {
            if (skill.Value.isCoolDown)
            {
                float a_CoolProgress = (Time.time - skill.Value.skillCoolDown) / skill.Value.skillCoolDown;
                UpdateSkillBar(skill.Key, Mathf.Clamp01(a_CoolProgress));
                if (a_CoolProgress >= 1)
                {
                    skill.Value.isCoolDown = false;
                    skill.Value.skillCoolDown = 0; // 쿨다운 초기화
                }
            }
        }
    }

    public void RefreshUI()
    {
        m_HpTxt.text = Data_Mgr.m_StartData.CurHp.ToString() + " / " + Data_Mgr.m_StartData.MaxHp.ToString();
        m_MpTxt.text = Data_Mgr.m_StartData.CurMp.ToString() + " / " + Data_Mgr.m_StartData.MaxMp.ToString();

    }

    #region Die
    public void DieOn()
    {
        m_DiePanel.gameObject.SetActive(true);
        m_HPBar.fillAmount = 0;
    }

    public void DieOff()
    {
        m_DiePanel.gameObject.SetActive(false);
        m_HPBar.fillAmount = 1;
    }
    #endregion

    public void ResetButtonPress()
    {
        IsPressed = false;
    }

    #region Item
    public void UseItem(int a_Idx)
    {
        Player_Ctrl a_Player = FindObjectOfType<Player_Ctrl>();

        switch (a_Idx)
        {
            case 0:
                {
                    // 1아이템 사용
                    m_1stItem.fillAmount = 0;
                    a_Player.CurHp += 50; // CurHp 프로퍼티를 사용하여 HP 증가

                    if (a_Player.CurHp > a_Player.MaxHp)
                    {
                        a_Player.CurHp = a_Player.MaxHp;
                    }

                    if (Co_Item1Recovery != null)
                        StopCoroutine(Co_Item1Recovery);
                    Co_Item1Recovery = StartCoroutine(RecoverItem(m_1stItem, 15f));
                    break;
                }
            case 1:
                {
                    // 2아이템 사용
                    m_2ndItem.fillAmount = 0;
                    a_Player.CurMp += 50; // CurMp 프로퍼티를 사용하여 MP 증가

                    if (a_Player.CurMp > a_Player.MaxMp)
                    {
                        a_Player.CurMp = a_Player.MaxMp;
                    }

                    if (Co_Item2Recovery != null)
                        StopCoroutine(Co_Item2Recovery);

                    Co_Item2Recovery = StartCoroutine(RecoverItem(m_2ndItem, 15f));
                    break;
                }
        }
    }

    //쿨타임 복구
    IEnumerator RecoverItem(Image a_Icon, float a_Dur)
    {
        float a_Dealy = 0f;

        while (a_Dealy < a_Dur)
        {
            a_Dealy += Time.deltaTime;
            a_Icon.fillAmount = Mathf.Clamp01(a_Dealy / a_Dur);
            yield return null;
        }


        a_Icon.fillAmount = 1f;
    }
    #endregion

}
