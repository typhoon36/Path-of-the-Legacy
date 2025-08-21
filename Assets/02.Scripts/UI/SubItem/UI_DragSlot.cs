using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class UI_DragSlot : MonoBehaviour
{
    public static UI_DragSlot   Inst;

    public UI_Slot              m_DragSlot;   // 슬롯 담는 변수
    public Image                Icon;           // 아이템 이미지

    void Start()
    {
        Inst = this;
    }

    // 드래그 할 경우 활성화
    public void DragSetImage(Image _icon)
    {
        Managers.UI.SetOrder(GetComponent<Canvas>());
        Icon.sprite = _icon.sprite;
        SetColor(1);
    }

    public void SetColor(float _alpha)
    {
        Color color = Icon.color;
        color.a = _alpha;
        Icon.color = color;
    }
}
