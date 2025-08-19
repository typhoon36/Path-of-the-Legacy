using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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

        //자식 객체 불러오기
        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));

        //커스텀 객체가 없으면 찾기
        if (m_Custom.IsNull() == true)
            m_Custom = GameObject.FindObjectOfType<CharacterCustom>();

        //Grid 오브젝트 자식인 커스텀 버튼  정보 설정
        foreach (Transform a_Child in GetObject((int)GameObjects.Grid).transform)
            a_Child.GetComponent<UI_CustomButton>().SetInfo(m_Custom);

        //버튼 이벤트 설정
        GetButton((int)Buttons.CheckButton).onClick.AddListener(OnClickCheckButton);
        GetButton((int)Buttons.ExitButton).onClick.AddListener(OnClickExitButton);

        return true;
    }

    //확인 버튼 
    void OnClickCheckButton()
    {
        // 커스텀 정보 저장
        m_Custom.IsStopRot = true;
        m_Custom.SaveCustom();

        // 이름 입력 Popup 생성
        Managers.UI.ShowPopupUI<UI_InputPopup>().SetInfo((string a_InputText) =>
        {
            Managers.Game.Name = a_InputText;
            LoadPopup();
        }
        , "닉네임을 입력해 주세요", "닉네임 입력란", Define.NameRegex
        , () =>
        {
            m_Custom.IsStopRot = false;
        });
    }

    // 로딩 팝업 생성
    void LoadPopup()
    {
        // 인터넷 연결 확인

        if (Application.internetReachability == NetworkReachability.NotReachable)
            Managers.UI.MakeSubItem<UI_Guide>().SetInfo("인터넷 연결 끊김.", Color.red);

        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            Managers.UI.ShowPopupUI<UI_LoadPopup>().SetInfo(Define.Scene.Game, 6);

        else
            Managers.UI.ShowPopupUI<UI_LoadPopup>().SetInfo(Define.Scene.Game, 7);
    }

    // 종료 버튼
    void OnClickExitButton() { Managers.Scene.LoadScene(Define.Scene.Title); }
}
