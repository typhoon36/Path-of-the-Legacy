using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


// 장비, 소비 등의 아이템을 구매/판매할 수 있는 상점 UI

public class UI_ShopPopup : UI_Popup
{
    enum Gameobjects
    {
        Title,
        Background,
        ExitButton,
        BuyButton,
        SaleButton,
        BuyList,
        SaleList,
        GoSaleButton,
    }

    enum Texts
    {
        TitleText,
    }

    public Define.ShopType m_ShopType = Define.ShopType.Unknown;

    public List<UI_ShopSaleSlot> SaleList;     // 판매 슬롯
    List<UI_ShopBuySlot> BuyList;       // 구매 슬롯

    int m_CurShopId = 0;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        // 자식 객체 불러오기
        BindObject(typeof(Gameobjects));
        BindText(typeof(Texts));

        SetInfo();

        GetObject((int)Gameobjects.SaleList).SetActive(false);
        GetObject((int)Gameobjects.GoSaleButton).SetActive(false);

        Managers.UI.ClosePopupUI(this);

        return true;
    }

    public void RefreshUI(ShopNpcController a_Npc, int a_ShopBuyId)
    {
        // 상점 이름 설정
        GetText((int)Texts.TitleText).text = $"{a_Npc.shopType.ToString()} Shop";

        // 구매 슬롯 설정
        SettingBuySlot(a_ShopBuyId);
    }

    // 구매 슬롯 설정
    void SettingBuySlot(int a_ShopBuyId)
    {
        // 똑같은 상점에 들린다면
        if (m_CurShopId == a_ShopBuyId) return;

        m_CurShopId = a_ShopBuyId;

        // 구매 슬롯 초기화
        Transform a_BuyListParent = GetObject((int)Gameobjects.BuyList).transform;
        for (int i = a_BuyListParent.childCount - 1; i >= 0; i--)
        {
            Managers.Resource.Destroy(a_BuyListParent.GetChild(i).gameObject);
            BuyList.Clear();
        }

        // 구매 Id List 가져오기
        List<int> a_ItemIdList = new List<int>();
        if (Managers.Data.Shop.TryGetValue(a_ShopBuyId, out a_ItemIdList) == false)
        {
            Debug.Log("Shop ItemIdList Failed!");
            return;
        }

        // 구매 슬롯 채우기
        for (int i = 0; i < a_ItemIdList.Count; i++)
        {
            UI_ShopBuySlot a_BuyShop = Managers.UI.MakeSubItem<UI_ShopBuySlot>(parent: GetObject((int)Gameobjects.BuyList).transform);
            a_BuyShop.SetInfo(Managers.Data.Item[a_ItemIdList[i]]);
            BuyList.Add(a_BuyShop);
        }
    }

    // 구매 리스트 호출 버튼
    void OnClickBuyListButton(PointerEventData eventData)
    {
        GetObject((int)Gameobjects.BuyList).SetActive(true);
        GetObject((int)Gameobjects.SaleList).SetActive(false);
        GetObject((int)Gameobjects.GoSaleButton).SetActive(false);
    }

    // 판매 호출 버튼
    void OnClickSaleListButton(PointerEventData eventData)
    {
        GetObject((int)Gameobjects.BuyList).SetActive(false);
        GetObject((int)Gameobjects.SaleList).SetActive(true);
        GetObject((int)Gameobjects.GoSaleButton).SetActive(true);
    }

    // 판매 진행 버튼
    void OnClickGoSaleButton(PointerEventData eventData)
    {
        // 판매 등록 확인
        if (SaleList.Count == 0) return;

        // 팔기 전 골드 저장
        int a_BeforeGold = Managers.Game.Gold;

        // 아이템 팔기
        for (int i = 0; i < SaleList.Count; i++)
            SaleList[i].GetSale();

        // 판매 후 골드 저장
        int a_AfterGold = Managers.Game.Gold - a_BeforeGold;

        // 획득한 골드 안내문 생성
        Managers.UI.MakeSubItem<UI_Guide>().SetInfo($"Gold {a_AfterGold}+", Color.yellow);

        // 초기화
        SaleList.Clear();
    }

    // 판매 아이템 등록
    private void SetSaleItemRegister(UI_InvenSlot a_InvenSlot)
    {
        // 장비거나 개수가 한개라면 판매 등록
        if (a_InvenSlot.Item is EquipmentData || a_InvenSlot.ItemCount == 1)
        {
            SaleItemRegister(a_InvenSlot);
            return;
        }

        // 판매 개수 선택
        UI_NumberCheckPopup a_NumberCheckPopup = Managers.UI.ShowPopupUI<UI_NumberCheckPopup>();
        if (a_NumberCheckPopup.IsNull() == true) return;

        // 개수 선택 설정
        a_NumberCheckPopup.SetInfo(a_InvenSlot, (int subItemCount) =>
        {
            // 개수 선택한 만큼 판매 등록
            SaleItemRegister(a_InvenSlot, subItemCount);
        });
    }

    // 판매 등록
    void SaleItemRegister(UI_InvenSlot a_InvenItem, int a_Count = 1)
    {
        // 판매 슬로 생성 후 아이템 등록
        UI_ShopSaleSlot a_SaleItem = Managers.UI.MakeSubItem<UI_ShopSaleSlot>(GetObject((int)Gameobjects.SaleList).transform);
        a_SaleItem.SetInfo(a_InvenItem, a_Count);
        SaleList.Add(a_SaleItem);
    }

    void SetInfo()
    {
        BuyList = new List<UI_ShopBuySlot>();
        SaleList = new List<UI_ShopSaleSlot>();

        // 판매 슬롯 초기화
        foreach (Transform a_Child in GetObject((int)Gameobjects.SaleList).transform)
            Managers.Resource.Destroy(a_Child.gameObject);

        // 구매 슬롯 초기화
        foreach (Transform a_Child in GetObject((int)Gameobjects.BuyList).transform)
            Managers.Resource.Destroy(a_Child.gameObject);

        SetEventHandler();
    }

    void SetEventHandler()
    {
        // Title 잡고 인벤토리 이동
        RectTransform shopPos = GetObject((int)Gameobjects.Background).GetComponent<RectTransform>();
        GetObject((int)Gameobjects.Title).BindEvent((PointerEventData eventData) =>
        {
            shopPos.anchoredPosition = new Vector2
            (
                Mathf.Clamp(shopPos.anchoredPosition.x + eventData.delta.x, -655, 655),
                Mathf.Clamp(shopPos.anchoredPosition.y + eventData.delta.y, -253, 217)
            );
        }, Define.UIEvent.Drag);

        // Order 설정
        GetObject((int)Gameobjects.Background).BindEvent((PointerEventData eventData) =>
        {
            Managers.UI.SetOrder(GetComponent<Canvas>());
        }, Define.UIEvent.Click);

        // Exit 버튼
        GetObject((int)Gameobjects.ExitButton).BindEvent((PointerEventData eventData) =>
        {
            ExitShop();
        }, Define.UIEvent.Click);

        // 판매할 아이템 받기
        GetObject((int)Gameobjects.SaleList).BindEvent((PointerEventData eventData) =>
        {
            UI_Slot dragSlot = UI_DragSlot.instance.dragSlotItem;

            // 인벤토리 슬롯 확인
            if (dragSlot is UI_InvenSlot == true)
                SetSaleItemRegister(dragSlot as UI_InvenSlot);

        }, Define.UIEvent.Drop);

        // 우클릭으로 판매할 아이템 받기
        Managers.Game._getSlotInteract -= GetSlotInteract;
        Managers.Game._getSlotInteract += GetSlotInteract;

        GetObject((int)Gameobjects.BuyButton).BindEvent(OnClickBuyListButton);
        GetObject((int)Gameobjects.SaleButton).BindEvent(OnClickSaleListButton);
        GetObject((int)Gameobjects.GoSaleButton).BindEvent(OnClickGoSaleButton);
    }

    // 우클릭 아이템 받기
    void GetSlotInteract(UI_InvenSlot a_InvenSlot)
    {
        // 판매 리스트가 현재 활성화 중이라면
        if (GetObject((int)Gameobjects.SaleList).activeSelf == true)
            SetSaleItemRegister(a_InvenSlot);
    }

    // 상점 나가기 (초기화)
    public void ExitShop()
    {
        GetObject((int)Gameobjects.BuyList).SetActive(true);
        GetObject((int)Gameobjects.SaleList).SetActive(false);
        GetObject((int)Gameobjects.GoSaleButton).SetActive(false);

        for (int i = 0; i < SaleList.Count; i++)
            SaleList[i].Clear();

        SaleList.Clear();

        Managers.Game.IsInteract = false;

        Managers.UI.CloseAllPopupUI();
    }
}
