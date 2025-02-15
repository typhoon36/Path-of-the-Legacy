using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Drop_UI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    Image m_Image;
    RectTransform m_Rect;

    void Awake()
    {
        m_Image = GetComponent<Image>();
        m_Rect = GetComponent<RectTransform>();
    }

    // 드래그 중에 마우스가 들어왔을때
    public void OnPointerEnter(PointerEventData eventData)
    {
        m_Image.color = Color.yellow;
    }
    // 드래그 중에 마우스가 밖으로 나갔을때
    public void OnPointerExit(PointerEventData eventData)
    {
        m_Image.color = Color.white;
    }

    // 드래그 중에 마우스 드롭되었을때
    public void OnDrop(PointerEventData eventData)
    {
        // 드래그 중인 아이템이 있다면
        if (eventData.pointerDrag != null)
        {
            if (transform.CompareTag("EquipSlot"))
            {
                // 드래그 중인 아이템의 부모를 현재 슬롯으로 설정
                eventData.pointerDrag.transform.SetParent(transform);
                // 드래그 중인 아이템의 위치를 현재 슬롯의 위치로 설정
                eventData.pointerDrag.GetComponent<RectTransform>().position = m_Rect.position;

                // 장비 장착
                EqStatPopup_UI.Inst.SetEquip(eventData.pointerDrag);

            }
            // 인벤 슬롯일 경우
            else
            {
                eventData.pointerDrag.transform.SetParent(transform);
                eventData.pointerDrag.GetComponent<RectTransform>().position = m_Rect.position;

                // 장비 해제
                EqStatPopup_UI.Inst.RemoveEquip(eventData.pointerDrag);
            }
        }
    }
}
