using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;



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

    Sprite buySprite;          // 구매 아이템 sprite
    string itemNameText;       // 아이템 이름 text
    string itemPriceText;      // 아이템 가격 text

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImage(typeof(Images));
        BindText(typeof(Texts));

        Icon = GetImage((int)Images.BuyItemImage);
        Icon.sprite = buySprite;

        GetText((int)Texts.BuyItemName).text = itemNameText;
        GetText((int)Texts.BuyItemPrice).text = itemPriceText;

        // 버튼 기능 등록 (onClick.AddListener이랑 같음.)
        gameObject.BindEvent(OnClickBuyButton, Define.UIEvent.Click);

        SetEventHandler();

        return true;
    }

    public void SetInfo(ItemData itemData)
    {
        Item = itemData;

        buySprite = Item.ItemIcon;
        itemNameText = Item.ItemName;
        itemPriceText = Item.ItemPrice.ToString();
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
        if (Managers.Game.Gold < Item.ItemPrice)
        {
            Managers.UI.MakeSubItem<UI_Guide>().SetInfo("금액이 부족합니다.", Color.yellow);
            return;
        }


        //실제 구매 진행(UseItem)
        if (Item.ItemType == Define.ItemType.Use)
        {
            //UseItem인경우 갯수확인팝업 생성
            UI_NumberCheckPopup a_NumberCheck = Managers.UI.ShowPopupUI<UI_NumberCheckPopup>();
            if (a_NumberCheck.IsNull() == true) return;

            // 아이템 이름과 가격 설정
            a_NumberCheck.SetInfo(Item, (int a_ItemCount) =>
            {
                Managers.Game.Gold -= Item.ItemPrice * a_ItemCount;
                Managers.Game.m_PlayScene.Inventory.AcquireItem(Item.ItemClone(), a_ItemCount);
            });
        }
        // 실제 구매진행(장비)
        else
        {
            // 장비 아이템인 경우 바로 구매
            UI_ConfirmPopup a_Confirm = Managers.UI.ShowPopupUI<UI_ConfirmPopup>();

            // 확인 팝업이 생성되지 않았다면
            if (a_Confirm.IsNull() == true) return;

            // 아이템 이름과 가격 설정
            a_Confirm.SetInfo(() =>
            {
                Managers.Game.Gold -= Item.ItemPrice;
                Managers.Game.m_PlayScene.Inventory.AcquireItem(Item.ItemClone());
            }, Define.ShopSaleMessage);
        }
    }

    public override void SetInfo() { }
}
