using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



//  모든 Item관련 Slot은 해당 클래스를 상속받는다.
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
        if ((Item is UseItemData) == true)
        {
            (Item as UseItemData).itemCount = a_Count;
            ItemCount = a_Count;
            GetText((int)Texts.ItemCountText).text = ItemCount.ToString();
        }
        else
        {
            ItemCount = 1;

            if (GetText((int)Texts.ItemCountText).IsNull() == false)
                GetText((int)Texts.ItemCountText).text = "";
        }

        if (Item.itemIcon.IsFakeNull() == true)
            Item.itemIcon = Managers.Data.Item[Item.id].itemIcon;

        // Spirte 넣기
        // try는 null체크 시 없는 객체면 item Data에서 빼옴.
        try
        {
            icon.sprite = Item.itemIcon;
        }
        catch 
        {
            icon.sprite = Item.itemIcon = Managers.Data.Item[Item.id].itemIcon;
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
            (Item as UseItemData).itemCount += a_Count;

        // 개수가 없다면
        if (ItemCount <= 0)
            ClearSlot();
    }

    // 마우스가 슬롯에 닿았다면 정보 활성화
    protected override void OnEnterSlot(PointerEventData eventData)
    {
        if (Item.IsNull() == false)
        {
            Managers.Game._playScene._slotTip.OnSlotTip(true);
            Managers.Game._playScene._slotTip.m_Background.position = icon.transform.position;
            Managers.Game._playScene._slotTip.RefreshUI(Item);
        }
    }

    // 마우스가 슬롯에서 빠져나오면 정보 비활성화
    protected override void OnExitSlot(PointerEventData eventData)
    {
        if (Item.IsNull() == false)
            Managers.Game._playScene._slotTip.OnSlotTip(false);
    }

    // 투명도 설정 (0 ~ 255)
    protected override void SetColor(float a_Alpha)
    {
        base.SetColor(a_Alpha);

        if (GetText((int)Texts.ItemCountText).IsNull() == false)
            GetText((int)Texts.ItemCountText).color = icon.color;
    }

    public override void ClearSlot()
    {
        base.ClearSlot();
        Item = null;
        ItemCount = 0;

        if (GetText((int)Texts.ItemCountText).IsNull() == false)
            GetText((int)Texts.ItemCountText).text = "";

        Managers.Game._playScene._slotTip.OnSlotTip(false);
    }
}
