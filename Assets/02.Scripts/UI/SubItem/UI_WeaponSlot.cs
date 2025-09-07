using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


//무기 장착 슬롯
public class UI_WeaponSlot : UI_ItemDragSlot
{
    [SerializeField] Define.WeaponType weaponType = Define.WeaponType.Unknown;
    WeaponItemData WeaponItem;

    public override void SetInfo()
    {
        Managers.Game._playScene._equipment.WeaponSlot = this;

        // 해당 부위 장비가 장착되어 있다면
        if (Managers.Game.CurrentWeapon.id != 0)
            AddItem(Managers.Game.CurrentWeapon);
        else
            Managers.Game.CurrentWeapon = null;

        base.SetInfo();
    }

    // 무기 장착
    public void ChangeWeapon(UI_ItemSlot a_ItemSlot) { ChangeSlot(a_ItemSlot); }

    public override void AddItem(ItemData a_Item, int a_Count = 1)
    {
        base.AddItem(a_Item, a_Count);

        WeaponItem = a_Item as WeaponItemData;

        // 장착 중인 무기가 있다면 비활성화
        if (Managers.Game.CurrentWeapon.IsNull() == false)
        {
            // 장비 파츠 확인
            GetPart(Managers.Game.CurrentWeapon);

            // charEquipment가 null이 아니면 SetActive 호출
            if (Managers.Game.CurrentWeapon.charEquipment != null)
                Managers.Game.CurrentWeapon.charEquipment.SetActive(false);
        }

        // 장비 파츠 확인
        GetPart(WeaponItem);
        Managers.Game.CurrentWeapon = WeaponItem;

        if (WeaponItem != null && WeaponItem.charEquipment != null)
            WeaponItem.charEquipment.SetActive(true);
    }

    protected override void OnClickSlot(PointerEventData eventData)
    {
        if (Item.IsNull() == true || UI_DragSlot.instance.dragSlotItem.IsNull() == false) return;

        // 장비 벗기
        if (Input.GetMouseButtonUp(1))
        {
            if (Managers.Game._playScene._inventory.AcquireItem(WeaponItem) == true)
                ClearSlot();
        }
    }

    protected override void OnEndDragSlot(PointerEventData eventData)
    {
        // 아이템을 버린 위치가 UI가 아니라면
        if (Item.IsNull() == false && !EventSystem.current.IsPointerOverGameObject())
        {
            // 아이템 인벤으로 이동
            if (Managers.Game._playScene._inventory.AcquireItem(WeaponItem) == true)
                ClearSlot();
        }

        base.OnEndDragSlot(eventData);
    }

    protected override void OnDropSlot(PointerEventData eventData)
    {
        UI_Slot a_DragSlot = UI_DragSlot.instance.dragSlotItem;

        if (a_DragSlot.IsNull() == false)
        {
            // 자기 자신이라면
            if (a_DragSlot == this) return;

            // 장비 장착 (or 교체)
            ChangeSlot(a_DragSlot as UI_ItemSlot);
        }
    }

    protected override void ChangeSlot(UI_ItemSlot a_ItemSlot)
    {
        // 장비 확인
        if ((a_ItemSlot.Item is WeaponItemData) == false)  return;

        // 같은 부위 확인
        WeaponItemData a_Weapon = a_ItemSlot.Item as WeaponItemData;
        if (weaponType != a_Weapon.weaponType) return;

        // 레벨 체크
        if (Managers.Game.Level < a_Weapon.minLevel)
        {
            Managers.UI.MakeSubItem<UI_Guide>().SetInfo("레벨이 부족합니다.", new Color(1f, 0.5f, 0f));
            return;
        }

        ItemData a_TempItem = Item;

        // 장비 장착
        AddItem(a_ItemSlot.Item);

        // 기존 장비 인벤 이동
        UI_InvenSlot a_Inven = a_ItemSlot as UI_InvenSlot;
        if (a_TempItem.IsNull() == false)
            a_Inven.AddItem(a_TempItem);
        else
            a_Inven.ClearSlot();
    }

    void GetPart(WeaponItemData a_Weapon)
    {
        if (a_Weapon.charEquipment.IsNull() == true)
            a_Weapon.charEquipment = (Managers.Data.Item[a_Weapon.id] as WeaponItemData).charEquipment;
    }

    public override void ClearSlot()
    {
        base.ClearSlot();

        if (Managers.Game.CurrentWeapon != null && Managers.Game.CurrentWeapon.charEquipment != null)
            Managers.Game.CurrentWeapon.charEquipment.SetActive(false);

        Managers.Game.CurrentWeapon = null;
        WeaponItem = null;
    }
}
