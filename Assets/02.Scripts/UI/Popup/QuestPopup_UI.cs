using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// 퀘스트 팝업 UI(수락한 퀘스트가 무엇이 있는지 보여주는 팝업)
public class QuestPopup_UI : MonoBehaviour
{
    [Header("QuestPopup")]
    public GameObject m_QuestPopup;
    public GameObject m_QuestContent;

    [Header("QuestButton")]
    public Button m_CloseBtn;

    [Header("QuestText")]
    public Text m_QuestTitle;
    public Text m_QuestDesc;

    [Header("QuestNode")]
    public GameObject m_QuestNode;

    QuestData m_QuestData;

    void Start()
    {
        m_QuestPopup.SetActive(false);
        //m_CloseBtn.onClick.AddListener(() => m_QuestPopup.SetActive(false));
        //m_QuestTitle.text = m_QuestData.TitleName;
        //m_QuestDesc.text = m_QuestData.Desc;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            m_QuestPopup.SetActive(!m_QuestPopup.activeSelf);
        }
    }

    //노드 생성
    public void CreateNode(QuestData a_QuestData)
    {

    }



}
