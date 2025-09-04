using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 타이틀 화면 UI
public class UI_TitleScene : UI_Scene
{
    enum Buttons
    {
        StartButton,
        LoadButton,
        ExitButton,
    }

    enum Texts
    {
        LoadButtonText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        // 자식 객체 불러오기
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        // 버튼 기능 등록
        GetButton((int)Buttons.StartButton).onClick.AddListener(OnClickStartButton);
        GetButton((int)Buttons.LoadButton).onClick.AddListener(OnClickLoadButton);
        GetButton((int)Buttons.ExitButton).onClick.AddListener(OnClickExitButton);

        // 세이브 로드 여부 확인
        if (Managers.Game.IsSaveLoad() == false)
        {
            Color a_Color = GetText((int)Texts.LoadButtonText).color;
            a_Color.a = 0.5f;
            GetText((int)Texts.LoadButtonText).color = a_Color;

            //세이브 파일이 없으니 주소에 있는 이미지로 변경
            string a_Path = "Art/UI/Classic_RPG_GUI/Parts/mid_button_off";
            GetButton((int)Buttons.LoadButton).GetComponent<Image>().sprite = Managers.Resource.Load<Sprite>(a_Path);
        }

        return true;
    }

    // 시작 버튼
    void OnClickStartButton()
    {
        Managers.Scene.LoadScene(Define.Scene.PlayerCustom);
    }

    // 세이브 로드 버튼
    void OnClickLoadButton()
    {
        if (Managers.Game.LoadGame() == false) return;

        //인터넷 연결 상태 확인
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            // 인터넷 연결이 안되었을 때 행동
            Managers.UI.MakeSubItem<UI_Guide>().SetInfo("네트워크 연결이 필요합니다.", Color.red);
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            // 모바일 네트워크에 연결되었을 때 행동
            Managers.UI.ShowPopupUI<UI_LoadPopup>().SetInfo(Define.Scene.Game, 6);
        }
        else
        {
            // 와이파이에 연결되었을 때 행동
            Managers.UI.ShowPopupUI<UI_LoadPopup>().SetInfo(Define.Scene.Game, 7);
        }
    }

    // 나가기 버튼
    void OnClickExitButton() { Application.Quit(); UnityEditor.EditorApplication.isPlaying = false; }

}
