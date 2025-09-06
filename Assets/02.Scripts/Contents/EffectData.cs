using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Effect 데이터
public class EffectData : Effect
{
    public int      Id;
    public float    DisableDelayTime = 0;   // effect 전용 비활성화 딜레이

     bool    IsEffect = false;       // 이펙트가 실행 중인가?

    // PlayerController에서 스킬 이펙트 비활성화를 위해 호출
    public void EffectDisableDelay()
    {
        if (!gameObject.activeInHierarchy)
            return;


        // 딜레이 시간이 0이라면 바로 비활성화
        if (DisableDelayTime == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        // 이펙트가 실행 중이 아니면
        if (IsEffect == false)
        {
            // disableDelayTime 동안 부모와 상속 해제
            StopCoroutine(EffectDisableDelayTime());
            StartCoroutine(EffectDisableDelayTime());
        }
    }

    // 플레이어가 움직이더라도 스킬 이펙트가 활성화되야 한다면 사용
    IEnumerator EffectDisableDelayTime()
    {
        IsEffect = true;

        Transform a_EffParent = transform.parent;   // 이펙트 부모
        Vector3 a_EffPos = transform.localPosition; // 이펙트 위치

        // 부모 빠져나오기
        transform.SetParent(null);

        // 이펙트 비활성화 기다리기
        yield return new WaitForSeconds(DisableDelayTime);

        // 원위치 이동 후 비활성화
        transform.SetParent(a_EffParent);
        transform.localPosition = a_EffPos;
        transform.localRotation = Quaternion.identity;

        IsEffect = false;

        gameObject.SetActive(false);
    }
}
