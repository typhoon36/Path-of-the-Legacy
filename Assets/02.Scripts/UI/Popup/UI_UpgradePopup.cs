using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


// 장비를 강화할 수 있는 Popup UI(UpgradeNpc와 상호작용 시 활성화)
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

    public EquipmentData m_Equipment;

    int m_MaxUpgradeCount = 10;   // 최대 강화 수치

    public override bool Init()
    {
        if (base.Init() == false) return false;

        // 자식 객체 불러오기
        BindObject(typeof(Gameobjects));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        SetInfo();

        // 켜져있을수 있으니 초기화
        Managers.UI.ClosePopupUI(this);

        return true;
    }

    public void RefreshUI(EquipmentData equipment)
    {
        m_Equipment = equipment;

        // 풀강 확인
        if (equipment.upgradeCount >= m_MaxUpgradeCount)
        {
            GetText((int)Texts.ItemNameText).text = m_Equipment.itemName;
            GetText((int)Texts.UpgradeResultText).text = $"Max";
            GetText((int)Texts.UpgradeGoldText).text = "";
        }
        else
        {
            GetText((int)Texts.ItemNameText).text = m_Equipment.itemName;
            GetText((int)Texts.UpgradeResultText).text = $"{m_Equipment.upgradeCount}   →   {m_Equipment.upgradeCount + 1}";
            GetText((int)Texts.UpgradeGoldText).text = EquipmentUpgradeGold(m_Equipment).ToString();
        }
    }

    // 강화 진행 버튼
    void OnClickUpgradeButton()
    {
        if (m_Equipment.IsNull() == true) return;

        // 강화 수치 Max 확인
        if (m_Equipment.upgradeCount >= m_MaxUpgradeCount) return;

        // 금액 확인
        int a_UpgradeGold = EquipmentUpgradeGold(m_Equipment);
        if (Managers.Game.Gold < a_UpgradeGold)
        {
            GetText((int)Texts.ItemNameText).text = "금액이 부족합니다!";
            return;
        }

        Managers.Game.Gold -= a_UpgradeGold;

        // 강화 적용
        EquipmentUpgrade(m_Equipment);
        RefreshUI(m_Equipment);
    }

    // 강화 비용 계산
    int EquipmentUpgradeGold(EquipmentData equipment)
    {
        // 강화 금액 : 아이템 판매 가격 + ((판매 가격 / 2) * 강화 횟수)
        int gold = equipment.itemPrice + (int)((equipment.itemPrice / 4) * (equipment.upgradeCount));
        return gold;
    }

    // 강화 적용
    void EquipmentUpgrade(EquipmentData equipment)
    {
        equipment.upgradeCount += 1;

        // 장비 타입 확인 후 강화 적용
        if (equipment is WeaponItemData)
        {
            WeaponItemData weapon = equipment as WeaponItemData;

            weapon.addAttack = weapon.upgradeValue * weapon.upgradeCount;
        }
        else if (equipment is ArmorItemData)
        {
            ArmorItemData armor = equipment as ArmorItemData;

            armor.addDefnece = armor.upgradeValue * armor.upgradeCount;
            armor.addHp = (armor.upgradeValue * 5) * armor.upgradeCount;
            armor.addMp = (armor.upgradeValue * 5) * armor.upgradeCount;
        }
    }

    public void ExitUpgrade()
    {
        if (m_Equipment.IsNull() == false)
            Managers.Game._playScene._inventory.AcquireItem(m_Equipment);

        Clear();

        // 강화 슬롯 초기화
        GetObject((int)Gameobjects.ItemSlot).GetComponent<UI_UpgradeSlot>().ClearSlot();

        Managers.Game.IsInteract = false;

        Managers.UI.CloseAllPopupUI();
    }

    public void Clear()
    {
        m_Equipment = null;

        Managers.Game._playScene._slotTip.OnSlotTip(false);

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
