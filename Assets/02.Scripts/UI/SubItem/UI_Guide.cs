using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class UI_Guide : UI_Base
{
    [SerializeField] Text m_MessageText;
    Color m_Color;
    Coroutine Co;

    public void SetInfo(string messageText, Color color)
    {
        // 초기화
        m_MessageText.text = messageText;
        m_MessageText.transform.localPosition = Vector3.zero;
        m_Color = color;
        m_MessageText.color = m_Color;

        if (Co.IsNull() == false) StopCoroutine(Co);
        Co = StartCoroutine(MessageCoroutine());
    }

    //연출
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

        //연출이 끝나면 오브젝트 제거
        Managers.Resource.Destroy(gameObject);
    }
}
