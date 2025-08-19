using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class UI_DragSlot : MonoBehaviour
{
    public static UI_DragSlot   instance;

    public UI_Slot              dragSlotItem;   // 슬롯 담는 변수
    public Image                icon;           // 아이템 이미지

    void Start()
    {
        instance = this;
    }

    // 드래그 할 경우 활성화
    public void DragSetImage(Image _icon)
    {
        Managers.UI.SetOrder(GetComponent<Canvas>());
        icon.sprite = _icon.sprite;
        SetColor(1);
    }

    public void SetColor(float _alpha)
    {
        Color color = icon.color;
        color.a = _alpha;
        icon.color = color;
    }
}
