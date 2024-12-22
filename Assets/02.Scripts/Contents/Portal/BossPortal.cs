using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPortal : MonoBehaviour
{
    private float m_ScanRange = 10f;   // 플레이어 스캔 거리
    private bool IsPortal = false;  // 포탈 접촉 여부

    [SerializeField]
    private Define_S.Scene m_Scene;          // Load할 Scene 타입

    [SerializeField]
    private GameObject m_PortalObj;       // 포탈 객체

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
            // 플레이어와 포탈의 거리 계산
            Vector3 direction = coll.transform.position - this.transform.position;
            if (direction.sqrMagnitude <= m_ScanRange * 10)
            {
                IsPortal = true;
                ConfirmPopup_UI.Inst.ShowPopup(m_Scene);
                ConfirmPopup_UI.Inst.m_TargetScene = m_Scene;
                ConfirmPopup_UI.Inst.m_ConfirmText.text = "보스룸으로 이동하시겠습니까?";
            }
        }
    }
}
