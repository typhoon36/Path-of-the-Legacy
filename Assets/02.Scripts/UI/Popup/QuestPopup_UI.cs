using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// 퀘스트 팝업 UI(수락한 퀘스트가 무엇이 있는지 보여주는 팝업)
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
        //    //퀘스트 갱신
        //});
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            
        }

    }


}
