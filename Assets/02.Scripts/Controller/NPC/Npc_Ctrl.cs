using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Npc_Ctrl : Base_Ctrl
{
    [SerializeField]
    protected string m_NpcName; // NPC�� �̸�

    [SerializeField]
    protected float m_Range; // NPC�� �����Ÿ�

    [SerializeField]
    protected Text m_NameTxt; // NPC�� �̸��� ǥ���� �ؽ�Ʈ

    protected bool IsInteract = false; // ��ȣ�ۿ� ������ üũ

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

        // �÷��̾���� �Ÿ� ��ǥ
        Vector3 a_Dir = m_Target.transform.position - transform.position;

        // �Ÿ� üũ
        if (a_Dir.magnitude <= m_Range)
        {
            // ��ȣ�ۿ� üũ
            InteractCheck();

            if (m_NameTxt != null)
            {
                m_NameTxt.gameObject.SetActive(true); // �̸��� Ȱ��ȭ
            }

            // ���� ����
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

        // ��ȣ�ۿ� ����
        if (IsInteract)
        {
            // ��� �˾� ��Ȱ��ȭ
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

    // �÷��̾ ������ �ִٸ� ��ȣ�ۿ� ����
    private void InteractCheck()
    {
        if (Input.GetKeyDown(KeyCode.F))
            OnInteract();

        // ��ȣ�ۿ� ���̶��
        if (IsInteract == true)
        {
            // Esc Key�� ��ȣ�ۿ� ����
            if (Input.GetKeyDown(KeyCode.Escape))
                OnInteract();
        }
    }
}
