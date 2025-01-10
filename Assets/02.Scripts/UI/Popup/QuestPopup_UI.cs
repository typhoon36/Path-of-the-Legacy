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

    QuestData m_QuestData;

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

        Data_Mgr.LoadData();//<--먼저 데이터를 불러오고 퀘스트 데이터를 가져오기 위해 

        // 수락된 퀘스트 데이터를 가져와서 진행중인 퀘스트 생성
        foreach (var Id in Data_Mgr.m_AcceptedQuest)
        {
            var a_Quest = Data_Mgr.m_QuestData.Find(q => q.Id == Id);

            //진행중 상태라면 노드 생성
            if (a_Quest != null && a_Quest.IsClear == false)
                CreateNode(a_Quest);
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            m_QuestPopup.SetActive(!m_QuestPopup.activeSelf);
        }
    }

    // 진행중인 노드 생성
    public void CreateNode(QuestData a_QuestData)
    {
        if (a_QuestData == null) return;

        GameObject a_Obj = Instantiate(m_QuestNode, m_QuestContent.transform);
        Quest_Nd a_Node = a_Obj.GetComponent<Quest_Nd>();
        a_Node.Init(a_QuestData);

        // 노드 클릭시 퀘스트 정보 보여주기
        a_Node.m_NodeBtn.onClick.AddListener(() =>
        {
            m_QuestData = a_QuestData;
            // 퀘스트 제목 표시
            m_QuestTitle.text = m_QuestData.TitleName;

            // 퀘스트 설명, 목표, 보상 표시
            m_QuestDesc.text = m_QuestData.Desc + "\n" + m_QuestData.TargetCnt + "마리" + "\n" + m_QuestData.RewardGold +
            "골드" + "\n" + m_QuestData.RewardExp + "경험치";
        });
    }
}
