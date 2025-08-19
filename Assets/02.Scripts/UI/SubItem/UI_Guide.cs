using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class UI_Guide : UI_Base
{
    [SerializeField]
     Text     _messageText;
     Color               _color;
     Coroutine           co;

    public void SetInfo(string messageText, Color color)
    {
        // 초기화
        _messageText.text = messageText;
        _messageText.transform.localPosition = Vector3.zero;
        _color = color;
        _messageText.color = _color;

        if (co.IsNull() == false) StopCoroutine(co);
        co = StartCoroutine(MessageCoroutine());
    }

     IEnumerator MessageCoroutine()
    {
        yield return new WaitForSeconds(1f);

        // 점점 사라지며 올라가기
        for(float i=1.0f; i>=0.0f; i-=0.01f)
        {
            _color.a = i;
            _messageText.color = _color;

            _messageText.transform.localPosition += Vector3.up * 0.7f;

            yield return null;
        }
        
        Managers.Resource.Destroy(gameObject);
    }
}
