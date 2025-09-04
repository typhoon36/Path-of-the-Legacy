using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



// Monster 피격 시 생성되는 데미지 Effect UI

public class UI_HitEffect : UI_Base
{
    public Text m_HitText;

    public float m_UpSpeed = 0.1f;

    void OnEnable()
    {
        StartCoroutine(DelayDisalbe());
    }

    void FixedUpdate()
    {
        m_HitText.transform.rotation = Camera.main.transform.rotation;

        transform.position += Vector3.up * m_UpSpeed * Time.deltaTime;
    }

    IEnumerator DelayDisalbe()
    {
        yield return new WaitForSeconds(2f);

        Managers.Resource.Destroy(gameObject);
    }
}
