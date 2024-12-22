using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Drop_UI : MonoBehaviour, IPointerEnterHandler, IDropHandler, IPointerExitHandler
{
    Image m_Image;
    RectTransform m_Rect;

    private void Awake()
    {
        m_Image = GetComponent<Image>();
        m_Rect = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_Image.color = Color.yellow;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        m_Image.color = Color.white;
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.transform.SetParent(transform);
            eventData.pointerDrag.GetComponent<RectTransform>().position = m_Rect.position;
        }
    }
}
