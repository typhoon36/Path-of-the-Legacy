using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//UI_ShopPopup.cs에서 생성되며 아이템 판매가 등록됐을 때 기능

public class UI_ShopSaleSlot : UI_Base
{
    enum Buttons
    {
        CloseButton,
    }

    enum Images
    {
        SaleItemIcon,
    }

    enum Texts
    {
        SaleItemCountText,
    }

    UI_InvenSlot m_InvenItem;             // 인벤토리 슬롯
    Image m_Icon;

    int m_SaleItemCount = 0;     // 판매될 개수
    string m_ItemCountText;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        BindImage(typeof(Images));
        BindText(typeof(Texts));

        GetButton((int)Buttons.CloseButton).onClick.AddListener(OnClickCloseButton);

        GetImage((int)Images.SaleItemIcon).sprite = m_Icon.sprite;
        GetText((int)Texts.SaleItemCountText).text = m_ItemCountText;

        return true;
    }

    public void SetInfo(UI_InvenSlot invenItem, int subItemCount = 1)
    {
        m_InvenItem = invenItem;
        m_SaleItemCount = subItemCount;
        m_Icon = m_InvenItem.icon;

        // 판매할 인벤토리의 슬롯 잠그기
        m_InvenItem.IsLock = true;

        // 소비 아이템이면 개수 활성화
        if (invenItem.Item is UseItemData)
            m_ItemCountText = m_SaleItemCount.ToString();
        else
            m_ItemCountText = "";
    }

    // 판매 진행
    public void GetSale()
    {
        // 장비면 강화 확인 후 판매
        if ((m_InvenItem.Item is EquipmentData) == true)
        {
            EquipmentData equipment = m_InvenItem.Item as EquipmentData;
            Managers.Game.Gold += m_InvenItem.Item.itemPrice + (int)((equipment.itemPrice / 4) * (equipment.upgradeCount));
        }
        else
            Managers.Game.Gold += m_InvenItem.Item.itemPrice * m_SaleItemCount;

        // 판매된 슬롯에 개수 차감
        m_InvenItem.SetCount(-m_SaleItemCount);

        Clear();
    }

    // 판매 등록 취소
    void OnClickCloseButton()
    {
        Managers.Game._playScene._shop.SaleList.Remove(this);

        Clear();
    }

    public void Clear()
    {
        m_InvenItem.IsLock = false;

        Managers.Resource.Destroy(this.gameObject);
    }
}
