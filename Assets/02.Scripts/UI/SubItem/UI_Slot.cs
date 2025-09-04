using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public abstract class UI_Slot : UI_Base
{
    enum Images { ItemImage, }

    public Image    icon;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        SetInfo();
        SetEventHandler();

        return true;
    }

    public virtual void SetInfo()
    {
        BindImage(typeof(Images));
        icon = GetImage((int)Images.ItemImage);
    }

    public virtual void RefreshUI() {}

    protected virtual void SetEventHandler()
    {
        gameObject.BindEvent((PointerEventData eventData)=>{ OnEnterSlot(eventData); }, Define.UIEvent.Enter);
        gameObject.BindEvent((PointerEventData eventData)=>{ OnExitSlot(eventData); }, Define.UIEvent.Exit);
        gameObject.BindEvent((PointerEventData eventData)=>{ OnClickSlot(eventData); }, Define.UIEvent.Click);
        gameObject.BindEvent((PointerEventData eventData)=>{ OnBeginDragSlot(eventData); }, Define.UIEvent.BeginDrag);
        gameObject.BindEvent((PointerEventData eventData)=>{ OnDragSlot(eventData); }, Define.UIEvent.Drag);
        gameObject.BindEvent((PointerEventData eventData)=>{ OnEndDragSlot(eventData); }, Define.UIEvent.EndDrag);
        gameObject.BindEvent((PointerEventData eventData)=>{ OnDropSlot(eventData); }, Define.UIEvent.Drop);
    }
    
    protected virtual void OnEnterSlot(PointerEventData eventData) {}        //  마우스 포인터가 나랑 닿을 경우
    protected virtual void OnExitSlot(PointerEventData eventData) {}         //  마우스 포인터가 나에게서 벗어날 경우
    protected virtual void OnClickSlot(PointerEventData eventData) {}        //  마우스 나를 클릭할 경우
    protected virtual void OnBeginDragSlot(PointerEventData eventData) {}    //  마우스 드래그 시작
    protected virtual void OnDragSlot(PointerEventData eventData) {}         //  마우스 드래그 진행
    protected virtual void OnEndDragSlot(PointerEventData eventData) {}      //  마우스 드래그 종료
    protected virtual void OnDropSlot(PointerEventData eventData) {}         //  마우스 드래그가 내 위에서 끝났을 때

    // 투명도 설정 (0 ~ 255)
    protected virtual void SetColor(float a_Alpha)
    {
        Color a_Color = icon.color;
        a_Color.a = a_Alpha;
        icon.color = a_Color;
    }

    // 슬롯 초기화
    public virtual void ClearSlot()
    {
        icon.sprite = null;
        
        SetColor(0);
    }
}
