using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EffectParticle : Effect
{
    Action OnParticleColl;

    // 설정
    public void SetInfo(Action IsColl)
    {
        OnParticleColl = IsColl;
    }

    // 파티클 접촉 시 호출
    void ParticleCollider()
    {
        if (OnParticleColl.IsNull() == false)
        {
            OnParticleColl.Invoke();
            OnParticleColl = null;
        }
    }


    void OnParticleCollision(GameObject Coll)
    {
        // 플레이어가 접촉하면 True
        if (Coll.CompareTag("Player")) ParticleCollider();
    }
}
