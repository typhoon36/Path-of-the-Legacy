using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public class UI_UpgradePopup : UI_Popup
{
    enum Gameobjects
    {
        ItemSlot,
    }

    enum Buttons
    {
        UpgradeButton,
        ExitButton,
    }

    enum Texts
    {
        ItemNameText,
        UpgradeResultText,
        UpgradeGoldText,
    }

    public EquipmentData _equipment;

    int maxUpgradeCount = 10;   // 최대 강화 수치

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        // 자식 객체 불러오기
        BindObject(typeof(Gameobjects));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        SetInfo();

        Managers.UI.ClosePopupUI(this);

        return true;
    }

    public void RefreshUI(EquipmentData equipment)
    {
        _equipment = equipment;


        if (equipment.UpgradeCount >= maxUpgradeCount)
        {
            GetText((int)Texts.ItemNameText).text = _equipment.ItemName;
            GetText((int)Texts.UpgradeResultText).text = $"Max";
            GetText((int)Texts.UpgradeGoldText).text = "";
        }
        else
        {
            GetText((int)Texts.ItemNameText).text = _equipment.ItemName;
            GetText((int)Texts.UpgradeResultText).text = $"{_equipment.UpgradeCount}   →   {_equipment.UpgradeCount + 1}";
            GetText((int)Texts.UpgradeGoldText).text = EquipmentUpgradeGold(_equipment).ToString();
        }
    }

    // 강화 진행 버튼
    void OnClickUpgradeButton()
    {
        if (_equipment.IsNull() == true)
            return;

        // 강화 수치 Max 확인
        if (_equipment.UpgradeCount >= maxUpgradeCount)
            return;

        // 금액 확인
        int upgradeGold = EquipmentUpgradeGold(_equipment);
        if (Managers.Game.Gold < upgradeGold)
        {
            GetText((int)Texts.ItemNameText).text = "금액이 부족합니다!";
            return;
        }

        Managers.Game.Gold -= upgradeGold;

        // 강화 적용
        EquipmentUpgrade(_equipment);
        RefreshUI(_equipment);
    }

    // 강화 비용 계산
    int EquipmentUpgradeGold(EquipmentData equipment)
    {
        // 강화 금액 : 아이템 판매 가격 + ((판매 가격 / 2) * 강화 횟수)
        int gold = equipment.ItemPrice + (int)((equipment.ItemPrice / 4) * (equipment.UpgradeCount));
        return gold;
    }

    // 강화 적용
    void EquipmentUpgrade(EquipmentData equipment)
    {
        equipment.UpgradeCount += 1;

        // 장비 타입 확인 후 강화 적용
        if (equipment is WeaponItemData)
        {
            WeaponItemData weapon = equipment as WeaponItemData;

            weapon.AddAttack = weapon.UpgradeValue * weapon.UpgradeCount;
        }
        else if (equipment is ArmorItemData)
        {
            ArmorItemData armor = equipment as ArmorItemData;

            armor.AddDefnece = armor.UpgradeValue * armor.UpgradeCount;
            armor.AddHp = (armor.UpgradeValue * 5) * armor.UpgradeCount;
            armor.AddMp = (armor.UpgradeValue * 5) * armor.UpgradeCount;
        }
    }

    public void ExitUpgrade()
    {
        if (_equipment.IsNull() == false)
            Managers.Game.m_PlayScene.Inventory.AcquireItem(_equipment);

        Clear();

        // 강화 슬롯 초기화
        GetObject((int)Gameobjects.ItemSlot).GetComponent<UI_UpgradeSlot>().ClearSlot();

        Managers.Game.IsInteract = false;

        Managers.UI.CloseAllPopupUI();
    }

    public void Clear()
    {
        _equipment = null;

        Managers.Game.m_PlayScene.SlotTip.OnSlotTip(false);

        GetText((int)Texts.ItemNameText).text = "강화할 장비를 선택하세요";
        GetText((int)Texts.UpgradeResultText).text = "";
        GetText((int)Texts.UpgradeGoldText).text = "0";
    }

    void SetInfo()
    {
        GetText((int)Texts.ItemNameText).text = "강화할 장비를 선택하세요";
        GetText((int)Texts.UpgradeResultText).text = "";
        GetText((int)Texts.UpgradeGoldText).text = "0";

        GetButton((int)Buttons.UpgradeButton).onClick.AddListener(OnClickUpgradeButton);
        GetButton((int)Buttons.ExitButton).onClick.AddListener(ExitUpgrade);
    }
}
