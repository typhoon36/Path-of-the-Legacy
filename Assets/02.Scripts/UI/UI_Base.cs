using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


// 모든 UI의 부모(UI자동화)
public abstract class UI_Base : MonoBehaviour
{
    // 컴포넌트 타입 별로 담기
    protected Dictionary<Type, UnityEngine.Object[]> m_Objects = new Dictionary<Type, UnityEngine.Object[]>();

    protected bool init = false;

    public virtual bool Init()
    {
        if (init) return false;

        return init = true;
    }

    void Start() { Init(); }

    protected void Bind<T>(Type a_Type) where T : UnityEngine.Object
    {
        // enum 타입이 맞는지 확인
        string[] a_Names = Enum.GetNames(a_Type);

        if (m_Objects.ContainsKey(typeof(T)) == true) return;

        // enum의 개수만큼 배열 생성 후 m_Objects에 추가
        UnityEngine.Object[] a_Objects = new UnityEngine.Object[a_Names.Length];
        m_Objects.Add(typeof(T), a_Objects);

        for (int i = 0; i < a_Names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
                a_Objects[i] = Util.FindChild(gameObject, a_Names[i], true);
            else
                a_Objects[i] = Util.FindChild<T>(gameObject, a_Names[i], true);

            if (a_Objects[i].IsNull() == true)
                Debug.Log($"Failed to bind({a_Names[i]})");
        }
    }

    protected void BindObject(Type a_Type) { Bind<GameObject>(a_Type); }
    protected void BindImage(Type a_Type) { Bind<Image>(a_Type); }
    protected void BindText(Type a_Type) { Bind<Text>(a_Type); }
    protected void BindButton(Type a_Type) { Bind<Button>(a_Type); }

    // 사용 메소드
    protected T Get<T>(int a_Idx) where T : UnityEngine.Object
    {
        // Dictionary의 Value를 받을 변수 생성
        UnityEngine.Object[] a_Objects = null;

        // 해당 Key 컴포넌트에 Value가 존재하는지 확인
        if (m_Objects.TryGetValue(typeof(T), out a_Objects) == false) return null;

        return a_Objects[a_Idx] as T;
    }

    // 자주 사용하는 컴포넌트는 사용하기 좋게 메소드 생성
    protected GameObject GetObject(int a_Idx) { return Get<GameObject>(a_Idx); }
    protected Text GetText(int a_Idx) { return Get<Text>(a_Idx); }
    protected Button GetButton(int a_Idx) { return Get<Button>(a_Idx); }
    protected Image GetImage(int a_Idx) { return Get<Image>(a_Idx); }

    // Event 핸들러에 관한 메소드 (Command 패턴)
    public static void BindEvent(GameObject a_Obj, Action<PointerEventData> a_Action, Define.UIEvent a_Type = Define.UIEvent.Click)
    {
        // 객체에 컴포넌트 추가 및 읽어오기
        // EventSystem 관련 클래스이기 때문에 스크립트를 추가하면 클릭 드래그에 관한 메소드를 바로 사용 가능하다.
        UI_EventHandler evt = Util.GetOrAddComponent<UI_EventHandler>(a_Obj);

        // UI_EventHandler 안에 Action이 있음!
        switch (a_Type)
        {
            case Define.UIEvent.Enter:
                evt.OnEnterHandler -= a_Action;
                evt.OnEnterHandler += a_Action;
                break;
            case Define.UIEvent.Exit:
                evt.OnExitHandler -= a_Action;
                evt.OnExitHandler += a_Action;
                break;
            case Define.UIEvent.Click:
                evt.OnClickHandler -= a_Action;
                evt.OnClickHandler += a_Action;
                break;
            case Define.UIEvent.Drag:
                evt.OnDragHandler -= a_Action;
                evt.OnDragHandler += a_Action;
                break;
            case Define.UIEvent.BeginDrag:
                evt.OnBeginDragHandler -= a_Action;
                evt.OnBeginDragHandler += a_Action;
                break;
            case Define.UIEvent.EndDrag:
                evt.OnEndDragHandler -= a_Action;
                evt.OnEndDragHandler += a_Action;
                break;
            case Define.UIEvent.Drop:
                evt.OnDropHandler -= a_Action;
                evt.OnDropHandler += a_Action;
                break;
        }
    }
}
