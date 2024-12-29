using I18N.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestNpc_Ctrl : Npc_Ctrl
{
    //����Ʈ Ÿ��
    public Define_S.QuestType m_QuestType;

    [SerializeField]
    private int[] m_QuestId; //����Ʈ id Array

    [SerializeField]
    private int[] m_TalkId;             // ��ȭ id Array

    private int m_NextQuestIdx;     // ���� ����Ʈ �ε���

    [HideInInspector] public bool IsQuest;            // ����Ʈ�� �����ϴ���?

    QuestData m_CurQuest;       // ���� ����Ʈ
    TalkData m_CurTalk;        // ���� ��ȭ

    List<QuestData> m_QuestDataList;
    List<TalkData> m_TalkDataList;

    QuestMark_UI m_MarkObj;

    #region <Init>
    public override void Init()
    {
        base.Init();

        m_QuestDataList = new List<QuestData>();
        m_TalkDataList = new List<TalkData>();

        // id�� �°� ����Ʈ, ��ȭ �����͸� �����´�.
        for (int i = 0; i < m_QuestId.Length; i++)
        {
            // m_QuestId�� m_TalkId�� ���̰� ���ٰ� ����
            if (i < Data_Mgr.m_QuestData.Count)
            {
                m_QuestDataList.Add(Data_Mgr.m_QuestData[m_QuestId[i]]);
            }
            if (i < Data_Mgr.m_TalkData.Count)
            {
                m_TalkDataList.Add(Data_Mgr.m_TalkData[m_TalkId[i]]);
            }
        }

        m_NextQuestIdx = 0;

        // ���� ����Ʈ & ��ȭ ����
        if (m_QuestDataList.Count > 0 && m_TalkDataList.Count > 0)
        {
            m_CurQuest = m_QuestDataList[m_NextQuestIdx];
            m_CurTalk = m_TalkDataList[m_NextQuestIdx];
            IsQuest = true;
        }
        else
        {
            IsQuest = false;
        }


        // QuestMark_UI ��ü�� ã�Ƽ� �Ҵ�
        m_MarkObj = GetComponentInChildren<QuestMark_UI>();

        // ������ ����
        Invoke("DelayInit", 0.0001f);
    }

    void DelayInit()
    {
        //���� Ŭ������ ����Ʈ�� �ִ���
        for (int i = 0; i < m_QuestDataList.Count; i++)
        {
            if (m_QuestDataList[i].IsClear == true)
            {
                QuestCheck();
                continue;
            }
        }

        //���� Ȯ��
        for(int i = 0; i < m_QuestDataList.Count; i++)
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
        base.OnInteract();
    }

    protected override void OpenPopup()
    {
        TalkCheck();
    }

    #region Check
    void TalkCheck()
    {
        if (m_CurTalk == null) return;

        //�̹� ����Ʈ�� Ŭ���� �߰ų� ����Ʈ�� �������
        if (m_CurQuest.IsClear == true || IsQuest == false)
        {
            SetTalk(m_CurTalk.basicsTalk);
            return;
        }

        //����Ʈ�� �����Ǿ� �������� ���¶��
        if (m_CurQuest.IsAccept == true)
        {
            //����Ʈ  ��ǥ ���� Ȯ��
            if (m_CurQuest.CurTargetCnt >= m_CurQuest.TargetCnt)
            {
                SetTalk(m_CurTalk.clearTalk);
                m_CurQuest.QuestClear();
                QuestCheck();
            }
            else
                SetTalk(m_CurTalk.procTalk);

            return;
        }

        //������ �Ǹ� ����Ʈ ��ȭ ����
        if (m_CurQuest.MinLevel <= Data_Mgr.m_StartData.Level)
        {
            SetTalk(m_CurTalk);
            return;
        }


        SetTalk(m_CurTalk.basicsTalk);
    }

    //�⺻ ��ȭ ����(�����ε�)
    void SetTalk(string a_Text)
    {
        TalkPopup_UI a_Talk = TalkPopup_UI.Inst;
        a_Talk.m_TalkPanel.gameObject.SetActive(true);
        a_Talk.SetInfo(a_Text, m_NpcName);

    }
    //TalkData�� �޾� ����Ʈ ����(�����ε�)
    void SetTalk(TalkData a_Talk)
    {
        TalkPopup_UI a_TalkPopup = TalkPopup_UI.Inst;
        a_TalkPopup.m_TalkPanel.gameObject.SetActive(true);
        a_TalkPopup.SetInfo(a_Talk, m_CurQuest, m_NpcName);
    }
    void QuestCheck()
    {
        m_NextQuestIdx++;

        //����Ʈ ���� Ȯ��
        if (m_NextQuestIdx >= m_QuestId.Length)
        {
            IsQuest = false;
            return;
        }

        //����Ʈ, ��ȭ ����
        m_CurQuest = m_QuestDataList[m_NextQuestIdx];
        m_CurTalk = m_TalkDataList[m_NextQuestIdx];

        IsQuest = true;
    }
    #endregion

    #region <Mark>
    void MarkUpdate()
    {
        //��ũ ������Ʈ�� �Ҵ� �Ǿ����� ������ ����
        if (m_MarkObj == null) return;

        //���� Ȯ�� 
        if (m_CurQuest.MinLevel > Data_Mgr.m_StartData.Level) return;

        //���� ����Ʈ�� �������� �ʾ��� ���
        if (m_CurQuest.IsAccept == false)
        {
            m_MarkObj.SetInfo("?", transform.position);
            return;
        }


        //����Ʈ Ŭ���� ���� Ȯ��
        if (m_CurQuest.IsClear == true || IsQuest == false)
        {
            m_MarkObj.SetInfo(" ", transform.position);
            return;
        }

        //����Ʈ ��ǥ �޼� Ȯ��
        if (m_CurQuest.CurTargetCnt >= m_CurQuest.TargetCnt)
        {
            m_MarkObj.SetInfo("?", transform.position);
            return;
        }

        //����Ʈ ���� ���� Ȯ��
        if (m_CurQuest.IsAccept == true)
        {
            m_MarkObj.SetInfo(" ", transform.position);
        }
        else
        {
            m_MarkObj.SetInfo("!", transform.position);
        }
    }
    #endregion


}
