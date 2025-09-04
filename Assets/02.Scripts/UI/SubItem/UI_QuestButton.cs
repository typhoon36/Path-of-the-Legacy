using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;


// 퀘스트에서 퀘스트 버튼 기능
public class UI_QuestButton : UI_Base
{
    enum Buttons { QuestSceneButton }
    enum Images { QuestSceneOkIcon }
    enum Texts { QuestSlotText }

    QuestData m_Quest;

    string SlotText;           // 퀘스트 제목
    bool IsNotice = true;    // 퀘스트 알람

    public override bool Init()
    {
        if (base.Init() == false) return false;

        BindButton(typeof(Buttons));
        BindImage(typeof(Images));
        BindText(typeof(Texts));

        gameObject.BindEvent((PointerEventData eventData) =>
        {
            Managers.Game._playScene._quest.OnQuest(m_Quest);
        });

        GetButton((int)Buttons.QuestSceneButton).onClick.AddListener(OnClickSceneNoticeButton);

        GetImage((int)Images.QuestSceneOkIcon).gameObject.SetActive(!IsNotice);

        GetText((int)Texts.QuestSlotText).text = SlotText;

        return true;
    }

    public void SetInfo(QuestData a_Quest)
    {
        m_Quest = a_Quest;
        SlotText = m_Quest.titleName;
    }

    // 씬에 퀘스트 알림 추가
    void OnClickSceneNoticeButton()
    {
        IsNotice = !IsNotice;

        // 알람 활성화/비활성화
        if (IsNotice == true)
        {
            IsNotice = Managers.Game._playScene._quest.SetQuestNotice(m_Quest);
            GetButton((int)Buttons.QuestSceneButton).image.sprite =
                 Managers.Resource.Load<Sprite>("Art/UI/Classic_RPG_GUI/Parts/Minus");
        }
        else
        {
            Managers.Game._playScene._quest.CloseQuestNotice(m_Quest);
            GetButton((int)Buttons.QuestSceneButton).image.sprite =
                 Managers.Resource.Load<Sprite>("Art/UI/Classic_RPG_GUI/Parts/Plus");
        }

        GetImage((int)Images.QuestSceneOkIcon).gameObject.SetActive(!IsNotice);
    }
}
