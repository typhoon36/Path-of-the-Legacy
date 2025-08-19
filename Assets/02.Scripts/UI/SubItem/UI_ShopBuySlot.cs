using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
 * File :   UI_ShopBuySlot.cs
 * Desc :   UI_ShopPopup.cs에서 생성되며 아이템 구매 버튼을 담당
 *
 & Functions
 &  [Public]
 &  : Init()                - 초기 설정
 &  : SetInfo()             - 기능 설정
 &
 &  [Prviate]
 &  : OnClickBuyButton()    - 구매 버튼 클릭 기능
 *
 */

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

     Sprite      buySprite;          // 구매 아이템 sprite
     string      itemNameText;       // 아이템 이름 text
     string      itemPriceText;      // 아이템 가격 text

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImage(typeof(Images));
        BindText(typeof(Texts));

        icon = GetImage((int)Images.BuyItemImage);
        icon.sprite = buySprite;

        GetText((int)Texts.BuyItemName).text = itemNameText;
        GetText((int)Texts.BuyItemPrice).text = itemPriceText;

        // 버튼 기능 등록 (onClick.AddListener이랑 같음.)
        gameObject.BindEvent(OnClickBuyButton, Define.UIEvent.Click);

        SetEventHandler();

        return true;
    }

    public void SetInfo(ItemData itemData)
    {
        item = itemData;

        buySprite = item.itemIcon;
        itemNameText = item.itemName;
        itemPriceText = item.itemPrice.ToString();
    }

     void OnClickBuyButton(PointerEventData eventData)
    {
        // 인벤 크기 확인
        if (Managers.Game.m_PlayScene.Inventory.IsInvenMaxSize() == true)
        {
            Managers.UI.MakeSubItem<UI_Guide>().SetInfo("인벤토리가 가득 찼습니다.", Color.red);
            return;
        }

        Managers.Game.m_PlayScene.SlotTip.OnSlotTip(false);
        
        // 금액 확인
        if (Managers.Game.Gold < item.itemPrice)
        {
            Managers.UI.MakeSubItem<UI_Guide>().SetInfo("금액이 부족합니다.", Color.yellow);
            return;
        }

        // < 구매 시작 >
        // 소비 아이템이면 개수 선택
        if (item.itemType == Define.ItemType.Use)
        {
            UI_NumberCheckPopup numberCheckPopup = Managers.UI.ShowPopupUI<UI_NumberCheckPopup>();
            if (numberCheckPopup.IsNull() == true)
                return;

            numberCheckPopup.SetInfo(item, (int itemCount)=>
            {
                Managers.Game.Gold -= item.itemPrice * itemCount;
                Managers.Game.m_PlayScene.Inventory.AcquireItem(item.ItemClone(), itemCount);
            });
        }
        else
        {
            UI_ConfirmPopup confirmPopup = Managers.UI.ShowPopupUI<UI_ConfirmPopup>();
            if (confirmPopup.IsNull() == true)
                return;
            
            confirmPopup.SetInfo(()=>
            {
                Managers.Game.Gold -= item.itemPrice;
                Managers.Game.m_PlayScene.Inventory.AcquireItem(item.ItemClone());
            }, Define.ShopSaleMessage);
        }
    }

    public override void SetInfo() {}
}
