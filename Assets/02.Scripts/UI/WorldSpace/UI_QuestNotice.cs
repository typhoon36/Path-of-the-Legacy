using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


 // 퀘스트 NPC에 !, ?를 띄운다.

public class UI_QuestNotice : UI_Base
{
    [SerializeField] TextMeshProUGUI     m_NoticeText;    // 알림 text

    public UI_QuestNotice SetInfo(string a_NoticeText, Vector3 a_Pos = new Vector3())
    {
        m_NoticeText.text = a_NoticeText;

        if (a_Pos != Vector3.zero)
            transform.position = a_Pos + (Vector3.up * 3f);

        return this;
    }
}
