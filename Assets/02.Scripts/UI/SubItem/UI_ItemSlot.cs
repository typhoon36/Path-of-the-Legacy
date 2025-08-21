using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public class UI_ItemSlot : UI_Slot
{
    enum Texts { ItemCountText, }

    public ItemData     Item;
    public int          ItemCount;

    public override void SetInfo()
    {
        base.SetInfo();

        BindText(typeof(Texts));
    }

    // 아이템 등록
    public virtual void AddItem(ItemData a_Item, int a_Count = 1)
    {
        Item = a_Item;

        // 장비가 아니라면 개수 설정
        if ((a_Item is UseItemData) == true)
        {
            (a_Item as UseItemData).ItemCount = a_Count;
            ItemCount = a_Count;
            GetText((int)Texts.ItemCountText).text = ItemCount.ToString();
        }
        else
        {
            ItemCount = 1;

            if (GetText((int)Texts.ItemCountText).IsNull() == false)
                GetText((int)Texts.ItemCountText).text = "";
        }

        if (a_Item.ItemIcon.IsFakeNull() == true)
            a_Item.ItemIcon = Managers.Data.Item[a_Item.Id].ItemIcon;

        // Spirte 넣기(없다면 ItemData에서 불러오기)
        try
        {
            Icon.sprite = a_Item.ItemIcon;
        }
        catch 
        {
            Icon.sprite = a_Item.ItemIcon = Managers.Data.Item[a_Item.Id].ItemIcon;
        }

        // 색 활성화
        SetColor(255);
    }

    // 아이템 개수 업데이트
    public virtual void SetCount(int a_Count = 1)
    {
        ItemCount += a_Count;
        GetText((int)Texts.ItemCountText).text = ItemCount.ToString();
        
        if (Item is UseItemData)
            (Item as UseItemData).ItemCount += a_Count;

        // 개수가 없다면
        if (ItemCount <= 0)
            ClearSlot();
    }

    // 마우스가 슬롯에 닿았다면 정보 활성화
    protected override void OnEnterSlot(PointerEventData eventData)
    {
        if (Item.IsNull() == false)
        {
            Managers.Game.m_PlayScene.SlotTip.OnSlotTip(true);
            Managers.Game.m_PlayScene.SlotTip.background.position = Icon.transform.position;
            Managers.Game.m_PlayScene.SlotTip.RefreshUI(Item);
        }
    }

    // 마우스가 슬롯에서 빠져나오면 정보 비활성화
    protected override void OnExitSlot(PointerEventData eventData)
    {
        if (Item.IsNull() == false)
            Managers.Game.m_PlayScene.SlotTip.OnSlotTip(false);
    }

    // 투명도 설정 (0 ~ 255)
    protected override void SetColor(float a_Alpha)
    {
        base.SetColor(a_Alpha);

        if (GetText((int)Texts.ItemCountText).IsNull() == false)
            GetText((int)Texts.ItemCountText).color = Icon.color;
    }

    public override void ClearSlot()
    {
        base.ClearSlot();
        Item = null;
        ItemCount = 0;

        if (GetText((int)Texts.ItemCountText).IsNull() == false)
            GetText((int)Texts.ItemCountText).text = "";

        Managers.Game.m_PlayScene.SlotTip.OnSlotTip(false);
    }
}
