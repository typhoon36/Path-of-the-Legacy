using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPortal : MonoBehaviour
{
    private float m_ScanRange = 10f;   // �÷��̾� ��ĵ �Ÿ�
    private bool IsPortal = false;  // ��Ż ���� ����

    [SerializeField]
    private Define_S.Scene m_Scene;          // Load�� Scene Ÿ��

    [SerializeField]
    private GameObject m_PortalObj;       // ��Ż ��ü

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            IsPortal = false;
            m_PortalObj.SetActive(true);
        }
    }

    void OnTriggerStay(Collider coll)
    {
        if (coll.CompareTag("Player") && IsPortal == false)
        {
            // �÷��̾�� ��Ż�� �Ÿ� ���
            Vector3 direction = coll.transform.position - this.transform.position;
            if (direction.sqrMagnitude <= m_ScanRange * 10)
            {
                IsPortal = true;
                ConfirmPopup_UI.Inst.ShowPopup(m_Scene);
                ConfirmPopup_UI.Inst.m_TargetScene = m_Scene;
                ConfirmPopup_UI.Inst.m_ConfirmText.text = "���������� �̵��Ͻðڽ��ϱ�?";
            }
        }
    }
}
