using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title_Mgr : MonoBehaviour
{
    public Text m_TitleTxt;
    public Button m_newStartBtn;
    public Button m_ContinueBtn;
    public Button m_ExitBtn;

    void Start()
    {
        if (m_newStartBtn != null)
            m_newStartBtn.onClick.AddListener(() =>
            {
                //빌드시 데이터 초기화
                InitData();

                PlayerPrefs.DeleteAll();

                SceneManager.LoadScene("LoadScene");
            });

        if (m_ContinueBtn != null)
            m_ContinueBtn.onClick.AddListener(() =>
            {
                Data_Mgr.LoadData();
                SceneManager.LoadScene("LoadScene");
            });

        if (m_ExitBtn != null)
            m_ExitBtn.onClick.AddListener(() =>
            {
                Application.Quit();
            });
    }

    float a_Time = 0;
    void Update()
    {
        if (a_Time < 0.5)
        {
            a_Time += Time.deltaTime;
            m_TitleTxt.GetComponent<Outline>().effectColor = new Color(0, 0, 0, 1 - a_Time);
        }
        else if (a_Time < 1)
        {
            a_Time = 1;
            m_TitleTxt.GetComponent<Outline>().effectColor = new Color(0, 0, 0, 1);
        }
        else
        {
            m_TitleTxt.GetComponent<Outline>().effectColor = new Color(1, 1, 1, a_Time);
        }

        a_Time += Time.deltaTime;
    }

    void InitData()
    {
        //퀘스트 초기화
        Data_Mgr.m_AcceptedQuest.Clear();
        //StartData 초기화
        Data_Mgr.m_StartData.Exp = 0;
        Data_Mgr.m_StartData.Level = 1;
        Data_Mgr.m_StartData.MaxHp = 100;
        Data_Mgr.m_StartData.CurHp = 100;
        Data_Mgr.m_StartData.ATK = 10;
        Data_Mgr.m_StartData.STR = 2;
        Data_Mgr.m_StartData.Speed = 5;
        Data_Mgr.m_StartData.Int = 2;
        Data_Mgr.m_StartData.Luk = 2;
        Data_Mgr.m_StartData.Gold = 100;
        Data_Mgr.m_StartData.SkillPoint = 0;
        Data_Mgr.m_StartData.StatPoint = 0;
        Data_Mgr.m_StartData.m_Pos = new Vector3(147.201f, 0.064f, 22.87f);

        Data_Mgr.SaveData();
    }
}
