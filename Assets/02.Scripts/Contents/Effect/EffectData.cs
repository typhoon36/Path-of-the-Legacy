using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectData : Effects
{
    public int id;
    public float m_DisableTime = 0;   // effect ���� ��Ȱ��ȭ ������

    bool IsEffect = false;       // ����Ʈ�� ���� ���ΰ�?
    Coroutine disableCoroutine;  // ��Ȱ��ȭ �ڷ�ƾ ����

    // ~ PlayerController.cs ���� ��ų ����Ʈ ��Ȱ��ȭ�� ���� ȣ��
    public void EffectDisableDelay()
    {
        // ������ �ð��� 0�̶�� �ٷ� ��Ȱ��ȭ
        if (m_DisableTime == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        // ����Ʈ�� ���� ���� �ƴϸ�
        if (IsEffect == false)
        {
            // disableDelayTime ���� �θ�� ��� ����
            if (disableCoroutine != null)
            {
                StopCoroutine(disableCoroutine);
            }
            disableCoroutine = StartCoroutine(EffectDisableDelayTime());
        }
    }

    // �÷��̾ �����̴��� ��ų ����Ʈ�� Ȱ��ȭ�Ǿ� �Ѵٸ� ���
    IEnumerator EffectDisableDelayTime()
    {
        IsEffect = true;

        Transform effectParent = transform.parent;   // ����Ʈ �θ�
        Vector3 effectPos = transform.localPosition; // ����Ʈ ��ġ

        // �θ� ����������
        transform.SetParent(null);

        // ����Ʈ ��Ȱ��ȭ ��ٸ���
        yield return new WaitForSeconds(m_DisableTime);

        // ����ġ �̵� �� ��Ȱ��ȭ
        transform.SetParent(effectParent);
        transform.localPosition = effectPos;
        transform.localRotation = Quaternion.identity;

        IsEffect = false;

        gameObject.SetActive(false);
    }
}
