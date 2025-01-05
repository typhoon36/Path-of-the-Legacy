using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Quest_Nd : MonoBehaviour
{
    public Button m_NodeBtn;
    public Text m_QTitleTxt;
    QuestData m_QuestData;

    void Start()
    {
        if (m_QuestData != null)
        {
            m_QTitleTxt.text = m_QuestData.TitleName;
        }
    }

    public void Init(QuestData a_QuestData)
    {
        m_QuestData = a_QuestData;
        if (m_QTitleTxt != null)
        {
            m_QTitleTxt.text = m_QuestData.TitleName;
        }
    }
}
