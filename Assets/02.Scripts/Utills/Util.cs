using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 유틸리티 함수 모음
public class Util
{
    // 컴포넌트 찾은 후 추가하기
    public static T GetOrAddComponent<T>(GameObject a_Obj) where T : UnityEngine.Component
    {
        T component = a_Obj.GetComponent<T>();

        if (component.IsNull() == true)
            component = a_Obj.AddComponent<T>();

        return component;
    }

    // GameObjec는 컴포넌트가 아니므로 Transform을 통해 찾음
    public static GameObject FindChild(GameObject a_Obj, string a_Name = null, bool IsDecision = false)
    {
        Transform transform = FindChild<Transform>(a_Obj, a_Name, IsDecision);

        if (transform.IsNull() == true) return null;

        return transform.gameObject;
    }

    // 자식 객체 컴포넌트 찾기
    public static T FindChild<T>(GameObject a_Obj, string a_Name = null, bool IsDecision = false) where T : UnityEngine.Object
    {
        if (a_Obj.IsNull() == true) return null;

        // IsDecision : 자기 자신의 자식 객체를 가져올지 판단
        if (IsDecision == false)
        {
            // a_Obj의 자식객체 수 만큼
            for (int i = 0; i < a_Obj.transform.childCount; i++)
            {
                // 지정된 자식객체를 transform에 반환
                Transform transform = a_Obj.transform.GetChild(i);

                // string.IsNullOrEmpty = 빈문자열이면 true (null 또는 "")
                if (string.IsNullOrEmpty(a_Name) || transform.name == a_Name)
                {
                    // 해당 T(Button, Text, ...) 컴포넌트 반환
                    T component = transform.GetComponent<T>();
                    if (component.IsNull() == false)
                        return component;
                }
            }
        }
        else
        {
            // true일 경우 자식의 자식까지 다 가져온다.
            foreach (T component in a_Obj.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(a_Name) || component.name == a_Name)
                    return component;
            }
        }

        return null;
    }
}
