using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// ����Ʈ �˾� UI(������ ����Ʈ�� ������ �ִ��� �����ִ� �˾�)
public class QuestPopup_UI : MonoBehaviour
{
    [Header("QuestPopup")]
    public GameObject m_QuestPopup;
    public GameObject m_QuestView;    
   // public Button m_RefreshBtn;

    //[Header("QuestNode")]
    //public GameObject m_QuestNode;

    QuestData m_QuestData;

    private void Start()
    {
        //m_RefreshBtn.onClick.AddListener(() =>
        //{
        //    //����Ʈ ����
        //});
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            
        }

    }


}
