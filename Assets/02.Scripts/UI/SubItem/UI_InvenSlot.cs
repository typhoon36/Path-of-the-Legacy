using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



// UI_InvenPopup.cs에서 생성되며  인벤토리 안에서 아이템을 관리하는 Slot

public class UI_InvenSlot : UI_ItemDragSlot
{
    enum GameObjects { Lock, }

    public int InvenNumber;     // 인벤 자리 번호

    // 상점 판매 등록될 시 인벤 Lock
    bool isLock = false;
    public bool IsLock
    {
        get { return isLock; }
        set
        {
            isLock = value;
            GetObject((int)GameObjects.Lock).SetActive(isLock);
        }
    }

    public override void SetInfo()
    {
        base.SetInfo();

        BindObject(typeof(GameObjects));

        GetObject((int)GameObjects.Lock).SetActive(false);

        //제대로 갱신
        if (Item.IsNull() == false && Item.itemIcon.IsNull() == false)
            icon.sprite = Item.itemIcon;
        else
            icon.sprite = null;
    }

    // 아이템 등록
    public override void AddItem(ItemData a_Item, int a_Count = 1)
    {
        base.AddItem(a_Item, a_Count);

     
        // 매니저에 저장
        if (Managers.Game.InvenItem.ContainsKey(InvenNumber) == false)
            Managers.Game.InvenItem.Add(InvenNumber, a_Item);
        else
            Managers.Game.InvenItem[InvenNumber] = a_Item;


        //아이콘 제대로 갱신
        if (a_Item.IsNull() == false && a_Item.itemIcon.IsNull() == false)
            icon.sprite = a_Item.itemIcon;
        else
            icon.sprite = null;
    }

    // 슬롯 우클릭
    protected override void OnClickSlot(PointerEventData eventData)
    {
        if (isLock == true) return;

        if (Item.IsNull() == true || UI_DragSlot.instance.dragSlotItem.IsNull() == false) return;

        // 슬롯 우클릭
        if (Input.GetMouseButtonUp(1))
        {
            // 상호작용 중이라면
            if (Managers.Game.IsInteract == true)
            {
                Managers.Game.GetSlotInteract(this);
                return;
            }

            // 장비 or 소비 아이템이라면
            if ((Item is EquipmentData) == true)
            {
                // 장착 레벨 확인
                if (Managers.Game.Level >= (Item as EquipmentData).minLevel)
                    Managers.Game._playScene._equipment.SetEquipment(this);
                else
                    Managers.UI.MakeSubItem<UI_Guide>().SetInfo("레벨이 부족합니다.", new Color(1f, 0.5f, 0f));
            }
            else if ((Item is UseItemData) == true)
            {
                // 아이템 사용이 성공적으로 됐다면 -1 차감
                if ((Item as UseItemData).UseItem(this.Item) == true)
                    SetCount(-1);
            }
        }
    }

    protected override void OnBeginDragSlot(PointerEventData eventData)
    {
        // Lock 확인
        if (IsLock == true) return;

        base.OnBeginDragSlot(eventData);
    }

    // 슬롯 받기
    protected override void OnDropSlot(PointerEventData eventData)
    {
        if (UI_DragSlot.instance.dragSlotItem.IsNull() == true) return;

        UI_Slot a_DragSlot = UI_DragSlot.instance.dragSlotItem;

        if (a_DragSlot == this) return;

        // 어떤 슬롯에서 왔는지 체크
        switch (a_DragSlot)
        {
            case UI_UpgradeSlot upgradeSlot:            // 업그레이드 Slot
                {
                    AddSlot<UI_UpgradeSlot>(upgradeSlot);
                }
                break;
            case UI_ArmorSlot armorSlot:                // 방어구 Slot
                {
                    // 현재 아이템이 같은 종류의 방어구라면 교체
                    if (ItemTypeCheck<ArmorItemData>() == true)
                    {
                        if ((armorSlot.ArmorType == (Item as ArmorItemData).armorType))
                        {
                            armorSlot.ChangeArmor(this);
                            return;
                        }
                    }

                    AddSlot<UI_ArmorSlot>(armorSlot);
                }
                break;
            case UI_WeaponSlot weaponSlot:              // 무기 Slot
                {
                    // 현재 아이템이 무기라면 교체
                    if (ItemTypeCheck<WeaponItemData>() == true)
                    {
                        weaponSlot.ChangeWeapon(this);
                        return;
                    }

                    AddSlot<UI_WeaponSlot>(weaponSlot);
                }
                break;
            case UI_UseItemSlot useSlot:                // 소비 Slot
                {
                    AddSlot<UI_UseItemSlot>(useSlot, useSlot.ItemCount);
                }
                break;
            case UI_InvenSlot invenSlot:                // 인벤 Slot
                {
                    // 두 슬롯의 아이템이 같은 아이템일 경우 개수 체크
                    if (Item == invenSlot.Item && (invenSlot.Item is UseItemData))
                    {
                        int addValue = ItemCount + invenSlot.ItemCount;
                        if (addValue > Item.itemMaxCount)
                        {
                            invenSlot.SetCount(-(Item.itemMaxCount - ItemCount));
                            SetCount(Item.itemMaxCount - ItemCount);
                        }
                        else
                        {
                            SetCount(invenSlot.ItemCount);
                            invenSlot.ClearSlot();  // 들고 있었던 슬롯은 초기화
                        }
                    }
                    else
                        ChangeSlot(invenSlot);
                }
                break;
        }
    }

    protected override void ChangeSlot(UI_ItemSlot a_ItemSlot)
    {
        // 임시 변수
        ItemData a_TempItem = Item;
        int a_TempItemCount = ItemCount;

        // 인벤 가져오기
        UI_InvenSlot a_InvenSlot = a_ItemSlot as UI_InvenSlot;

        // 새로 받은 슬롯 Add
        AddItem(a_InvenSlot.Item, a_InvenSlot.ItemCount);

        // 자신의 아이템을 상대 슬롯에 전달
        if (a_TempItem.IsNull() == false)
            a_InvenSlot.AddItem(a_TempItem, a_TempItemCount);
        else
            a_InvenSlot.ClearSlot();
    }

    // 현재 슬롯의 아이템 타입 체크
    bool ItemTypeCheck<T>() where T : EquipmentData
    {
        if (Item.IsNull() == false)
        {
            if ((Item is T) == true)
                return true;
        }

        return false;
    }

    // 슬롯 받기
    void AddSlot<T>(T a_Slot, int a_Count = 1) where T : UI_ItemDragSlot
    {
        // 아이템이 있다면 다른 슬롯 || 없다면 지금 슬롯에 넣기
        if (Item.IsNull() == false)
            Managers.Game._playScene._inventory.AcquireItem(a_Slot.Item, a_Count);
        else
            AddItem(a_Slot.Item, a_Count);

        a_Slot.ClearSlot();
    }

    // 슬롯 초기화
    public override void ClearSlot()
    {
        base.ClearSlot();

        IsLock = false;

        // 매니저에 저장
        if (Managers.Game.InvenItem.ContainsKey(InvenNumber) == true)
            Managers.Game.InvenItem[InvenNumber] = null;
    }
}
