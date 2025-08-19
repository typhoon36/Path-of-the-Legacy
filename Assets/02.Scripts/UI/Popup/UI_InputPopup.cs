using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;


public class UI_InputPopup : UI_Popup
{
    enum Buttons { NoButton, YesButton, }

    [SerializeField] InputField m_InputField;

    [SerializeField] Text m_MessageText;

    string m_Regex;         // 입력 확인을 위한 정규식

    public override bool Init()
    {
        if (base.Init() == false) return false;

        BindButton(typeof(Buttons));

        // 버튼 기능 등록
        GetButton((int)Buttons.YesButton).onClick.AddListener(OnClickYesButton);
        GetButton((int)Buttons.NoButton).onClick.AddListener(OnClickNoButton);

        return true;
    }

    // 설정
    Action<string> IsClickYesButton;
    Action IsClickNoButton;
    public void SetInfo(Action<string> a_ClickYesButton, string messageText, string placeholderText, string regex, Action onClickNoButton = null)
    {
        IsClickYesButton = a_ClickYesButton;
        IsClickNoButton = onClickNoButton;
        m_MessageText.text = messageText;
        m_Regex = regex;

        m_InputField.placeholder.GetComponent<Text>().text = placeholderText;
        m_InputField.Select();
    }

    void OnClickYesButton()
    {
        Regex a_Regex = new Regex(m_Regex);

        // 입력값이 정규식에 맞는지 확인
        if (a_Regex.IsMatch(m_InputField.text))
        {
            // 입력값이 정규식에 맞으면 팝업 닫기
            Managers.UI.ClosePopupUI(this);

            // 확인 기능 실행
            if (IsClickYesButton.IsNull() == false)
                IsClickYesButton.Invoke(m_InputField.text);
        }
        // 입력값이 정규식에 맞지 않으면 경고문 생성
        else
        {
            
            Managers.UI.MakeSubItem<UI_Guide>().SetInfo("한글|영어|숫자 2글자 이상 8글자 이하", Color.red);
        }
    }

    void OnClickNoButton()
    {

        //취소했으니 팝업 닫기
        Managers.UI.ClosePopupUI(this);

        // 취소 기능 실행
        if (IsClickNoButton.IsNull() == false) IsClickNoButton.Invoke();
    }
}
