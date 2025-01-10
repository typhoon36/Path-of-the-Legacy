using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

///메인 UI 매니저 : 메인 UI를 위한 스크립트
public class UI_Mgr : MonoBehaviour
{
    public bool IsPressed { get; private set; } = false;

    [Header("HUD")]
    public Image m_HPBar;
    public Image m_MPBar;
    public Image m_ExpBar;

    [Header("Level")]
    public Text m_LevelText;

    #region DiePanel
    [Header("Die")]
    public GameObject m_DiePanel;
    public Button m_ConfrimBtn;
    #endregion

    [Header("Menu")]
    public GameObject m_MenuPanel;
    public Button m_ResumeBtn;
    public Button m_OptionBtn;
    public Button m_ExitBtn;

    [Header("Option")]
    public GameObject m_OptionPanel;
    public Slider m_SoundSlider;
    public Slider m_MusicSlider;

    [Header("Mobile_OFF")]
    public GameObject m_Slots;

    [Header("MiniMap")]
    public GameObject m_MiniMap;
    public Define_S.Scene m_TargetScene;

    [Header("SkillBar")]
    public Image m_SkillA;
    public Image m_SkillD;
    public Dictionary<Define_S.KeySkill, SkillData> SkillBarList = new Dictionary<Define_S.KeySkill, SkillData>();

    [Header("ItemBar")]
    public Image m_1stItem;
    public Image m_2ndItem;

    Coroutine Co_Item1Recovery;
    Coroutine Co_Item2Recovery;

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

        m_HPBar.fillAmount = 1;
        m_MPBar.fillAmount = 1;
        m_ExpBar.fillAmount = 0;

        m_DiePanel.gameObject.SetActive(false);

        if (m_ConfrimBtn != null)
            m_ConfrimBtn.onClick.AddListener(() =>
            {
                IsPressed = true;
                DieOff();
                Scene_Mgr.Inst.ChangeScene(Define_S.Scene.Game);
            });

        m_MenuPanel.gameObject.SetActive(false);

        if (m_MenuPanel != null)
        {
            if (m_ResumeBtn != null)
                m_ResumeBtn.onClick.AddListener(() =>
                {
                    IsPressed = true;
                    m_MenuPanel.gameObject.SetActive(false);
                    Time.timeScale = 1;
                });
            if (m_OptionBtn != null)
                m_OptionBtn.onClick.AddListener(() =>
                {
                    IsPressed = true;
                    m_MenuPanel.gameObject.SetActive(false);
                    m_OptionPanel.gameObject.SetActive(true);
                });
            if (m_ExitBtn != null)
                m_ExitBtn.onClick.AddListener(() =>
                {
                    IsPressed = true;
                    m_MenuPanel.gameObject.SetActive(false);
                    Time.timeScale = 1;
                    Data_Mgr.SaveData();
                    SceneManager.LoadScene("TitleScene");
                });
        }

        if (Application.isMobilePlatform)
            m_Slots.gameObject.SetActive(false);

        if (m_TargetScene == Define_S.Scene.Game)
            m_MiniMap.gameObject.SetActive(true);
        else
            m_MiniMap.gameObject.SetActive(false);

        if (Data_Mgr.m_SkillData.Count > 1)
        {
            SkillBarList.Add(Define_S.KeySkill.A, Data_Mgr.m_SkillData[0]);
            SkillBarList.Add(Define_S.KeySkill.D, Data_Mgr.m_SkillData[1]);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IsPressed = true;
            m_MenuPanel.gameObject.SetActive(true);
            Time.timeScale = 0;
        }
        LevelRefresh();
        CheckExpBar();
        UpdateSkillCooldowns();// 스킬 쿨다운 갱신
    }

    #region HUD
    public void LevelRefresh()
    {
        m_LevelText.text = Data_Mgr.m_StartData.Level.ToString();
    }

    public void CheckExpBar()
    {
        if (m_ExpBar.fillAmount >= 1)
        {
            m_ExpBar.fillAmount = 0;
            Data_Mgr.m_StartData.Level += 1;
            LevelRefresh();
            Player_Ctrl a_Player = FindObjectOfType<Player_Ctrl>();
            if (a_Player != null)
            {
                a_Player.LevelUpEffect();
            }
        }
    }

    public void UpdateHPBar(float a_CurHp, float a_MaxHp)
    {
        if (a_MaxHp > 0)
        {
            m_HPBar.fillAmount = Mathf.Clamp01(a_CurHp / a_MaxHp);
        }
        else
        {
            m_HPBar.fillAmount = 0;
        }
    }

    public void UpdateMpBar(float a_CurMp, float a_MaxMp)
    {
        if (a_MaxMp > 0)
        {
            m_MPBar.fillAmount = Mathf.Clamp01(a_CurMp / a_MaxMp);
        }
        else
        {
            m_MPBar.fillAmount = 0;
        }
    }

    public void UpdateSkillBar(Define_S.KeySkill keySkill, float fillAmount)
    {
        switch (keySkill)
        {
            case Define_S.KeySkill.A:
                m_SkillA.fillAmount = fillAmount;
                break;
            case Define_S.KeySkill.D:
                m_SkillD.fillAmount = fillAmount;
                break;
        }
    }

    public void UpdateSkillCooldowns()
    {
        foreach (var skill in SkillBarList)
        {
            if (skill.Value.isCoolDown)
            {
                float cooldownProgress = (Time.time - skill.Value.skillCoolDown) / skill.Value.skillCoolDown;
                UpdateSkillBar(skill.Key, Mathf.Clamp01(cooldownProgress));
                if (cooldownProgress >= 1)
                {
                    skill.Value.isCoolDown = false;
                    skill.Value.skillCoolDown = 0; // 쿨다운 초기화
                }
            }
        }
    }
    #endregion

    public void DieOn()
    {
        m_DiePanel.gameObject.SetActive(true);
        m_HPBar.fillAmount = 0;
    }

    public void DieOff()
    {
        m_DiePanel.gameObject.SetActive(false);
    }

    public void ResetButtonPress()
    {
        IsPressed = false;
    }

    #region ItemSlotUse
    public void UseItem(int Idx)
    {
        switch (Idx)
        {
            case 0:
                {
                    // 1아이템 사용
                    m_1stItem.fillAmount = 0;
                    m_HPBar.fillAmount += 0.5f;
                    Data_Mgr.m_StartData.CurHp += 50;

                    if(Data_Mgr.m_StartData.CurHp > Data_Mgr.m_StartData.MaxHp)
                    {
                        Data_Mgr.m_StartData.CurHp = Data_Mgr.m_StartData.MaxHp;
                    }

                    if (Co_Item1Recovery != null)
                        StopCoroutine(Co_Item1Recovery);
                    Co_Item1Recovery = StartCoroutine(RecoverItem(m_1stItem, 5f)); // 5초 동안 회복
                    break;
                }
            case 1:
                {
                    // 2아이템 사용
                    m_2ndItem.fillAmount = 0;
                    m_MPBar.fillAmount += 0.5f;
                    Data_Mgr.m_StartData.CurMp += 50;

                    if(Data_Mgr.m_StartData.CurMp > Data_Mgr.m_StartData.MaxMp)
                    {
                        Data_Mgr.m_StartData.CurMp = Data_Mgr.m_StartData.MaxMp;
                    }


                    if (Co_Item2Recovery != null)
                        StopCoroutine(Co_Item2Recovery);

                    Co_Item2Recovery = StartCoroutine(RecoverItem(m_2ndItem, 5f)); // 5초 동안 회복
                    break;
                }
            default:
                {
                    Debug.LogWarning("Invalid item index");
                    break;
                }
        }
    }

    IEnumerator RecoverItem(Image Icon, float a_Dur)
    {
        float a_Dealy = 0f;
        while (a_Dealy < a_Dur)
        {
            a_Dealy += Time.deltaTime;
            Icon.fillAmount = Mathf.Clamp01(a_Dealy / a_Dur);
            yield return null;
        }
        Icon.fillAmount = 1f;
    }
    #endregion
}
