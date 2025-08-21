using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public class UI_WeaponSlot : UI_ItemDragSlot
{
    [SerializeField] Define.WeaponType   weaponType = Define.WeaponType.Unknown;
     WeaponItemData      weaponItem;

    public override void SetInfo()
    {
        Managers.Game.m_PlayScene.Equipment.weaponSlot = this;

        // 해당 부위 장비가 장착되어 있다면
        if (Managers.Game.CurrentWeapon.Id != 0)
            AddItem(Managers.Game.CurrentWeapon);
        else
            Managers.Game.CurrentWeapon = null;

        base.SetInfo();
    }

    // 무기 장착
    public void ChangeWeapon(UI_ItemSlot itemSlot) { ChangeSlot(itemSlot); }

    public override void AddItem(ItemData _item, int count = 1)
    {
        base.AddItem(_item, count);

        weaponItem = _item as WeaponItemData;
        
        // 장착 중인 무기가 있다면 비활성화
        if (Managers.Game.CurrentWeapon.IsNull() == false)
        {
            // 장비 파츠 확인
            GetPart(Managers.Game.CurrentWeapon);

            Managers.Game.CurrentWeapon.charEquipment.SetActive(false);
        }
        
        // 장비 파츠 확인
        GetPart(weaponItem);
        Managers.Game.CurrentWeapon = weaponItem;


        weaponItem.charEquipment.SetActive(true);
    }



    protected override void OnClickSlot(PointerEventData eventData)
    {
        if (Item.IsNull() == true || UI_DragSlot.Inst.m_DragSlot.IsNull() == false) return;

        // 장비 벗기
        if (Input.GetMouseButtonUp(1))
        {
            if (Managers.Game.m_PlayScene.Inventory.AcquireItem(weaponItem) == true)
                ClearSlot();

            //Debug.Log("장비 벗는사운드 재생");
        }
    }

    protected override void OnEndDragSlot(PointerEventData eventData)
    {
        // 아이템을 버린 위치가 UI가 아니라면
        if (Item.IsNull() == false && !EventSystem.current.IsPointerOverGameObject())
        {
            // 아이템 인벤으로 이동
            if (Managers.Game.m_PlayScene.Inventory.AcquireItem(weaponItem) == true)
                ClearSlot();
        }
        
        base.OnEndDragSlot(eventData);
    }

    protected override void OnDropSlot(PointerEventData eventData)
    {
        UI_Slot dragSlot = UI_DragSlot.Inst.m_DragSlot;

        if (dragSlot.IsNull() == false)
        {
            // 자기 자신이라면
            if (dragSlot == this)
                return;

            // 장비 장착 (or 교체)
            ChangeSlot(dragSlot as UI_ItemSlot);
        }
    }

    protected override void ChangeSlot(UI_ItemSlot itemSlot)
    {
        // 장비 확인
        if ((itemSlot.Item is WeaponItemData) == false)
            return;

        // 같은 부위 확인
        WeaponItemData weapon = itemSlot.Item as WeaponItemData;
        if (weaponType != weapon.WeaponType)
            return;

        // 레벨 체크
        if (Managers.Game.Level < weapon.MinLevel)
        {
            Managers.UI.MakeSubItem<UI_Guide>().SetInfo("레벨이 부족합니다.", new Color(1f, 0.5f, 0f));
            return;
        }

        ItemData _tempItem = Item;

        // 장비 장착
        AddItem(itemSlot.Item);

        // 기존 장비 인벤 이동
        UI_InvenSlot inven = itemSlot as UI_InvenSlot;
        if (_tempItem.IsNull() == false)
            inven.AddItem(_tempItem);
        else
            inven.ClearSlot();
    }

     void GetPart(WeaponItemData weapon)
    {
        if (weapon.charEquipment.IsNull() == true)
            weapon.charEquipment = (Managers.Data.Item[weapon.Id] as WeaponItemData).charEquipment;
    }

    public override void ClearSlot()
    {
        base.ClearSlot();

        Managers.Game.CurrentWeapon.charEquipment.SetActive(false);
        Managers.Game.CurrentWeapon = null;
        weaponItem = null;
    }
}
