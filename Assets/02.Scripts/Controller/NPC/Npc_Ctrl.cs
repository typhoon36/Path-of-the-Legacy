using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Npc_Ctrl : Base_Ctrl
{
    [SerializeField]
    protected string m_NpcName; // NPC의 이름

    [SerializeField]
    protected float m_Range; // NPC의 사정거리

    [SerializeField]
    protected Text m_NameTxt; // NPC의 이름을 표시할 텍스트

    protected bool IsInteract = false; // 상호작용 중인지 체크

    public override void Init()
    {
        if (m_NameTxt != null)
        {
            m_NameTxt.text = m_NpcName + "[F]";
        }
    }

    public void GetInteract() { OnInteract(); }

    protected override void Idle()
    {
        if (m_Target == null) return;

        // 플레이어와의 거리 좌표
        Vector3 a_Dir = m_Target.transform.position - transform.position;

        // 거리 체크
        if (a_Dir.magnitude <= m_Range)
        {
            // 상호작용 체크
            InteractCheck();

            if (m_NameTxt != null)
            {
                m_NameTxt.gameObject.SetActive(true); // 이름바 활성화
            }

            // 방향 설정
            a_Dir.y = 0;
            transform.rotation = Quaternion.LookRotation(a_Dir);
        }
        else
        {
            m_Target = null;
            if (m_NameTxt != null)
            {
                m_NameTxt.gameObject.SetActive(false);
            }
        }
    }

    protected virtual void OnInteract()
    {
        IsInteract = !IsInteract;

        // 상호작용 시작
        if (IsInteract)
        {
            // 모든 팝업 비활성화
            ConfirmPopup_UI.Inst.m_ConfirmObj.SetActive(false);
            EqStatPopup_UI.Inst.m_EquipPopup.SetActive(false);
            EqStatPopup_UI.Inst.m_StatPopup.SetActive(false);

            OpenPopup(); // Popup Open
        }
        else
        {
            ExitPopup(); // Popup Exit
        }
    }

    protected virtual void OpenPopup() { }
    protected virtual void ExitPopup() { }

    // 플레이어가 가까이 있다면 상호작용 가능
    private void InteractCheck()
    {
        if (Input.GetKeyDown(KeyCode.F))
            OnInteract();

        // 상호작용 중이라면
        if (IsInteract == true)
        {
            // Esc Key로 상호작용 종료
            if (Input.GetKeyDown(KeyCode.Escape))
                OnInteract();
        }
    }
}
