using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Drag_UI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Transform m_Canvas;
    Transform m_PrevParent;
    RectTransform m_Rect;
    CanvasGroup m_CanvasGroup;
    ScrollRect m_ScrollRect;

    void Awake()
    {
        m_Canvas = FindObjectOfType<Canvas>().transform;
        m_Rect = GetComponent<RectTransform>();
        m_CanvasGroup = GetComponent<CanvasGroup>();
        m_ScrollRect = GetComponentInParent<ScrollRect>();
    }

    // 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 빈 슬롯시 드래그 불가능
        if (!gameObject.activeSelf) return;

        m_PrevParent = transform.parent;

        // 현재 드래그 중인 UI가 화면의 최상단에 보이도록 함
        transform.SetParent(m_Canvas);
        transform.SetAsLastSibling();

        // 드래그 가능한 오브젝트가 하나가 아닌 자식들을 가지고있을수있기에 CanvasGroup을 통해 통제
        // 알파값을 0.6으로 설정 후 RayCast가 되지않도록
        m_CanvasGroup.alpha = 0.6f;
        m_CanvasGroup.blocksRaycasts = false;

        // 드래그 중에 ScrollRect 비활성화
        if (m_ScrollRect != null)
            m_ScrollRect.enabled = false;
    }

    // 드래그 중
    public void OnDrag(PointerEventData eventData)
    {
        m_Rect.position = eventData.position; // 드래그 중인 UI의 위치를 마우스 위치로 이동
    }

    // 드래그 끝
    public void OnEndDrag(PointerEventData eventData)
    {
        // 엉뚱한 곳에 드롭을 했다는 뜻이기 때문에 드래그 직전에 소속된 슬롯으로 이동
        if (transform.parent == m_Canvas)
        {
            transform.SetParent(m_PrevParent);
            m_Rect.position = m_PrevParent.GetComponent<RectTransform>().position;
        }

        // 원래대로 복구
        m_CanvasGroup.alpha = 1f;
        m_CanvasGroup.blocksRaycasts = true;

        // 드래그가 끝난 후 ScrollRect 활성화
        if (m_ScrollRect != null)
            m_ScrollRect.enabled = true;

        // 장착 슬롯으로 드롭되었는지 확인
        if (transform.parent.CompareTag("EquipSlot"))
        {
            EqStatPopup_UI.Inst.SetEquip(gameObject);
        }
        else
        {
            EqStatPopup_UI.Inst.RemoveEquip(gameObject);
        }
    }
}
