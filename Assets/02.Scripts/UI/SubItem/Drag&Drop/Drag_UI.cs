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

    // �巡�� ����
    public void OnBeginDrag(PointerEventData eventData)
    {
        // �� ���Խ� �巡�� �Ұ���
        if (!gameObject.activeSelf) return;

        m_PrevParent = transform.parent;

        // ���� �巡�� ���� UI�� ȭ���� �ֻ�ܿ� ���̵��� ��
        transform.SetParent(m_Canvas);
        transform.SetAsLastSibling();

        // �巡�� ������ ������Ʈ�� �ϳ��� �ƴ� �ڽĵ��� �������������ֱ⿡ CanvasGroup�� ���� ����
        // ���İ��� 0.6���� ���� �� RayCast�� �����ʵ���
        m_CanvasGroup.alpha = 0.6f;
        m_CanvasGroup.blocksRaycasts = false;

        // �巡�� �߿� ScrollRect ��Ȱ��ȭ
        if (m_ScrollRect != null)
            m_ScrollRect.enabled = false;
    }

    // �巡�� ��
    public void OnDrag(PointerEventData eventData)
    {
        m_Rect.position = eventData.position; // �巡�� ���� UI�� ��ġ�� ���콺 ��ġ�� �̵�
    }

    // �巡�� ��
    public void OnEndDrag(PointerEventData eventData)
    {
        // ������ ���� ����� �ߴٴ� ���̱� ������ �巡�� ������ �Ҽӵ� �������� �̵�
        if (transform.parent == m_Canvas)
        {
            transform.SetParent(m_PrevParent);
            m_Rect.position = m_PrevParent.GetComponent<RectTransform>().position;
        }

        // ������� ����
        m_CanvasGroup.alpha = 1f;
        m_CanvasGroup.blocksRaycasts = true;

        // �巡�װ� ���� �� ScrollRect Ȱ��ȭ
        if (m_ScrollRect != null)
            m_ScrollRect.enabled = true;

        // ���� �������� ��ӵǾ����� Ȯ��
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
