using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


// 방어구 슬롯
public class UI_ArmorSlot : UI_ItemDragSlot
{
    public Define.ArmorType ArmorType = Define.ArmorType.Unknown;
    public ArmorItemData ArmorItem;

    public override void SetInfo()
    {
        Managers.Game._playScene._equipment.ArmorSlots.Add(this);

        // 해당 부위 장비가 이미 장착되어 있다면 장착 (Save Load 했을때)
        if (Managers.Game.CurrentArmor.TryGetValue(ArmorType, out ArmorItem) == true)
        {
            base.AddItem(ArmorItem);
            AddArmor(ArmorItem);
        }

        base.SetInfo();

    }

    // 방어구 교체
    public void ChangeArmor(UI_ItemSlot a_ItemSlot)
    {
        ChangeSlot(a_ItemSlot);
    }

    public override void AddItem(ItemData a_Item, int a_Count = 1)
    {
        base.AddItem(a_Item, a_Count);

        ArmorItem = a_Item as ArmorItemData;

        // 장착 중인 장비가 있다면 비활성화
        if (Managers.Game.CurrentArmor.ContainsKey(ArmorType) == true)
        {
            // 현재 장착한 장비 가져오기
            ArmorItemData currentArmor = Managers.Game.CurrentArmor[ArmorType];

            // 플레이어가 현재 입고 있는 장비 오브젝트 비활성화
            EquipmentActive(currentArmor, false);

            // 스탯 해제
            Managers.Game.RefreshArmor(currentArmor, false);
        }

        // 방어구 장착
        AddArmor(ArmorItem);
    }

    protected override void OnClickSlot(PointerEventData eventData)
    {
        // 드래그 중이거나 아이템이 없다면 취소
        if (Item.IsNull() == true || UI_DragSlot.instance.dragSlotItem.IsNull() == false) return;

        // 우클릭하여 장비 벗기
        if (Input.GetMouseButtonUp(1))
        {
            // 인벤으로 보내고 초기화
            if (Managers.Game._playScene._inventory.AcquireItem(ArmorItem) == true)
                ClearSlot();
        }
    }

    protected override void OnEndDragSlot(PointerEventData eventData)
    {
        // 아이템을 버린 위치가 UI가 아니라면
        if (Item.IsNull() == false && !EventSystem.current.IsPointerOverGameObject())
        {
            // 인벤으로 보내고 초기화
            if (Managers.Game._playScene._inventory.AcquireItem(ArmorItem) == true)
                ClearSlot();
        }

        base.OnEndDragSlot(eventData);
    }

    protected override void OnDropSlot(PointerEventData eventData)
    {
        UI_Slot a_DragSlot = UI_DragSlot.instance.dragSlotItem;

        if (a_DragSlot.IsNull() == false)
        {
            // 자기 자신이라면 취소
            if (a_DragSlot == this)
            {
                Debug.Log("dragSlot == this");
                return;
            }

            // 장비 장착 (or 교체)
            ChangeSlot(a_DragSlot as UI_ItemSlot);
        }
    }

    protected override void ChangeSlot(UI_ItemSlot a_ItemSlot)
    {
        // 장비 확인
        if ((a_ItemSlot.Item is ArmorItemData) == false) return;

        // 같은 부위 확인
        ArmorItemData armor = a_ItemSlot.Item as ArmorItemData;

        if (ArmorType != armor.armorType) return;

        // 레벨 확인
        if (Managers.Game.Level < armor.minLevel)
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

    // 장비 장착
    void AddArmor(ArmorItemData a_ArmorItem)
    {
        // 장비 장착 진행
        if (Managers.Game.CurrentArmor.ContainsKey(ArmorType) == false)
            Managers.Game.CurrentArmor.Add(ArmorType, a_ArmorItem);
        else
            Managers.Game.CurrentArmor[ArmorType] = a_ArmorItem;

        // 장비 오브젝트 활성화
        EquipmentActive(a_ArmorItem, true);

        // 스탯 적용
        Managers.Game.RefreshArmor(a_ArmorItem, true);
    }

    // 캐릭터 장비 파츠 활성화 여부
    void EquipmentActive(ArmorItemData a_Armor, bool IsActive)
    {
        // 아이템이 현재 입고 있는 장비를 알고 있다면
        if (a_Armor.charEquipment.IsNull() == false)
        {
            foreach (GameObject obj in a_Armor.charEquipment)
                obj.SetActive(IsActive);

            return;
        }

        // 모른다면 id로 찾기
        PlayerController a_Player = Managers.Game.GetPlayer().GetComponent<PlayerController>();

        List<GameObject> objList = new List<GameObject>();
        if (a_Player.charEquipment.TryGetValue(a_Armor.id, out objList) == false)
        {
            Debug.Log($"{a_Armor.id} : 활성화 실패");
            return;
        }

        // 아이템 안에 넣어주기
        a_Armor.charEquipment = objList;

        foreach (GameObject obj in objList)
            obj.SetActive(IsActive);
    }

    public override void ClearSlot()
    {
        base.ClearSlot();

        EquipmentActive(ArmorItem, false);              // 장비 비활성화
        Managers.Game.RefreshArmor(ArmorItem, false);   // 장비 스탯 해제
        ArmorItem = null;
        Managers.Game.CurrentArmor.Remove(ArmorType);
    }
}
