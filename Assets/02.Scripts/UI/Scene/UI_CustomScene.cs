using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   UI_CustomScene.cs
 * Desc :   캐릭터 커스텀 Scene UI
 *
 & Functions
 &  [Public]
 &  : Init()        - 초기 설정
 &
 &  [Private]
 &  : OnClickCheckButton()  - 커스텀이 끝날 때 확인 버튼 
 &  : LoadPopup()           - Scene을 로드할 Popup 생성
 &  : OnClickExitButton()   - 커스텀 나가기 버튼
 *
 */

public class UI_CustomScene : UI_Scene
{
    enum GameObjects
    {
        Grid,
    }

    enum Buttons
    {
        CheckButton,
        ExitButton,
    }

    public CharacterCustom m_Custom;

    public override bool Init()
    {
        if (base.Init() == false) return false;

        // 자식 객체 불러오기
        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));

        // 커스텀할 캐릭터 찾기
        if (m_Custom.IsNull() == true)
            m_Custom = GameObject.FindObjectOfType<CharacterCustom>();

        // 커스텀 버튼에 캐릭터 객체 보내주기
        foreach (Transform child in GetObject((int)GameObjects.Grid).transform)
            child.GetComponent<UI_CustomButton>().SetInfo(m_Custom);

        // 버튼 기능 등록
        GetButton((int)Buttons.CheckButton).onClick.AddListener(OnClickCheckButton);
        GetButton((int)Buttons.ExitButton).onClick.AddListener(OnClickExitButton);

        return true;
    }

    // 커스텀이 끝날 때 확인 버튼
    void OnClickCheckButton()
    {
        // 캐릭터 회전 중지
        m_Custom.stopRotation = true;
        m_Custom.SaveCustom();

        // 입력 Popup 생성 후 이름 받기
        Managers.UI.ShowPopupUI<UI_InputPopup>().SetInfo((string inputText) =>
        {
            Managers.Game.Name = inputText;
            LoadPopup();
        }
        , "이름을 입력해 주세요", "이름 입력란", Define.NameRegex
        , () =>
        {
            m_Custom.stopRotation = false;
        });
    }

    // Scene을 로드할 Popup 생성
    void LoadPopup()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            // 인터넷 연결이 안되었을 때 행동
            Managers.UI.MakeSubItem<UI_Guide>().SetInfo("네트워크 연결이 필요합니다.", Color.red);
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            // 데이터로 연결이 되었을 때 행동
            Managers.UI.ShowPopupUI<UI_LoadPopup>().SetInfo(Define.Scene.Game, 6);
        }
        else
        {
            // 와이파이로 연결이 되었을 때 행동
            Managers.UI.ShowPopupUI<UI_LoadPopup>().SetInfo(Define.Scene.Game, 7);
        }
    }

    // 커스텀 나가기 버튼
    void OnClickExitButton()
    {
        Managers.Scene.LoadScene(Define.Scene.Title);
    }
}
