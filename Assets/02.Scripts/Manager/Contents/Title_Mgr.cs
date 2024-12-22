using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title_Mgr : MonoBehaviour
{
    public Button m_newStartBtn;
    public Button m_ContinueBtn;
    public Button m_ExitBtn;
    public Button m_LoadBtn;

    void Start()
    {
        if (m_newStartBtn != null)
            m_newStartBtn.onClick.AddListener(() =>
            {
                // 데이터 초기화
                Data_Mgr.m_StartData = new StartData();
                Data_Mgr.m_ItemData = new List<ItemData>();
                Data_Mgr.m_SkillData = new List<SkillData>();
                Data_Mgr.m_QuestData = new List<QuestData>();
                Data_Mgr.m_TalkData = new List<TalkData>();

                SceneManager.LoadScene("Level1Scene");
            });

        if (m_ContinueBtn != null)
            m_ContinueBtn.onClick.AddListener(() =>
            {
                Data_Mgr.LoadData();
            });

        if (m_ExitBtn != null)
            m_ExitBtn.onClick.AddListener(() =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                Data_Mgr.SaveData();
#else
                Application.Quit();
#endif
            });

        if (m_LoadBtn != null)
            m_LoadBtn.onClick.AddListener(() =>
            {
                Data_Mgr.LoadData();
            });
    }
}
