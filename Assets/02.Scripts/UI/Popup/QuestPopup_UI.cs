using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// ����Ʈ �˾� UI(������ ����Ʈ�� ������ �ִ��� �����ִ� �˾�)
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

    //��� ����
    public void CreateNode(QuestData a_QuestData)
    {

    }



}
