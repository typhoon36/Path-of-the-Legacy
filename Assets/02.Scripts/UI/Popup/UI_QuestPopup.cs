using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 현재 수락한 퀘스트 확인 Popup UI
public class UI_QuestPopup : UI_Popup
{
    enum Gameobejcts
    {
        Content,
        QuestJournal,
        QuestRewardGrid,
    }

    enum Buttons
    {
        ExitButton,
    }

    enum Texts
    {
        QuestTitleText,
        QuestDescText,
        QuestTargetText,
        QuestRewardGoldText,
        QuestRewardExpText,
        QuestNoticeCountText,
    }

    public List<UI_QuestNoticeSlot> QuestNoticeList;    // Scene UI에 등록된 퀘스트 List

    QuestData m_CurClickQuest;              // 현재 클릭한 퀘스트 
    int m_MaxQuestNoticeCount = 5;        // 퀘스트 알림 최대 개수

    public override bool Init()
    {
        if (base.Init() == false) return false;

        popupType = Define.Popup.Quest;
        QuestNoticeList = new List<UI_QuestNoticeSlot>();

        // 자식 객체 불러오기
        BindObject(typeof(Gameobejcts));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        // InputManager에 입력 등록
        Managers.Input.KeyAction -= OnQuestPopup;
        Managers.Input.KeyAction += OnQuestPopup;

        SetInfo();

        Managers.UI.ClosePopupUI(this);

        return true;
    }

    void Update()
    {
        // 퀘스트창이 활성화되면 퀘스트 목표 실시간 새로고침
        if (Managers.Game.isPopups[Define.Popup.Quest] == true && m_CurClickQuest != null)
        {
            string str = m_CurClickQuest.targetDescription + "\n" + m_CurClickQuest.currnetTargetCount + " / " + m_CurClickQuest.targetCount;
            GetText((int)Texts.QuestTargetText).text = str;
        }
    }

    // 새로운 퀘스트 받기
    public void SetQeust(QuestData a_Quest)
    {
        // 퀘스트 수락
        a_Quest.isAccept = true;
        Managers.Game.CurrentQuest.Add(a_Quest);

        // 새로고침 UI
        RefreshUI();

        // Scene UI 알림 추가
        SetQuestNotice(a_Quest);
    }

    // 퀘스트 목표 개수 반영
    public void QuestTargetCount(GameObject a_Obj)
    {
        // 수락한 퀘스트가 없으면 종료
        if (Managers.Game.CurrentQuest.Count == 0)
            return;

        // 몬스터 체크
        if (a_Obj.GetComponent<MonsterStat>())
        {
            // 수락한 퀘스트 만큼 반복
            foreach (QuestData a_QuestData in Managers.Game.CurrentQuest)
            {
                // 오브젝트 id가 퀘스트 타겟 id와 일치하는지
                if (a_QuestData.targetId == a_Obj.GetComponent<MonsterStat>().Id)
                {
                    // 퀘스트 목표 횟수 ++
                    a_QuestData.currnetTargetCount++;

                    // 퀘스트 완료
                    if (a_QuestData.currnetTargetCount == a_QuestData.targetCount)
                    {
                        // 안내문 생성
                        string message = $"퀘스트 완료!\n<color=yellow>[{a_QuestData.titleName}]</color>\n\n\n\n\n\n\n\n\n";
                        Managers.UI.MakeSubItem<UI_Guide>().SetInfo(message, Color.green);
                    }

                    return;
                }
            }
        }
    }

    // 수락한 퀘스트 버튼 누를 시 퀘스트 정보 활성화
    public void OnQuest(QuestData a_Quest)
    {
        if (a_Quest.IsNull() == true)
        {
            Debug.Log("OnQuest() : quest Null");
            return;
        }

        m_CurClickQuest = a_Quest;

        // quest 정보 불러오기
        GetText((int)Texts.QuestTitleText).text = a_Quest.titleName;
        GetText((int)Texts.QuestDescText).text = a_Quest.description;
        GetText((int)Texts.QuestTargetText).text = a_Quest.targetDescription;
        GetText((int)Texts.QuestRewardGoldText).text = a_Quest.rewardGold.ToString();
        GetText((int)Texts.QuestRewardExpText).text = a_Quest.rewardExp.ToString();

        // 아이템 보상 초기화
        foreach (Transform a_Child in GetObject((int)Gameobejcts.QuestRewardGrid).transform)
            Managers.Resource.Destroy(a_Child.gameObject);

        // 아이템 보상 생성
        for (int i = 0; i < a_Quest.rewardItems.Count; i++)
        {
            UI_ItemSlot rewardItem = Managers.UI.MakeSubItem<UI_ItemSlot>(parent: GetObject((int)Gameobejcts.QuestRewardGrid).transform);
            rewardItem.SetInfo();
            rewardItem.AddItem(Managers.Data.Item[a_Quest.rewardItems[i].ItemId], a_Quest.rewardItems[i].itemCount);
        }

        GetObject((int)Gameobejcts.QuestJournal).SetActive(true);
    }

    // 씬에 퀘스트 알림 추가
    public bool SetQuestNotice(QuestData a_Quest)
    {
        // 알람 최대 개수 확인
        if (QuestNoticeList.Count > m_MaxQuestNoticeCount) return false;

        // Scene UI 알람 추가
        QuestNoticeList.Add(Managers.Game._playScene.SetQuestNoticeBar(a_Quest));
        GetText((int)Texts.QuestNoticeCountText).text = QuestNoticeList.Count + " / " + m_MaxQuestNoticeCount;

        return true;
    }

    // 씬 퀘스트 알람 끄기
    public void CloseQuestNotice(QuestData a_Quest)
    {
        // 등록된 알람만큼 반복
        foreach (UI_QuestNoticeSlot questNoticeSlot in QuestNoticeList)
        {
            // 요청한 퀘스트가 같으면 삭제
            if (questNoticeSlot.m_Quest == a_Quest)
            {
                QuestNoticeList.Remove(questNoticeSlot);
                Managers.Resource.Destroy(questNoticeSlot.gameObject);
                break;
            }
        }

        GetText((int)Texts.QuestNoticeCountText).text = QuestNoticeList.Count + " / " + m_MaxQuestNoticeCount;
    }

    // 퀘스트창 활성화
    void OnQuestPopup()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Managers.Game.isPopups[Define.Popup.Quest] = !Managers.Game.isPopups[Define.Popup.Quest];

            // 퀘스트 Popup On/Off
            if (Managers.Game.isPopups[Define.Popup.Quest])
            {
                Managers.UI.OnPopupUI(this);
                RefreshUI();
            }
            else
                Managers.UI.ClosePopupUI(this);
        }
    }

    void SetInfo()
    {
        // 버튼 기능 등록
        GetButton((int)Buttons.ExitButton).onClick.AddListener(() => { Managers.UI.ClosePopupUI(this); });

        // 미리보기 삭제
        foreach (Transform child in GetObject((int)Gameobejcts.Content).transform)
            Managers.Resource.Destroy(child.gameObject);

        // 진행 중인 퀘스트 알람에 등록
        for (int i = 0; i < Managers.Game.CurrentQuest.Count; i++)
            SetQuestNotice(Managers.Game.CurrentQuest[i]);

        GetObject((int)Gameobejcts.QuestJournal).SetActive(false);
    }

    void RefreshUI()
    {
        // 현재 퀘스트 확인
        Managers.Game.RefreshQuest();

        GetText((int)Texts.QuestNoticeCountText).text = QuestNoticeList.Count + " / " + m_MaxQuestNoticeCount;

        // 퀘스트가 없으면 퀘스트 정보 비활성화
        if (Managers.Game.CurrentQuest.Count == 0)
            GetObject((int)Gameobejcts.QuestJournal).SetActive(false);

        // 퀘스트 목록 초기화
        foreach (Transform a_Child in GetObject((int)Gameobejcts.Content).transform)
            Managers.Resource.Destroy(a_Child.gameObject);

        // 퀘스트 목록 채우기
        foreach (QuestData a_QuestData in Managers.Game.CurrentQuest)
        {
            UI_QuestButton a_QuestSlot = Managers.UI.MakeSubItem<UI_QuestButton>(parent: GetObject((int)Gameobejcts.Content).transform);
            a_QuestSlot.SetInfo(a_QuestData);
        }

        // 현재 첫 번째 퀘스트 정보 활성화
        if (Managers.Game.CurrentQuest.Count >= 1)
            OnQuest(Managers.Game.CurrentQuest[0]);
    }
}
