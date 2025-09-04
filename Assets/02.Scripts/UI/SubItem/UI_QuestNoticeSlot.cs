using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// Scene UI에 생성된 퀘스트 알람 기능

public class UI_QuestNoticeSlot : UI_Base
{
    enum Texts
    {
        QuestNameText,
        QuestDescText
    }

    public QuestData m_Quest;

    string m_TargetName;        // 목표 이름
    string m_QuestNameText;     // 퀘스트 제목
    string m_QeustDescText;     // 퀘스트 내용

    bool IsSuccess = false;

    public override bool Init()
    {
        if (base.Init() == false) return false;

        BindText(typeof(Texts));

        GetText((int)Texts.QuestNameText).text = m_QuestNameText;
        GetText((int)Texts.QuestDescText).text = m_QeustDescText;

        return true;
    }

    void FixedUpdate()
    {
        if (m_Quest.IsNull() == true || IsSuccess == true) return;

        // 퀘스트 목표 달성 시
        if (m_Quest.currnetTargetCount == m_Quest.targetCount)
        {
            // text 완료 표시
            GetText((int)Texts.QuestNameText).text = m_Quest.titleName + $@"<color=yellow> [완료]</color>";
            IsSuccess = true;
        }

        // 퀘스트 진행 상황 표시
        if (GetText((int)Texts.QuestDescText).IsNull() == false)
            GetText((int)Texts.QuestDescText).text
                = $"{m_TargetName} : {m_Quest.currnetTargetCount} / {m_Quest.targetCount}";
    }

    public void SetInfo(QuestData a_Quest)
    {
        m_Quest = a_Quest;

        // 퀘스트 타겟 이름
        m_TargetName = Managers.Data.Monster[m_Quest.targetId].GetComponent<MonsterStat>().Name;

        // 퀘스트 제목
        m_QuestNameText = m_Quest.titleName;
        m_QeustDescText = $"{m_TargetName} : {m_Quest.currnetTargetCount} / {m_Quest.targetCount}";
    }
}
