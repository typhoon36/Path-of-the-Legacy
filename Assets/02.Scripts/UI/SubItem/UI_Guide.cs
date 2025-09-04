using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;



// 안내문, 경고문 등 상황에 띄울 수 있는 가이드 UI
public class UI_Guide : UI_Base
{
    [SerializeField] Text m_MessageText;
    Color m_Color;
    Coroutine co;

    public void SetInfo(string messageText, Color a_Color)
    {
        // 초기화
        m_MessageText.text = messageText;
        m_MessageText.transform.localPosition = Vector3.zero;
        m_Color = a_Color;
        m_MessageText.color = m_Color;

        if (co.IsNull() == false) StopCoroutine(co);
        co = StartCoroutine(MessageCoroutine());
    }

    IEnumerator MessageCoroutine()
    {
        yield return new WaitForSeconds(1f);

        // 점점 사라지며 올라가기
        for (float i = 1.0f; i >= 0.0f; i -= 0.01f)
        {
            m_Color.a = i;
            m_MessageText.color = m_Color;

            m_MessageText.transform.localPosition += Vector3.up * 0.7f;

            yield return null;
        }

        Managers.Resource.Destroy(gameObject);
    }
}
