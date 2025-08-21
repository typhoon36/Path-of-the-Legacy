using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class UI_QuestNotice : UI_Base
{
    //TextMeshProGUI를 사용한 이유는 3D내에서도 글자가 선명하게 보이기 위함
    [SerializeField] TextMeshProUGUI    m_NoticeText;    // 알림 text

    public UI_QuestNotice SetInfo(string a_NoticeTxt, Vector3 a_Pos = new Vector3())
    {
        m_NoticeText.text = a_NoticeTxt;

        if (a_Pos != Vector3.zero)
            transform.position = a_Pos + (Vector3.up * 3f);

        return this;
    }
}
