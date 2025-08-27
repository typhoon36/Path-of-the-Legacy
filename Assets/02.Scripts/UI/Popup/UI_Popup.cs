using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// 모든 Popup의 부모
public class UI_Popup : UI_Base
{
    public Define.Popup popupType = Define.Popup.Unknown;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        // Canvas 생성
        Managers.UI.SetCanvas(gameObject, true);
        return true;
    }
}
