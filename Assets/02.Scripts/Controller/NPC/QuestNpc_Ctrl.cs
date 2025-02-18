using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestNpc_Ctrl : Npc_Ctrl
{
    //퀘스트 타입
    public Define_S.QuestType m_QuestType; //퀘스트 타입(대화, 몬스터, 아이템)

    [SerializeField] int[] m_QuestId; //퀘스트 id Array

    [SerializeField] int[] m_TalkId;             // 대화 id Array

    int m_NextQuestIdx;     // 다음 퀘스트 인덱스

    [HideInInspector] public bool IsQuest;            // 퀘스트를 가지고 있는가?

    QuestData m_CurQuest;       // 현재 퀘스트
    TalkData m_CurTalk;        // 현재 대화

    List<QuestData> m_QuestDataList;
    List<TalkData> m_TalkDataList;

    QuestMark_UI m_MarkObj;

    #region <Init>
    public override void Init()
    {
        base.Init();

        m_QuestDataList = new List<QuestData>();
        m_TalkDataList = new List<TalkData>();

        // id를 통해 퀘스트, 대화 데이터를 가져온다.
        for (int i = 0; i < m_QuestId.Length; i++)
        {
            m_QuestDataList.Add(Data_Mgr.m_QuestData[m_QuestId[i]]);
            m_TalkDataList.Add(Data_Mgr.m_TalkData[m_TalkId[i]]);
        }

        m_NextQuestIdx = 0;

        // 현재 퀘스트 & 대화 설정

        m_CurQuest = m_QuestDataList[m_NextQuestIdx];
        m_CurTalk = m_TalkDataList[m_NextQuestIdx];
        IsQuest = true;


        // QuestMark_UI 오브젝트를 찾아서 할당
        m_MarkObj = GetComponentInChildren<QuestMark_UI>();

        // 초기화 지연
        Invoke("DelayInit", 0.0001f);
    }

    void DelayInit()
    {
        //퀘스트 리스트를 순회하며 퀘스트가 클리어 되었는지 확인
        for (int i = 0; i < m_QuestDataList.Count; i++)
        {
            if (m_QuestDataList[i].IsClear == true)
            {
                QuestCheck();
                continue;
            }
        }

        //퀘스트 수락 여부 확인
        for (int i = 0; i < m_QuestDataList.Count; i++)
        {
            if (m_QuestDataList[i].IsAccept == true)
            {
                m_CurQuest = m_QuestDataList[i];
                m_CurTalk = m_TalkDataList[i];
                IsQuest = true;
                break;
            }
        }
    }
    #endregion

    void FixedUpdate()
    {
        MarkUpdate();
    }

    protected override void OnInteract()
    {
        //이미 대화중이라면 리턴
        if (TalkPopup_UI.Inst.m_TalkPanel.gameObject.activeSelf) return;

        base.OnInteract();
    }

    protected override void OpenPopup()
    {
        TalkCheck();
    }

    void TalkCheck()
    {
        if (m_CurTalk == null) return;

        // 이미 퀘스트가 클리어 되었거나 퀘스트가 없는 경우
        if (IsQuest == false)
        {
            SetTalk(m_CurTalk.BasicsTalk);
            return;
        }

        // 퀘스트가 수락된 상태인 경우
        if (m_CurQuest.IsAccept == true)
        {
            // 퀘스트 목표 달성 여부 확인
            if (m_CurQuest.CurTargetCnt >= m_CurQuest.TargetCnt)
            {
                SetTalk(m_CurTalk.ClearTalk);
                m_CurQuest.QuestClear();
                Data_Mgr.CompleteQuest(m_CurQuest.Id);
                QuestCheck();

                //보상 지급
                Data_Mgr.m_StartData.Gold += m_CurQuest.RewardGold;
                Data_Mgr.m_StartData.Exp += m_CurQuest.RewardExp;

                Debug.Log($"퀘스트 클리어! 보상: {m_CurQuest.RewardGold}골드, {m_CurQuest.RewardExp}경험치");

                Data_Mgr.SaveData();

                return; // 퀘스트 완료 대화 후 종료 

            }
            else
            {
                SetTalk(m_CurTalk.ProcTalk);
                return;
            }
        }

        // 레벨이 충족되면 퀘스트 대화 시작
        if (m_CurQuest.MinLevel <= Data_Mgr.m_StartData.Level)
        {
            SetTalk(m_CurTalk);
            m_CurQuest.IsAccept = true;
            Data_Mgr.AcceptQuest(m_CurQuest.Id);

            // 퀘스트 수락 시 UI 업데이트
            QuestPopup_UI.Inst.UpdateQuestInfo(m_CurQuest);

            return;
        }

        SetTalk(m_CurTalk.BasicsTalk);
    }

    //기본 대화 설정(문자열)
    void SetTalk(string a_Text)
    {
        TalkPopup_UI a_Talk = TalkPopup_UI.Inst;
        a_Talk.m_TalkPanel.gameObject.SetActive(true);
        a_Talk.SetInfo(a_Text, m_NpcName);
    }

    //TalkData를 받아 퀘스트 대화 설정
    void SetTalk(TalkData a_Talk)
    {
        TalkPopup_UI a_TalkPopup = TalkPopup_UI.Inst;
        a_TalkPopup.m_TalkPanel.gameObject.SetActive(true);
        a_TalkPopup.SetInfo(a_Talk, m_CurQuest, m_NpcName);
    }

    void QuestCheck()
    {
        m_NextQuestIdx++;

        // 퀘스트 리스트 범위 초과 확인
        if (m_NextQuestIdx >= m_QuestId.Length)
        {
            IsQuest = false;
            return;
        }

        // 퀘스트, 대화 설정
        m_CurQuest = m_QuestDataList[m_NextQuestIdx];
        m_CurTalk = m_TalkDataList[m_NextQuestIdx];

        IsQuest = true;
    }

    #region <Mark>
    void MarkUpdate()
    {
        //마크 오브젝트가 할당되지 않았으면 리턴
        if (m_MarkObj == null) return;

        //레벨 확인 
        if (m_CurQuest.MinLevel > Data_Mgr.m_StartData.Level) return;

        //퀘스트가 수락되지 않은 경우
        if (m_CurQuest.IsAccept == false)
        {
            m_MarkObj.SetInfo("?", transform.position);
            return;
        }

        //퀘스트 클리어 여부 확인
        if (m_CurQuest.IsClear == true || IsQuest == false)
        {
            m_MarkObj.SetInfo("!", transform.position);
            return;
        }

        //퀘스트 목표 달성 여부 확인
        if (m_CurQuest.CurTargetCnt >= m_CurQuest.TargetCnt)
        {
            m_MarkObj.SetInfo("!", transform.position);
            return;
        }

        //퀘스트 수락 여부 확인
        if (m_CurQuest.IsAccept == true)
        {
            m_MarkObj.SetInfo(" ", transform.position);
        }
        else
        {
            m_MarkObj.SetInfo("?", transform.position);
        }
    }
    #endregion
}
