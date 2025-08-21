using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class Npc_Ctrl : Base_Ctrl
{
    [SerializeField] protected string npcName;    // npc 이름           
    [SerializeField] protected float scanRange;  // 플레이어 스캔 사거리

    protected UI_NameBar nameBarUI;  // 이름바 UI

    public override void Init()
    {
        // 이름바 생성 및 이름 설정
        nameBarUI = Managers.UI.MakeWorldSpaceUI<UI_NameBar>(transform);
        nameBarUI.m_NameText = npcName + " [F]";
    }

    // 상호작용 외부 접근
    public void GetInteract() { OnInteract(); }

    protected override void UpdateIdle()
    {
        // 플레이어 Null Check
        if (Managers.Game.GetPlayer().IsNull() == true)
            return;

        // 플레이어와의 거리 좌표
        Vector3 direction = Managers.Game.GetPlayer().transform.position - transform.position;

        // 거리 체크
        if (direction.magnitude <= scanRange)
        {
            // 상호작용 체크
            InteractCheck();

            m_LockTarget = Managers.Game.GetPlayer();    // 타겟 설정
            nameBarUI.gameObject.SetActive(true);       // 이름바 활성화

            // 방향 설정
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction);
        }
        else
        {
            m_LockTarget = null;
            nameBarUI.gameObject.SetActive(false);
        }
    }

    // 상호작용 기능
    protected virtual void OnInteract()
    {
        Managers.Game.IsInteract = !Managers.Game.IsInteract;

        // 상호작용 시작
        if (Managers.Game.IsInteract)
        {
            // 모든 팝업 비활성화 및 플레이어 정지
            Managers.UI.CloseAllPopupUI();
            Managers.Game.StopPlayer();

            OpenPopup();    // Popup Open
        }
        else
            ExitPopup();    // Popup Exit
    }

    // Popup Open/Exit
    protected virtual void OpenPopup() { }
    protected virtual void ExitPopup() { }

    // 플레이어가 가까이 있다면 상호작용 가능
    void InteractCheck()
    {
        if (Input.GetKeyDown(KeyCode.G))
            OnInteract();

        // 상호작용 중이라면
        if (Managers.Game.IsInteract == true)
        {
            // Esc Key 상호작용 종료
            if (Input.GetKeyDown(KeyCode.Escape))
                OnInteract();
        }
    }
}
