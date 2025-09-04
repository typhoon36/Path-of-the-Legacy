using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 포탈 생성 및 Scene 이동
public class Portal : MonoBehaviour
{
    float m_ScanRange = 6f;   // 플레이어 스캔 거리
    bool IsPortal = false;  // 포탈 접촉 여부

    [SerializeField] Define.Scene m_SceneType;          // Load할 Scene 타입

    [SerializeField] GameObject m_PortalObj;       // 포탈 객체

    void OnTriggerEnter(Collider coll)
    {
        // 플레이어 체크
        if (coll.CompareTag("Player"))
        {
            IsPortal = false;
            m_PortalObj.SetActive(true);
        }
    }

    void OnTriggerStay(Collider coll)
    {
        // 포탈 활성화 체크
        if (m_PortalObj.activeSelf == true && IsPortal == false)
        {
            // 플레이어와 포탈이 근접한지 체크
            float a_Dist = (Managers.Game.GetPlayer().transform.position - m_PortalObj.transform.position).magnitude;

            if (a_Dist <= m_ScanRange)
            {
                IsPortal = true;

                // 플레이어 정지
                Managers.Game.StopPlayer();

                // 현재 Scene이 Game Scene라면
                if (Managers.Scene.CurrentScene.SceneType == Define.Scene.Game)
                {
                    // 확인 Popup 활성화
                    UI_ConfirmPopup a_ConfirmPopup = Managers.UI.ShowPopupUI<UI_ConfirmPopup>();
                    if (a_ConfirmPopup.IsNull() == true) return;


                    // 확인 Popup 설정
                    a_ConfirmPopup.SetInfo(() =>
                    {
                        // 게임 세이브
                        Managers.Game.SaveGame();

                        // 씬 이동 전 위치 저장
                        Managers.Game.CurrentPos += Vector3.forward * (-3f);

                        // 씬 로드
                        Managers.Scene.LoadScene(m_SceneType);
                    }, Define.DungeonMessage);
                }
                else
                    Managers.Scene.LoadScene(m_SceneType);
            }
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            m_PortalObj.SetActive(false);
        }
    }
}
