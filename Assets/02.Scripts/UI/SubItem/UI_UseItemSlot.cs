using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class UI_UseItemSlot : UI_ItemDragSlot
{
    public int                  key;

    [SerializeField] Text    keyText;

    public override void SetInfo()
    {
        base.SetInfo();

        keyText.text = key.ToString();

        if (Managers.Game.UseItemBarList.ContainsKey(key) == true)
        {
            UseItemData useItem = Managers.Game.UseItemBarList[key];
            AddItem(useItem, useItem.ItemCount);
        }
    }

    public override void AddItem(ItemData a_Item, int count = 1)
    {
        base.AddItem(a_Item, count);

        if (Managers.Game.UseItemBarList.ContainsKey(key) == false)
            Managers.Game.UseItemBarList.Add(key, a_Item as UseItemData);
        else
            Managers.Game.UseItemBarList[key] = a_Item as UseItemData;
    }

    protected override void OnEndDragSlot(PointerEventData eventData)
    {
        if (Item.IsNull() == false && !EventSystem.current.IsPointerOverGameObject())
        {
            if (Managers.Game.m_PlayScene.Inventory.AcquireItem(Item, ItemCount) == true)
                ClearSlot();
        }
        
        base.OnEndDragSlot(eventData);
    }

    protected override void OnDropSlot(PointerEventData eventData)
    {
        UI_Slot dragSlot = UI_DragSlot.Inst.m_DragSlot;

        if (dragSlot.IsNull() == false)
        {
            // 자기 자신이라면
            if (dragSlot == this)
                return;

            // 같은 종류의 슬롯이거나 인벤 슬롯일 때 통과
            if ((dragSlot is UI_UseItemSlot) == true || (dragSlot is UI_InvenSlot) == true)
                ChangeSlot(dragSlot as UI_ItemSlot);
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
            if (Item.Id == a_ItemSlot.Item.Id)
                SetCount((a_ItemSlot.Item as UseItemData).ItemCount);
            else
            {
                if (Managers.Game.m_PlayScene.Inventory.AcquireItem(Item, ItemCount) == false)
                    return;
                
                AddItem(a_ItemSlot.Item, (a_ItemSlot.Item as UseItemData).ItemCount);
            }
        }
        else AddItem(a_ItemSlot.Item, (a_ItemSlot.Item as UseItemData).ItemCount);

        // 기존에 온 슬롯 삭제
        if (a_ItemSlot is UI_UseItemSlot) (a_ItemSlot as UI_UseItemSlot).ClearSlot();
        if (a_ItemSlot is UI_InvenSlot) (a_ItemSlot as UI_InvenSlot).ClearSlot();
    }

    public override void ClearSlot()
    {
        base.ClearSlot();

        Managers.Game.UseItemBarList[key] = null;
    }
}
