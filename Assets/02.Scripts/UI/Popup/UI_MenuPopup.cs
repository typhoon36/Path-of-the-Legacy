using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class UI_MenuPopup : UI_Popup
{
    enum Buttons
    {
        ContinueButton,
        SaveButton,
        AppExitButton,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        popupType = Define.Popup.Menu;

        // 자식 객체 불러오기
        BindButton(typeof(Buttons));

        // 버튼 기능 등록
        GetButton((int)Buttons.ContinueButton).onClick.AddListener(OnClickContinueButton);
        GetButton((int)Buttons.SaveButton).onClick.AddListener(OnClickSaveButton);
        GetButton((int)Buttons.AppExitButton).onClick.AddListener(OnClickAppExitButton);

        // InputManager에 입력 등록
        Managers.Input.KeyAction -= OnMenuPopup;
        Managers.Input.KeyAction += OnMenuPopup;

        Managers.UI.ClosePopupUI(this);

        return true;
    }

    // 메뉴 활성화
    void OnMenuPopup()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Managers.Game.isPopups[Define.Popup.Menu] = !Managers.Game.isPopups[Define.Popup.Menu];

            // 메뉴 Popup On/Off
            if (Managers.Game.isPopups[Define.Popup.Menu])
                OnMenu();
            else
                Exit();
        }
    }

    void OnMenu()
    {
        // 현재 활성화 중인 Popup이 없다면
        if (Managers.UI.ClosePopupUI() == false)
        {
            // 메뉴 활성화
            Managers.UI.OnPopupUI(this);
            Time.timeScale = 0;
        }
        else
        {
            // 메뉴 끄기
            Managers.Game.isPopups[Define.Popup.Menu] = false;
            Managers.Game.m_PlayScene.SlotTip.OnSlotTip(false);
        }
    }

    // 게임 진행 버튼
    void OnClickContinueButton()
    {
        Exit();
    }

    // 게임 세이브 버튼
    void OnClickSaveButton()
    {
        Managers.Game.SaveGame();
        Exit();
    }

    // 게임 나가기 버튼
    void OnClickAppExitButton()
    {
        Exit();
        Application.Quit();
    }

    // 초기화
    void Exit()
    {
        Time.timeScale = 1;
        Managers.UI.ClosePopupUI(this);
    }
}
