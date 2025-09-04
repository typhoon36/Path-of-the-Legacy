using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ParticleSystem의 물리적 접촉이 필요할 때 사용
public class EffectParticle : Effect
{
    Action OnParticleCollider;     // 파티클 접촉 시 실행시킬 기능 저장

    // 설정
    public void SetInfo(Action onParticleColl)
    {
        OnParticleCollider = onParticleColl;
    }

    // 파티클 접촉 시 호출
    void ParticleCollider()
    {
        if (OnParticleCollider.IsNull() == false)
        {
            OnParticleCollider.Invoke();
            OnParticleCollider = null;
        }
    }

    // 파티클 물리적 접촉 확인
    void OnParticleCollision(GameObject coll)
    {
        // 플레이어가 접촉하면 True
        if (coll.CompareTag("Player"))
            ParticleCollider();
    }
}
