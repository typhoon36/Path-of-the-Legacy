using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Npc_Ctrl : Base_Ctrl
{
    [SerializeField]
    protected string m_Name;//NPC�� �̸�

    [SerializeField]
    protected float m_Range;//NPC�� �����Ÿ�

    public override void Init()
    {
       
    }
    public void Interact() { }

    protected override void Idle()
    {
        
    }
   
    protected virtual void OpenPopup() { }
    protected virtual void ExitPopup() { }


    // ��ȣ�ۿ� ���
    protected virtual void OnInteract()
    {
        //Managers.Game.IsInteract = !Managers.Game.IsInteract;

        // ��ȣ�ۿ� ����
        //if (Managers.Game.IsInteract)
        //{
            // ��� �˾� ��Ȱ��ȭ �� �÷��̾� ����
        //    Managers.UI.CloseAllPopupUI();
        //    Managers.Game.StopPlayer();

        //    OpenPopup();    // Popup Open
        //}
        //else
        //    ExitPopup();    // Popup Exit
    }

    private void InteractCheck()
    {
        //if (Input.GetKeyDown(KeyCode.F))
            //OnInteract();

        // ��ȣ�ۿ� ���̶��
        //if (Managers.Game.IsInteract == true)
        //{
        //    // Esc Key ��ȣ�ۿ� ����
        //    if (Input.GetKeyDown(KeyCode.Escape))
        //        OnInteract();
        //}
    }

}
