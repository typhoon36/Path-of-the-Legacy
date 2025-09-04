using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;



// 입력이 필요할 때 Popup UI (닉네임 입력 등..)


public class UI_InputPopup : UI_Popup
{
    enum Buttons
    {
        NoButton,
        YesButton,
    }

    [SerializeField] InputField m_InputField;

    [SerializeField] Text m_MessageText;

    string m_Regex;         // 정규식

    public override bool Init()
    {
        if (base.Init() == false) return false;

        BindButton(typeof(Buttons));

        // 버튼 기능 등록
        GetButton((int)Buttons.YesButton).onClick.AddListener(OnClickYesButton);
        GetButton((int)Buttons.NoButton).onClick.AddListener(OnClickNoButton);

        return true;
    }

    // 기능 설정
    Action<string> _onClickYesButton;
    Action _onClickNoButton;
    public void SetInfo(Action<string> a_OnClickYesButton, string messageText, string placeholderText, string regex, Action a_OnClickNoButton = null)
    {
        _onClickYesButton = a_OnClickYesButton;
        _onClickNoButton = a_OnClickNoButton;
        m_MessageText.text = messageText;
        m_Regex = regex;

        m_InputField.placeholder.GetComponent<Text>().text = placeholderText;
        m_InputField.Select();
    }

    void OnClickYesButton()
    {
        Regex a_Regex = new Regex(m_Regex);
        if (a_Regex.IsMatch(m_InputField.text))
        {
            Managers.UI.ClosePopupUI(this);

            // 확인 기능 실행
            if (_onClickYesButton.IsNull() == false)
                _onClickYesButton.Invoke(m_InputField.text);
        }
        else
        {
            // 경고문 생성
            Managers.UI.MakeSubItem<UI_Guide>().SetInfo("한글|영어|숫자 2글자 이상 8글자 이하", Color.red);
        }
    }

    void OnClickNoButton()
    {
        Managers.UI.ClosePopupUI(this);

        // 취소 기능 실행
        if (_onClickNoButton.IsNull() == false)
            _onClickNoButton.Invoke();
    }
}
