using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EffectData : Effect
{
    public int Id;
    public float DisableDelayTime = 0;

     bool IsEffect = false;


    public void EffectDisableDelay()
    {

        if (DisableDelayTime == 0)
        {
            gameObject.SetActive(false);
            return;
        }


        if (IsEffect == false)
        {
            StopCoroutine(EffectDisableDelayTime());
            StartCoroutine(EffectDisableDelayTime());
        }
    }


    IEnumerator EffectDisableDelayTime()
    {
        IsEffect = true;

        Transform a_EffParent = transform.parent;   // 이펙트 부모
        Vector3 a_EffPos = transform.localPosition; // 이펙트 위치


        transform.SetParent(null);


        yield return new WaitForSeconds(DisableDelayTime);


        transform.SetParent(a_EffParent);
        transform.localPosition = a_EffPos;
        transform.localRotation = Quaternion.identity;

        IsEffect = false;

        gameObject.SetActive(false);
    }
}
