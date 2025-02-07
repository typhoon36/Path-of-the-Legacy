using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EffParticles : Effects
{
    Action IsParticleColl;     // ��ƼŬ ���� �� �����ų ��� //Action�� ��������Ʈ�μ� �Ű������� ���� �޼��带 ��������.

    // ����
    public void SetInfo(Action a_ParticleColl)
    {
        IsParticleColl = a_ParticleColl;
    }

    // ��ƼŬ ���� �� ȣ��
    void ParticleCollider()
    {
        if (IsParticleColl != null)
        {
            IsParticleColl.Invoke();//Invoke�� IsParticleColl�� ����
            IsParticleColl = null;//������ ������ null�� �ʱ�ȭ
        }
    }

    // ��ƼŬ ���� Ȯ��
    void OnParticleCollision(GameObject coll)
    {

        if (coll.CompareTag("Player"))
            ParticleCollider();//�÷��̾ ���������� �̺�Ʈ ����
    }
}
