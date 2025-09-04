using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// 마우스로 슬롯이 옮겨지는 과정을 보여주기 위한 슬롯

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
    public void DragSetImage(Image a_Icon)
    {
        Managers.UI.SetOrder(GetComponent<Canvas>());
        icon.sprite = a_Icon.sprite;
        SetColor(1);
    }

    public void SetColor(float a_Alpha)
    {
        Color color = icon.color;
        color.a = a_Alpha;
        icon.color = color;
    }
}
