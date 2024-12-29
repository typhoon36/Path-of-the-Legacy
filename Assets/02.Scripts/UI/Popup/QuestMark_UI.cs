using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestMark_UI : MonoBehaviour
{
    //퀘스트 NPC Text에 ? 혹은 !표시 용도

    [SerializeField]
    private Text m_QuestMark;

    public QuestMark_UI SetInfo(string a_Text, Vector3 a_Pos = new Vector3())
    {
        if (m_QuestMark == null)
        {
            Debug.LogError("m_QuestMark is not assigned.");
            return this;
        }

        m_QuestMark.text = a_Text;
        if (a_Pos != new Vector3())
        {
            transform.position = a_Pos + (Vector3.up * 3.0f);
        }
        return this;
    }
}
