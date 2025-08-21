using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class QuestNpc_Ctrl : Npc_Ctrl
{
    public Define.QuestType questType;          // 퀘스트 타입

    [SerializeField]
    int[] questId;            // 퀘스트 id Array
    [SerializeField]
    int[] talkId;             // 대화 id Array

    int nextQuestIndex;     // 다음 퀘스트 Index

    bool isQuest;            // 퀘스트가 존재하는가?

    QuestData currentQuest;       // 현재 퀘스트
    TalkData currentTalk;        // 현재 대화

    List<QuestData> questDataList;      // 퀘스트 List
    List<TalkData> talkDataList;       // 대화 List

    UI_QuestNotice noticeObject;       // 알람 Prefab

    public override void Init()
    {
        base.Init();

        questDataList = new List<QuestData>();
        talkDataList = new List<TalkData>();

        // id에 맞게 퀘스트, 대화 데이터 가져오기
        for (int i = 0; i < questId.Length; i++)
        {
            questDataList.Add(Managers.Data.Quest[questId[i]]);
            talkDataList.Add(Managers.Data.Talk[talkId[i]]);
        }

        nextQuestIndex = 0;

        // 현재 퀘스트, 대화 설정
        currentQuest = questDataList[nextQuestIndex];
        currentTalk = talkDataList[nextQuestIndex];
        isQuest = true;

        // 딜레이 Init
        Invoke("DelayInit", 0.0001f);
    }

    void FixedUpdate()
    {
        NoticeUpdate();
    }

    // 상호작용
    protected override void OnInteract()
    {
        // 이미 대화 중이면 종료
        if (Managers.Game.isPopups[Define.Popup.Talk] == true)
            return;

        base.OnInteract();
    }

    protected override void OpenPopup()
    {
        TalkCheck();
    }

    // 대화 시작
    void TalkCheck()
    {
        if (currentTalk.IsNull() == true)
            return;

        // 이미 퀘스트를 클리어 했는가? or 퀘스트가 없거나
        if (currentQuest.IsClear == true || isQuest == false)
        {
            OnTalk(currentTalk.basicsTalk);
            return;
        }

        // 퀘스트가 수락 중이라면
        if (currentQuest.IsAccept == true)
        {
            // 퀘스트 목표 개수 충족 되면
            if (currentQuest.CurrnetTargetCount >= currentQuest.TargetCount)
            {
                // 인벤토리가 꽉찼는지 확인
                if (Managers.Game.m_PlayScene.Inventory.IsInvenMaxSize() == true)
                {
                    // 경고 안내문 생성
                    Managers.UI.MakeSubItem<UI_Guide>().SetInfo("인벤토리가 가득 찼습니다.", Color.red);
                    return;
                }

                // Scene UI 알람이 등록된 퀘스트라면 삭제
                Managers.Game.m_PlayScene.Quest.CloseQuestNotice(currentQuest);

                OnTalk(currentTalk.clearTalk);        // 클리어 대화 시작
                currentQuest.QuestClear();          // 퀘스트 클리어 (보상 지급)

                Managers.Game.RefreshQuest();       // 클리어 퀘스트에 등록
                NextQuestCheck();                        // 다음 퀘스트 확인
            }
            else
                OnTalk(currentTalk.procTalk);         // 퀘스트 진행 대화 시작

            return;
        }

        // 레벨이 되면 퀘스트 대화 시작
        if (currentQuest.MinLevel <= Managers.Game.Level)
        {
            OnTalk(currentTalk);
            return;
        }

        // 여기까지 오면 받을 퀘스트가 없기에 기본 대화 진행
        OnTalk(currentTalk.basicsTalk);
    }

    // 기본 하나의 대화 시작
    void OnTalk(string text)
    {
        UI_TalkPopup talkPopup = Managers.Game.m_PlayScene.Talk;

        Managers.UI.OnPopupUI(talkPopup);       // Popup 활성화
        talkPopup.SetInfo(text, npcName);       // 대화 세팅
    }

    // TalkData를 그대로 보낼 시 퀘스트 시작으로 판정
    void OnTalk(TalkData texts)
    {
        UI_TalkPopup talkPopup = Managers.Game.m_PlayScene.Talk;

        Managers.UI.OnPopupUI(talkPopup);                   // Popup 활성화
        talkPopup.SetInfo(texts, currentQuest, npcName);    // 대화 세팅
    }

    // 다음 퀘스트
    void NextQuestCheck()
    {
        nextQuestIndex++;

        // 퀘스트 개수 확인
        if (nextQuestIndex == questDataList.Count)
        {
            isQuest = false;
            return;
        }

        // 퀘스트, 대화 적용
        currentQuest = questDataList[nextQuestIndex];
        currentTalk = talkDataList[nextQuestIndex];

        isQuest = true;
    }

    // 알람 업데이트 ( 실시간 npc 위의 퀘스트 상태 표시 )
    void NoticeUpdate()
    {
        // Null Check
        if (noticeObject.IsNull() == true)
            return;

        // 레벨 확인
        if (currentQuest.MinLevel > Managers.Game.Level)
            return;

        // 이미 퀘스트를 클리어 했는가? or 퀘스트가 없거나
        if (currentQuest.IsClear == true || isQuest == false)
        {
            noticeObject.SetInfo("", transform.position);
            return;
        }

        // 퀘스트 목표 달성 확인
        if (currentQuest.CurrnetTargetCount >= currentQuest.TargetCount)
        {
            noticeObject.SetInfo("?");
            return;
        }

        // 퀘스트 수락 확인
        if (currentQuest.IsAccept == true)
            noticeObject.SetInfo("", transform.position);
        else
            noticeObject.SetInfo("!", transform.position);
    }

    void DelayInit()
    {
        // 현재 클리어한 퀘스트가 있는지
        for (int i = 0; i < Managers.Game.ClearQuest.Count; i++)
        {
            if (questDataList[nextQuestIndex].Id == Managers.Game.ClearQuest[i].Id)
            {
                NextQuestCheck();
                continue;
            }
        }

        // 수락 확인
        for (int i = 0; i < Managers.Game.CurrentQuest.Count; i++)
        {
            if (currentQuest.Id == Managers.Game.CurrentQuest[i].Id)
                currentQuest = Managers.Game.CurrentQuest[i];
        }

        noticeObject = Managers.UI.MakeWorldSpaceUI<UI_QuestNotice>();
    }
}
