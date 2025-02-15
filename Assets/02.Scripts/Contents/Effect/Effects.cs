using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
    new ParticleSystem particleSystem;

    void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    // 켜져있을 때 파티클 플레이
    void OnEnable()
    {
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
    }

    // 꺼져있을 때 파티클 스탑
    void OnDisable()
    {
        if (particleSystem != null)
        {
            particleSystem.Stop();
        }
    }
}
