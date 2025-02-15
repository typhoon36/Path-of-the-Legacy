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

    // �������� �� ��ƼŬ �÷���
    void OnEnable()
    {
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
    }

    // �������� �� ��ƼŬ ��ž
    void OnDisable()
    {
        if (particleSystem != null)
        {
            particleSystem.Stop();
        }
    }
}
