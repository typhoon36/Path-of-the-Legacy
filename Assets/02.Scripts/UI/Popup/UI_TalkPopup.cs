using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class UI_TalkPopup : UI_Popup
{
    enum Gameobejcts
    {
        TalkBackground,
        QuestJournal,
        QuestRewardGrid,
    }

    enum Buttons
    {
        NextButton,
        RefusalButton,
        AcceptButton,
    }

    enum Texts
    {
        NameText,
        TalkText,
        QuestTitleText,
        QuestDescText,
        QuestTargetText,
        QuestRewardGoldText,
        QuestRewardExpText,
    }

    TalkData TalkData;               // 대화 데이터
    QuestData QuestData;              // 퀘스트 데이터

    int m_NextTalkIdx = 0;

    bool IsNext = false;         // 다음 대화로 넘어갈 수 있는지
    bool IsNextTalk = false;     // 다음 대화가 있는지

    [SerializeField] float m_TalkDelay = 0.1f;   // 대화 속도 딜레이  

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        popupType = Define.Popup.Talk;

        // 자식 객체 가져오기
        BindObject(typeof(Gameobejcts));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        // 퀘스트 정보 비활성화
        GetObject((int)Gameobejcts.QuestJournal).SetActive(false);

        // 버튼 기능 등록
        GetButton((int)Buttons.NextButton).onClick.AddListener(OnClickNextButton);
        GetButton((int)Buttons.RefusalButton).onClick.AddListener(OnClickRefusalButton);
        GetButton((int)Buttons.AcceptButton).onClick.AddListener(OnClickAcceptButton);

        // 버튼 비활성화
        GetButton((int)Buttons.NextButton).gameObject.SetActive(false);
        GetButton((int)Buttons.RefusalButton).gameObject.SetActive(false);
        GetButton((int)Buttons.AcceptButton).gameObject.SetActive(false);

        Managers.UI.ClosePopupUI(this);

        return true;
    }

    void Update()
    {
        // 상호작용 키, 스페이스 바, 마우스를 좌클릭하면 대화속도가 빨라지고 대화를 넘김.
        if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            // 말이 다 안 끝났다면
            if (IsNext == false)
            {
                // 대화 속도 빠르게
                m_TalkDelay = m_TalkDelay / 2;
                return;
            }

            // 퀘스트가 On이라면
            if (GetObject((int)Gameobejcts.QuestJournal).activeSelf == true) return;

            // 다음 대화가 있으면 진행
            if (IsNextTalk == true)
                OnClickNextButton();
            else
                Clear();
        }
    }

    // 일반 대화 세팅
    public void SetInfo(string text, string npcName = null)
    {
        if (text.IsNull() == false)
        {
            if (npcName.IsNull() == false)
                GetText((int)Texts.NameText).text = npcName;

            // 대화 진행 후 종료
            IsNextTalk = false;
            StartCoroutine(TypingText(text));
            return;
        }
    }

    // 퀘스트 대화 세팅
    public void SetInfo(TalkData a_Talk, QuestData a_Quest, string a_NpcName = null)
    {
        if (a_Talk.IsNull() == true || a_Quest.IsNull() == true)
        {
            Debug.Log("talk or quest Data Null");
            return;
        }

        if (a_NpcName.IsNull() == false)
            GetText((int)Texts.NameText).text = a_NpcName;

        TalkData = a_Talk;
        QuestData = a_Quest;

        m_NextTalkIdx = 0;

        SetQuestUI();       // 퀘스트 정보 설정
        NextTalk();         // 대화 시작
    }

    void NextTalk()
    {
        // 할 대화가 없으면 종료
        if (m_NextTalkIdx >= TalkData.questStartTalk.Count)
        {
            Clear();
            return;
        }

        // 대화 시작
        StartCoroutine(TypingText(TalkData.questStartTalk[m_NextTalkIdx]));

        m_NextTalkIdx++;

        // 다음 대화가 없으면 퀘스트 정보 활성화
        if (m_NextTalkIdx >= TalkData.questStartTalk.Count)
        {
            IsNextTalk = false;
            GetObject((int)Gameobejcts.QuestJournal).SetActive(true);
        }
        else
            IsNextTalk = true;
    }

    // 타이핑 모션 코루틴
    IEnumerator TypingText(string sentence)
    {
        GetText((int)Texts.TalkText).text = "";

        IsNext = false;
        m_TalkDelay = 0.05f;

        // 대화 타이밍 모션 실행
        foreach (var letter in sentence)
        {
            GetText((int)Texts.TalkText).text += letter;
            yield return new WaitForSeconds(m_TalkDelay);
        }

        IsNext = true;

        // 다음 대화가 있다면 다음 버튼 On
        if (IsNextTalk == true)
            GetButton((int)Buttons.NextButton).gameObject.SetActive(true);

        // 퀘스트가 켜지면 수락 or 거절 버튼 On
        if (GetObject((int)Gameobejcts.QuestJournal).activeSelf == true)
            IsQuestActive(true);
    }

    // 다음 버튼
    void OnClickNextButton()
    {
        GetButton((int)Buttons.NextButton).gameObject.SetActive(false);
        NextTalk();
    }

    // 거절 버튼
    void OnClickRefusalButton()
    {
        IsQuestActive(false);
        SetInfo(TalkData.refusalTalk);
    }

    // 수락 버튼
    void OnClickAcceptButton()
    {
        Managers.Game.m_PlayScene.Quest.SetQeust(QuestData);

        IsQuestActive(false);
        SetInfo(TalkData.acceptTalk);

        Managers.UI.MakeWorldSpaceUI<UI_Navigation>().SetInfo(QuestData.TargetPos);
    }

    // 퀘스트 활성화/비활성화
    void IsQuestActive(bool isTrue)
    {
        GetButton((int)Buttons.AcceptButton).gameObject.SetActive(isTrue);
        GetButton((int)Buttons.RefusalButton).gameObject.SetActive(isTrue);
        GetObject((int)Gameobejcts.QuestJournal).SetActive(isTrue);
    }

    // 퀘스트 정보 설정
    void SetQuestUI()
    {
        GetText((int)Texts.QuestTitleText).text = QuestData.TitleName;
        GetText((int)Texts.QuestDescText).text = QuestData.Description;
        GetText((int)Texts.QuestTargetText).text = QuestData.TargetDescription;
        GetText((int)Texts.QuestRewardGoldText).text = QuestData.RewardGold.ToString();
        GetText((int)Texts.QuestRewardExpText).text = QuestData.RewardExp.ToString();

        foreach (Transform child in GetObject((int)Gameobejcts.QuestRewardGrid).transform)
            Managers.Resource.Destroy(child.gameObject);

        for (int i = 0; i < QuestData.RewardItems.Count; i++)
        {
            UI_ItemSlot rewardItem = Managers.UI.MakeSubItem<UI_ItemSlot>(parent: GetObject((int)Gameobejcts.QuestRewardGrid).transform);
            rewardItem.SetInfo();
            rewardItem.AddItem(Managers.Data.Item[QuestData.RewardItems[i].ItemId], QuestData.RewardItems[i].ItemCount);
        }
    }

    public void Clear()
    {
        Managers.Game.IsInteract = false;

        GetObject((int)Gameobejcts.QuestJournal).SetActive(false);
        GetButton((int)Buttons.NextButton).gameObject.SetActive(false);
        GetButton((int)Buttons.RefusalButton).gameObject.SetActive(false);
        GetButton((int)Buttons.AcceptButton).gameObject.SetActive(false);

        Managers.UI.ClosePopupUI(this);
    }
}
