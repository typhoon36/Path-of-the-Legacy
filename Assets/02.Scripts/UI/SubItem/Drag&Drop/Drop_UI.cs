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

    // �巡�� �߿� ���콺�� ��������
    public void OnPointerEnter(PointerEventData eventData)
    {
        m_Image.color = Color.yellow;
    }
    // �巡�� �߿� ���콺�� ������ ��������
    public void OnPointerExit(PointerEventData eventData)
    {
        m_Image.color = Color.white;
    }

    // �巡�� �߿� ���콺 ��ӵǾ�����
    public void OnDrop(PointerEventData eventData)
    {
        // �巡�� ���� �������� �ִٸ�
        if (eventData.pointerDrag != null)
        {
            if (transform.CompareTag("EquipSlot"))
            {
                // �巡�� ���� �������� �θ� ���� �������� ����
                eventData.pointerDrag.transform.SetParent(transform);
                // �巡�� ���� �������� ��ġ�� ���� ������ ��ġ�� ����
                eventData.pointerDrag.GetComponent<RectTransform>().position = m_Rect.position;

                // ��� ����
                EqStatPopup_UI.Inst.SetEquip(eventData.pointerDrag);

            }
            // �κ� ������ ���
            else
            {
                eventData.pointerDrag.transform.SetParent(transform);
                eventData.pointerDrag.GetComponent<RectTransform>().position = m_Rect.position;

                // ��� ����
                EqStatPopup_UI.Inst.RemoveEquip(eventData.pointerDrag);
            }
        }
    }
}
