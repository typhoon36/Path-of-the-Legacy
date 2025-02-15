using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EffParticles : Effects
{
    Action IsParticleColl;     // 파티클 접촉 시 실행시킬 기능 //Action은 델리게이트로서 매개변수가 없는 메서드를 참조가능.

    // 설정
    public void SetInfo(Action a_ParticleColl)
    {
        IsParticleColl = a_ParticleColl;
    }

    // 파티클 접촉 시 호출
    void ParticleCollider()
    {
        if (IsParticleColl != null)
        {
            IsParticleColl.Invoke();//Invoke로 IsParticleColl을 실행
            IsParticleColl = null;//실행이 끝나면 null로 초기화
        }
    }

    // 파티클 접촉 확인
    void OnParticleCollision(GameObject coll)
    {

        if (coll.CompareTag("Player"))
            ParticleCollider();//플레이어가 접촉했으니 이벤트 실행
    }
}
