using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


// Scene UI의 하단 퀵슬롯에서 소비아이템바

public class UI_UseItemSlot : UI_ItemDragSlot
{
    public int m_Key;

    [SerializeField] Text m_KeyText;

    public override void SetInfo()
    {
        base.SetInfo();

        m_KeyText.text = m_Key.ToString();

        if (Managers.Game.UseItemBarList.ContainsKey(m_Key) == true)
        {
            UseItemData a_UseItem = Managers.Game.UseItemBarList[m_Key];
            AddItem(a_UseItem, a_UseItem.itemCount);
        }
    }

    public override void AddItem(ItemData a_Item, int a_Count = 1)
    {
        base.AddItem(a_Item, a_Count);

        if (Managers.Game.UseItemBarList.ContainsKey(m_Key) == false)
            Managers.Game.UseItemBarList.Add(m_Key, a_Item as UseItemData);
        else
            Managers.Game.UseItemBarList[m_Key] = a_Item as UseItemData;

        // 아이콘 갱신 코드 추가
        if (a_Item != null && a_Item.itemIcon != null)
            icon.sprite = a_Item.itemIcon;
        else
            icon.sprite = null;
    }

    protected override void OnEndDragSlot(PointerEventData eventData)
    {
        if (Item.IsNull() == false && !EventSystem.current.IsPointerOverGameObject())
        {
            if (Managers.Game._playScene._inventory.AcquireItem(Item, ItemCount) == true)
                ClearSlot();
        }

        base.OnEndDragSlot(eventData);
    }

    protected override void OnDropSlot(PointerEventData eventData)
    {
        UI_Slot a_DragSlot = UI_DragSlot.instance.dragSlotItem;

        if (a_DragSlot.IsNull() == false)
        {
            // 자기 자신이라면
            if (a_DragSlot == this) return;

            // 같은 종류의 슬롯이거나 인벤 슬롯일 때 통과
            if ((a_DragSlot is UI_UseItemSlot) == true || (a_DragSlot is UI_InvenSlot) == true)
                ChangeSlot(a_DragSlot as UI_ItemSlot);
        }
    }

    protected override void ChangeSlot(UI_ItemSlot a_ItemSlot)
    {
        // 소비 아이템 확인
        if ((a_ItemSlot.Item is UseItemData) == false) return;

        // 지금 슬롯에 아이템이 존재할 때
        if (Item.IsNull() == false)
        {
            // 아이디 확인 후 개수 증가 or 체인지
            if (Item.id == a_ItemSlot.Item.id)
                SetCount((a_ItemSlot.Item as UseItemData).itemCount);
            else
            {
                if (Managers.Game._playScene._inventory.AcquireItem(Item, ItemCount) == false)
                    return;

                AddItem(a_ItemSlot.Item, (a_ItemSlot.Item as UseItemData).itemCount);
            }
        }
        else AddItem(a_ItemSlot.Item, (a_ItemSlot.Item as UseItemData).itemCount);

        // 기존에 온 슬롯 삭제시키기 
        if (a_ItemSlot is UI_UseItemSlot) (a_ItemSlot as UI_UseItemSlot).ClearSlot();
        if (a_ItemSlot is UI_InvenSlot) (a_ItemSlot as UI_InvenSlot).ClearSlot();
    }

    public override void ClearSlot()
    {
        base.ClearSlot();

        Managers.Game.UseItemBarList[m_Key] = null;
    }
}
