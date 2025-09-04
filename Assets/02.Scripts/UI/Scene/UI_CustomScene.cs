using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 커스텀 씬 UI
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

    public CharacterCustom custom;

    public override bool Init()
    {
        if (base.Init() == false) return false;

        // 자식 객체 불러오기
        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));

        // 커스텀할 캐릭터 찾기
        if (custom.IsNull() == true)
            custom = GameObject.FindObjectOfType<CharacterCustom>();

        // 커스텀 버튼에 캐릭터 객체 보내주기
        foreach (Transform a_Child in GetObject((int)GameObjects.Grid).transform)
            a_Child.GetComponent<UI_CustomButton>().SetInfo(custom);

        // 버튼 기능 등록
        GetButton((int)Buttons.CheckButton).onClick.AddListener(OnClickCheckButton);
        GetButton((int)Buttons.ExitButton).onClick.AddListener(OnClickExitButton);

        return true;
    }

    // 커스텀이 끝날 때 확인 버튼
    void OnClickCheckButton()
    {
        // 캐릭터 회전 중지
        custom.StopRotation = true;
        custom.SaveCustom();

        // 입력 Popup 생성 후 이름 받기
        Managers.UI.ShowPopupUI<UI_InputPopup>().SetInfo((string inputText) =>
        {
            Managers.Game.Name = inputText;
            LoadPopup();
        }
        , "이름을 입력해 주세요", "이름 입력란", Define.NameRegex
        , () =>
        {
            custom.StopRotation = false;
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
    void OnClickExitButton() { Managers.Scene.LoadScene(Define.Scene.Title); }
}
