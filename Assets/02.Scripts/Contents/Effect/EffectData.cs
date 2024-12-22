using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectData : Effects
{
    public int id;
    public float m_DisableTime = 0;   // effect 전용 비활성화 딜레이

    bool IsEffect = false;       // 이펙트가 실행 중인가?
    Coroutine disableCoroutine;  // 비활성화 코루틴 참조

    // ~ PlayerController.cs 에서 스킬 이펙트 비활성화를 위해 호출
    public void EffectDisableDelay()
    {
        // 딜레이 시간이 0이라면 바로 비활성화
        if (m_DisableTime == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        // 이펙트가 실행 중이 아니면
        if (IsEffect == false)
        {
            // disableDelayTime 동안 부모와 상속 해제
            if (disableCoroutine != null)
            {
                StopCoroutine(disableCoroutine);
            }
            disableCoroutine = StartCoroutine(EffectDisableDelayTime());
        }
    }

    // 플레이어가 움직이더라도 스킬 이펙트가 활성화되야 한다면 사용
    IEnumerator EffectDisableDelayTime()
    {
        IsEffect = true;

        Transform effectParent = transform.parent;   // 이펙트 부모
        Vector3 effectPos = transform.localPosition; // 이펙트 위치

        // 부모 빠져나오기
        transform.SetParent(null);

        // 이펙트 비활성화 기다리기
        yield return new WaitForSeconds(m_DisableTime);

        // 원위치 이동 후 비활성화
        transform.SetParent(effectParent);
        transform.localPosition = effectPos;
        transform.localRotation = Quaternion.identity;

        IsEffect = false;

        gameObject.SetActive(false);
    }
}
