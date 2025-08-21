using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class UI_InvenSlot : UI_ItemDragSlot
{
    enum GameObjects { Lock, }

    public int  invenNumber;     // 인벤 자리 번호

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
    }

    // 아이템 등록
    public override void AddItem(ItemData _item, int count = 1)
    {
        base.AddItem(_item, count);

        // 매니저에 저장
        if (Managers.Game.InvenItem.ContainsKey(invenNumber) == false)
            Managers.Game.InvenItem.Add(invenNumber, _item);
        else
            Managers.Game.InvenItem[invenNumber] = _item;
    }

    // 슬롯 우클릭
    protected override void OnClickSlot(PointerEventData eventData)
    {
        if (isLock == true)
            return;

        if (Item.IsNull() == true || UI_DragSlot.Inst.m_DragSlot.IsNull() == false) return;

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
                if (Managers.Game.Level >= (Item as EquipmentData).MinLevel)
                    Managers.Game.m_PlayScene.Equipment.SetEquipment(this);
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
        if (IsLock == true)
            return;

        base.OnBeginDragSlot(eventData);
    }

    // 슬롯 받기
    protected override void OnDropSlot(PointerEventData eventData)
    {
        if (UI_DragSlot.Inst.m_DragSlot.IsNull() == true)
            return;
            
        UI_Slot dragSlot = UI_DragSlot.Inst.m_DragSlot;

        if (dragSlot == this)
            return;

        // 어떤 슬롯에서 왔는지 체크
        switch (dragSlot)
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
                    if ((armorSlot.armorType == (Item as ArmorItemData).ArmorType))
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
                    if (addValue > Item.ItemMaxCount)
                    {
                        invenSlot.SetCount(-(Item.ItemMaxCount-ItemCount));
                        SetCount(Item.ItemMaxCount - ItemCount);
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

    protected override void ChangeSlot(UI_ItemSlot itemSlot)
    {
        // 임시 변수
        ItemData _tempItem = Item;
        int _tempItemCount = ItemCount;

        // 인벤 가져오기
        UI_InvenSlot invenSlot = itemSlot as UI_InvenSlot;

        // 새로 받은 슬롯 Add
        AddItem(invenSlot.Item, invenSlot.ItemCount);

        // 자신의 아이템을 상대 슬롯에 전달
        if (_tempItem.IsNull() == false)
            invenSlot.AddItem(_tempItem, _tempItemCount);
        else
            invenSlot.ClearSlot();
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
     void AddSlot<T>(T slot, int count = 1) where T : UI_ItemDragSlot
    {
        // 아이템이 있다면 다른 슬롯 || 없다면 지금 슬롯에 넣기
        if (Item.IsNull() == false)
            Managers.Game.m_PlayScene.Inventory.AcquireItem(slot.Item, count);
        else
            AddItem(slot.Item, count);

        slot.ClearSlot();
    }

    // 슬롯 초기화
    public override void ClearSlot()
    {
        base.ClearSlot();

        IsLock = false;

        // 매니저에 저장
        if (Managers.Game.InvenItem.ContainsKey(invenNumber) == true)
            Managers.Game.InvenItem[invenNumber] = null;
    }
}
