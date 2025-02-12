using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPopup_UI : MonoBehaviour
{
    [Header("Skill")]
    public GameObject m_SkillPopup;
    public Button m_CloseBtn;
    public Button m_Skill1Btn;
    public Button m_Skill2Btn;
    public Text m_HelpTxt;

    #region Singleton
    public static SkillPopup_UI Inst;
    void Awake()
    {
        if (Inst == null)
            Inst = this;
    }
    #endregion

    void Start()
    {
        m_SkillPopup.gameObject.SetActive(false);
        m_CloseBtn.onClick.AddListener(() => m_SkillPopup.SetActive(false));

        m_Skill1Btn.onClick.AddListener(() =>
        {
            m_HelpTxt.gameObject.SetActive(true);
            m_HelpTxt.text = Data_Mgr.m_SkillData[0].skillName + "\n\n" + Data_Mgr.m_SkillData[0].discription;
     
        });

        m_Skill2Btn.onClick.AddListener(() =>
        {
            m_HelpTxt.gameObject.SetActive(true);
            m_HelpTxt.text = Data_Mgr.m_SkillData[1].skillName + "\n\n" + Data_Mgr.m_SkillData[1].discription;
           
        });

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            m_SkillPopup.SetActive(!m_SkillPopup.activeSelf);
        }

    }




}
