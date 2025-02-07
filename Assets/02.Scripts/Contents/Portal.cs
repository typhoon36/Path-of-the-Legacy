using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    float m_ScanRange = 10f;   // �÷��̾� ��ĵ �Ÿ�
    bool IsPortal = false;  // ��Ż ���� ����

    [SerializeField] Define_S.Scene m_Scene;          // Load�� Scene Ÿ��

    [SerializeField] GameObject m_PortalObj;       // ��Ż ��ü

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
            Vector3 a_Dir = coll.transform.position - this.transform.position;
            if (a_Dir.sqrMagnitude <= m_ScanRange * 10)
            {
                IsPortal = true;
                ConfirmPopup_UI.Inst.ShowPopup(m_Scene);
            }
        }
    }
}
