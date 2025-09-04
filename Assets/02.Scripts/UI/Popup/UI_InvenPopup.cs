using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



// 인벤토리 Popup
public class UI_InvenPopup : UI_Popup
{
    enum Gameobjects
    {
        Background,
        Content,
        Title,
        ExitButton,
    }

    enum Texts
    {
        GoldText,
    }

    List<UI_InvenSlot> InvenSlots;         // 슬롯 List

    [SerializeField] int InvenCount = 42;    // 인벤 슬롯 개수

    public override bool Init()
    {
        if (base.Init() == false) return false;

        InvenSlots = new List<UI_InvenSlot>();
        popupType = Define.Popup.Inventory;

        // 자식 객체 불러오기
        BindObject(typeof(Gameobjects));
        BindText(typeof(Texts));

        // InputManager에 입력 등록
        Managers.Input.KeyAction -= OnInventoryUI;
        Managers.Input.KeyAction += OnInventoryUI;

        SetInfo();

        // UI 새로고침(시간차로 인해 새로고침이 안되는 경우가 있어 딜레이 후 새로고침)
        Invoke("DelayInit", 0.0001f);

        return true;
    }
    void DelayInit() { RefreshUI(); Managers.UI.ClosePopupUI(this); }

    void Update()
    {
        // 인벤토리 활성화되면 실시간 새로고침
        if (Managers.Game.isPopups[Define.Popup.Inventory] == true)
            RefreshUI();
    }

    // 인벤토리 자리 확인
    public bool IsInvenMaxSize()
    {
        foreach (UI_InvenSlot slot in InvenSlots)
        {
            if (slot.Item.IsNull() == true) return false;
        }

        return true;
    }

    // 인벤토리 슬롯 아이템 저장
    public bool AcquireItem(ItemData a_Item, int a_Count = 1)
    {
        // 모든 슬롯 확인
        foreach (UI_InvenSlot a_Slot in InvenSlots)
        {
            // 슬롯에 아이템이 없으면
            if (a_Slot.Item.IsNull() == true)
            {
                // 아이템 저장
                a_Slot.AddItem(a_Item, a_Count);
                return true;
            }

            // 소비 아이템이라면
            if (a_Item is UseItemData)
            {
                // 아이템의 id가 같다면 똑같은 아이템이므로
                if (a_Item.id == a_Slot.Item.id)
                {
                    // 개수 추가
                    a_Slot.SetCount(a_Count);
                    return true;
                }
            }
        }

        // 경고문 생성
        Managers.UI.MakeSubItem<UI_Guide>().SetInfo("인벤토리가 가득 찼습니다.", Color.red);

        return false;
    }

    // 인벤토리 Popup 초기화
    public void ResetPos()
    {
        RectTransform a_InvenPos = GetObject((int)Gameobjects.Background).GetComponent<RectTransform>();
        a_InvenPos.anchoredPosition = new Vector2(935, 0);
    }

    // 인벤토리 활성화
    void OnInventoryUI()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Managers.Game.isPopups[Define.Popup.Inventory] = !Managers.Game.isPopups[Define.Popup.Inventory];

            // 인벤토리 Popup On/Off
            if (Managers.Game.isPopups[Define.Popup.Inventory])
                Managers.UI.OnPopupUI(this);
            else
                Exit();
        }
    }

    // 기능 설정
    void SetInfo()
    {
        ResetSlot();        // 슬롯 초기화
        SetEventHandler();  // EventHandler 설정
    }

    void ResetSlot()
    {
        // 슬롯들을 담고 있는 부모 가져오기
        GameObject a_Grid = GetObject((int)Gameobjects.Content);

        // 슬롯 모두 삭제
        foreach (Transform child in a_Grid.transform)
            Managers.Resource.Destroy(child.gameObject);

        // invenCount만큼 슬롯 생성
        for (int i = 0; i < InvenCount; i++)
        {
            // 슬롯 생성
            UI_InvenSlot a_InvenItem = Managers.UI.MakeSubItem<UI_InvenSlot>(parent: a_Grid.transform);

            // 슬롯 위치 번호
            a_InvenItem.InvenNumber = i;

            // 위치 번호가 세이브에 있다면 item 가져오기
            if (Managers.Game.InvenItem.TryGetValue(i, out ItemData a_Item) == true)
            {
                // 기능 설정
                a_InvenItem.SetInfo();

                // 소비 아이템이라면
                if (a_Item is UseItemData)
                {
                    // 아이템 개수와 함께 저장
                    UseItemData useItem = a_Item as UseItemData;
                    a_InvenItem.AddItem(useItem, useItem.itemCount);
                }
                else
                    a_InvenItem.AddItem(a_Item);    //  갯수없이 Item 저장
            }

            // 생성된 슬롯 List에 저장
            InvenSlots.Add(a_InvenItem);
        }
    }

    void SetEventHandler()
    {
        // Title 잡고 인벤토리 이동
        RectTransform a_InvenPos = GetObject((int)Gameobjects.Background).GetComponent<RectTransform>();
        GetObject((int)Gameobjects.Title).BindEvent((PointerEventData eventData) =>
        {

            if (Managers.Game.IsInteract == true) return;

            //드래그하면 제한된 범위를 정해둬야함
            a_InvenPos.anchoredPosition = new Vector2
            (
                Mathf.Clamp(a_InvenPos.anchoredPosition.x + eventData.delta.x, -655, 935),
                Mathf.Clamp(a_InvenPos.anchoredPosition.y + eventData.delta.y, -253, 217)
            );
        }, Define.UIEvent.Drag);

        // Order 설정
        GetObject((int)Gameobjects.Background).BindEvent((PointerEventData eventData) =>
        {
            //UIManager에서 SetOrder를 호출하여 최상위로 올려주기
            Managers.UI.SetOrder(GetComponent<Canvas>());
        }, Define.UIEvent.Click);

        // Exit 버튼
        GetObject((int)Gameobjects.ExitButton).BindEvent((PointerEventData eventData) =>
        {
            if (Managers.Game.IsInteract == true) return;

            //나가게 되면 SlotTip,팝업창 닫기
            Exit();
        }, Define.UIEvent.Click);
    }

    void RefreshUI()
    {
        // 골드 개수 불러오기
        GetText((int)Texts.GoldText).text = Managers.Game.Gold.ToString();
    }

    void Exit()
    {
        Managers.Game._playScene._slotTip.OnSlotTip(false);
        Managers.UI.ClosePopupUI(this);
    }
}
