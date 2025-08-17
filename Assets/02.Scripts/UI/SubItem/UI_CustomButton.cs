using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class UI_CustomButton : UI_Base
{
    enum Buttons
    {
        NextButton,
        BackButton,
    }

    public Define.DefaultPart   m_PartType;   // 기본 파츠 타입

    private CharacterCustom     m_Custom;    // 커스텀 캐릭터 Object

    public override bool Init()
    {
        if (base.Init() == false) return false;

        BindButton(typeof(Buttons));

        // ▶ 클릭 버튼
        GetButton((int)Buttons.NextButton).onClick.AddListener(()=>{ m_Custom.NextPart(m_PartType, true); });

        // ◀ 클릭 버튼
        GetButton((int)Buttons.BackButton).onClick.AddListener(()=>{ m_Custom.NextPart(m_PartType, false); });

        return true;
    }

    public void SetInfo(CharacterCustom a_Custom) { m_Custom = a_Custom; }
}
