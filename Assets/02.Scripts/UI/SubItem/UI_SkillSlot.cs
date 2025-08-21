using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public class UI_SkillSlot : UI_Slot
{
    public SkillData        m_SkillData;

    // 스킬이 등록된 상태라면 마우스로 들기 가능.
    protected override void OnBeginDragSlot(PointerEventData eventData)
    {
        if (m_SkillData.IsNull() == true) return;

        UI_DragSlot.Inst.m_DragSlot = this;
        UI_DragSlot.Inst.DragSetImage(Icon);

        UI_DragSlot.Inst.Icon.transform.position = eventData.position;
    }

    // 마우스 드래그 방향으로 이동
    protected override void OnDragSlot(PointerEventData eventData)
    {
        if (m_SkillData.IsNull() == false)
            UI_DragSlot.Inst.Icon.transform.position = eventData.position;
    }

    protected override void OnEndDragSlot(PointerEventData eventData)
    {
        UI_DragSlot.Inst.SetColor(0);
        UI_DragSlot.Inst.m_DragSlot = null;
    }
}
