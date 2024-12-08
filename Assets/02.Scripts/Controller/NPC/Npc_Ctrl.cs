using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Npc_Ctrl : Base_Ctrl
{
    [SerializeField]
    protected string m_Name;//NPC의 이름

    [SerializeField]
    protected float m_Range;//NPC의 사정거리

    public override void Init()
    {
       
    }
    public void Interact() { }

    protected override void Idle()
    {
        
    }
   
    protected virtual void OpenPopup() { }
    protected virtual void ExitPopup() { }


    // 상호작용 기능
    protected virtual void OnInteract()
    {
        //Managers.Game.IsInteract = !Managers.Game.IsInteract;

        // 상호작용 시작
        //if (Managers.Game.IsInteract)
        //{
            // 모든 팝업 비활성화 및 플레이어 정지
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

        // 상호작용 중이라면
        //if (Managers.Game.IsInteract == true)
        //{
        //    // Esc Key 상호작용 종료
        //    if (Input.GetKeyDown(KeyCode.Escape))
        //        OnInteract();
        //}
    }

}
