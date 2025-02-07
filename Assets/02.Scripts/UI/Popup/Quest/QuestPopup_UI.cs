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

    [Header("QuestInfo")]
    public GameObject m_QuestInfoObj;
    public Text m_QuestInfo;
    public Text m_QuestTargetCnt;
    public Button m_QuestCloseBtn;
    public Button m_QuestOpenBtn;

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
        m_QuestTargetCnt.text = "";
        m_QuestInfo.text = "";

        Data_Mgr.LoadData(); // 먼저 데이터를 불러오고 퀘스트 데이터를 가져오기 위해 

        // 수락된 퀘스트 데이터를 가져와서 진행중인 퀘스트 생성
        foreach (var Id in Data_Mgr.m_AcceptedQuest)
        {
            var a_Quest = Data_Mgr.m_QuestData.Find(q => q.Id == Id);

            // 진행중 상태라면 노드 생성
            if (a_Quest != null && a_Quest.IsClear == false)
            {
                CreateNode(a_Quest);
                m_QuestData = a_Quest; // 현재 퀘스트 데이터를 설정
            }
        }

        // 퀘스트 닫기 버튼
        m_QuestCloseBtn.onClick.AddListener(() =>
        {
            m_QuestInfoObj.SetActive(false);

            //m_QuestInfoObj가 꺼졌을시  버튼 위치 변경
            if(m_QuestInfoObj.activeSelf == false)
            {
                RectTransform a_Rect = m_QuestOpenBtn.GetComponent<RectTransform>();
                a_Rect.anchoredPosition = new Vector2(600, 120);
            }
        });

        // 퀘스트 열기 버튼
        m_QuestOpenBtn.onClick.AddListener(() =>
        {
            m_QuestInfoObj.SetActive(true);

            if(m_QuestInfoObj.activeSelf == true)
            {
                RectTransform a_Rect = m_QuestOpenBtn.GetComponent<RectTransform>();
                a_Rect.anchoredPosition = new Vector2(375, 120);
            }
        });

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            m_QuestPopup.SetActive(!m_QuestPopup.activeSelf);
        }
        RefreshUI();
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


    //퀘스트 목표 개수 반영
    public void QuestTargetCnt(GameObject a_Obj)
    {
        // 수락한 퀘스트가 없으면 종료
        if (m_QuestData == null) return;

        // 몬스터 체크
        if (a_Obj.GetComponent<MonsterStat>())
        {
            // 수락한 퀘스트만큼 반복
            foreach (var a_Quest in Data_Mgr.m_QuestData)
            {
                // 오브젝트 ID와 퀘스트 타겟 ID가 같다면
                if (a_Quest.TargetId == a_Obj.GetComponent<MonsterStat>().m_Id && a_Quest.IsAccept && !a_Quest.IsClear)
                {
                    // 퀘스트 목표 개수 증가
                    a_Quest.CurTargetCnt++;
                    RefreshUI();

                    // 퀘스트 목표 개수와 현재 목표 개수가 같다면
                    if (a_Quest.CurTargetCnt >= a_Quest.TargetCnt)
                        // 퀘스트 완료
                        a_Quest.IsClear = true;


                    // 데이터 저장(목표 개수 반영 및 완료 퀘스트 저장)
                    Data_Mgr.SaveData();
                    return;
                }
            }
        }
    }

    public void RefreshUI()
    {
        if (m_QuestData == null) return;

        if (m_QuestData != null)
        {
            m_QuestInfo.text = m_QuestData.Desc;
            m_QuestTargetCnt.text = m_QuestData.CurTargetCnt + "/" + m_QuestData.TargetCnt;

            // 퀘스트 완료시 m_QuestTargetCnt 텍스트 변경
            if (m_QuestData.IsClear)
                m_QuestTargetCnt.text = m_QuestData.CurTargetCnt + "/" + m_QuestData.TargetCnt + " (완료)";
            

        }

    }
}
