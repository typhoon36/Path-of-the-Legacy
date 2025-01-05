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
    public Button m_ProgresBtn;
    public Button m_CompleteBtn;

    [Header("QuestText")]
    public Text m_QuestTitle;
    public Text m_QuestDesc;

    [Header("QuestNode")]
    public GameObject m_QuestNode;

    List<QuestData> m_AcceptedQuests = new List<QuestData>();

    #region Singleton
    public static QuestPopup_UI Inst;
    void Awake()
    {
        Inst = this;
    }
    #endregion

    void Start()
    {
        m_QuestPopup.SetActive(false);
        m_CloseBtn.onClick.AddListener(() => m_QuestPopup.SetActive(false));
        m_QuestTitle.text = "";
        m_QuestDesc.text = "";

        // 퀘스트 진행 버튼
        m_ProgresBtn.onClick.AddListener(() =>
        {
            // 퀘스트 진행 노드 생성, 완료된 퀘스트 노드 삭제


        });

        // 퀘스트 완료 버튼
        m_CompleteBtn.onClick.AddListener(() =>
        {
            Debug.Log("퀘스트 완료");
        });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            m_QuestPopup.SetActive(!m_QuestPopup.activeSelf);
        }
    }

    // 노드 생성
    public void CreateNode(QuestData a_QuestData)
    {
        if (a_QuestData == null) return;

        m_AcceptedQuests.Add(a_QuestData);

        GameObject t_Node = Instantiate(m_QuestNode, m_QuestContent.transform);
        Quest_Nd questNode = t_Node.GetComponent<Quest_Nd>();
        questNode.Init(a_QuestData);
    }

    // 완료된 퀘스트 노드 삭제
    public void RemoveNode(QuestData a_QuestData)
    {
        if (a_QuestData == null) return;
        m_AcceptedQuests.Remove(a_QuestData);
        foreach (Transform t_Node in m_QuestContent.transform)
        {
            Quest_Nd questNode = t_Node.GetComponent<Quest_Nd>();
            
        }
    }


}
