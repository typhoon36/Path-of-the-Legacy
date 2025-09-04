using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


//확장 기능
public static class Extension
{
    public static T GetOrAddComponent<T>(this GameObject a_Obj) where T : UnityEngine.Component
    {
        return Util.GetOrAddComponent<T>(a_Obj);
    }

    public static void BindEvent(this GameObject a_Obj, Action<PointerEventData> a_Action, Define.UIEvent a_Type = Define.UIEvent.Click)
    {
        UI_Base.BindEvent(a_Obj, a_Action, a_Type);
    }

    // 참조형식(Reference) null 체크
    public static bool IsNull(this UnityEngine.Object a_Obj) { return ReferenceEquals(a_Obj, null); }
    public static bool IsNull(this System.Object a_Obj) { return ReferenceEquals(a_Obj, null); }

    // Fake Null 체크
    public static bool IsFakeNull(this UnityEngine.Object a_Obj) { return (a_Obj.IsNull() == false && a_Obj == true) == false; }

    // 객체 유효성 확인
    public static bool isValid(this GameObject a_Obj) { return a_Obj.IsNull() == false && a_Obj.activeSelf == true; }
}

