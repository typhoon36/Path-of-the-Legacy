using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


//구매 슬롯
public class UI_ShopBuySlot : UI_ItemSlot
{
    enum Images
    {
        BuyItemImage,
    }

    enum Texts
    {
        BuyItemName,
        BuyItemPrice,
    }

    Sprite BuySprite;          // 구매 아이템 sprite
    string ItemNameText;       // 아이템 이름 text
    string ItemPriceText;      // 아이템 가격 text

    public override bool Init()
    {
        if (base.Init() == false) return false;

        BindImage(typeof(Images));
        BindText(typeof(Texts));

        icon = GetImage((int)Images.BuyItemImage);
        icon.sprite = BuySprite;

        GetText((int)Texts.BuyItemName).text = ItemNameText;
        GetText((int)Texts.BuyItemPrice).text = ItemPriceText;

        gameObject.BindEvent(OnClickBuyButton, Define.UIEvent.Click);

        SetEventHandler();

        return true;
    }

    public void SetInfo(ItemData a_ItemData)
    {
        Item = a_ItemData;

        BuySprite = Item.itemIcon;
        ItemNameText = Item.itemName;
        ItemPriceText = Item.itemPrice.ToString();

        // UI에 즉시 반영
        if (icon == null)
            icon = GetImage((int)Images.BuyItemImage);
        if (icon != null)
            icon.sprite = BuySprite;

        var nameText = GetText((int)Texts.BuyItemName);
        if (nameText != null)
            nameText.text = ItemNameText;

        var priceText = GetText((int)Texts.BuyItemPrice);
        if (priceText != null)
            priceText.text = ItemPriceText;
    }

    void OnClickBuyButton(PointerEventData eventData)
    {
        // 인벤 크기 확인
        if (Managers.Game._playScene._inventory.IsInvenMaxSize() == true)
        {
            Managers.UI.MakeSubItem<UI_Guide>().SetInfo("인벤토리가 가득 찼습니다.", Color.red);
            return;
        }

        Managers.Game._playScene._slotTip.OnSlotTip(false);

        // 금액 확인
        if (Managers.Game.Gold < Item.itemPrice)
        {
            Managers.UI.MakeSubItem<UI_Guide>().SetInfo("금액이 부족합니다.", Color.yellow);
            return;
        }

        // < 구매 시작 >
        // 소비 아이템이면 개수 선택
        if (Item.itemType == Define.ItemType.Use)
        {
            UI_NumberCheckPopup numberCheckPopup = Managers.UI.ShowPopupUI<UI_NumberCheckPopup>();
            if (numberCheckPopup.IsNull() == true)
                return;

            numberCheckPopup.SetInfo(Item, (int itemCount) =>
            {
                Managers.Game.Gold -= Item.itemPrice * itemCount;
                Managers.Game._playScene._inventory.AcquireItem(Item.ItemClone(), itemCount);
            });
        }
        else
        {
            UI_ConfirmPopup confirmPopup = Managers.UI.ShowPopupUI<UI_ConfirmPopup>();

            if (confirmPopup.IsNull() == true) return;

            confirmPopup.SetInfo(() =>
            {
                Managers.Game.Gold -= Item.itemPrice;
                Managers.Game._playScene._inventory.AcquireItem(Item.ItemClone());
            }, Define.ShopSaleMessage);
        }
    }

    public override void SetInfo() { }
}
