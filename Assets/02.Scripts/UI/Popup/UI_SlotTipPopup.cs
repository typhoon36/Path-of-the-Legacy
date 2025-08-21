using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class UI_SlotTipPopup : UI_Popup
{
    enum Gameobjects
    {
        Background,
    }

    enum Images
    {
        ItemImage,
    }

    enum Texts
    {
        ItemNameText,
        ItemTypeText,
        ItemGradeText,
        ItemLevelText,
        ItemStatText,
    }

    public RectTransform    background;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(Gameobjects));
        BindImage(typeof(Images));
        BindText(typeof(Texts));

        background = GetObject((int)Gameobjects.Background).GetComponent<RectTransform>();

        Managers.UI.ClosePopupUI(this);

        return true;
    }

    // 슬롯 팁 활성화
    public void OnSlotTip(bool isActive)
    {
        if (isActive)
            Managers.UI.OnPopupUI(this);
        else
        {
            if (this.gameObject.activeSelf == true)
                Managers.UI.ClosePopupUI(this);
        }
    }

    // 아이템 정보 확인시 새로고침
    public void RefreshUI(ItemData item)
    {
        if (item.IsNull() == true)
        {
            Debug.Log("아이템 정보가 없습니다.");
            OnSlotTip(false);
            return;
        }

        // 위치 설정
        RectTransform tipRect = background;
        Vector3 slotTipPos = background.anchoredPosition;
        slotTipPos.x = slotTipPos.x + (tipRect.rect.width * 0.65f);
        slotTipPos.y = slotTipPos.y - (tipRect.rect.height * 0.65f); 
        background.anchoredPosition = slotTipPos;

        GetImage((int)Images.ItemImage).sprite = item.ItemIcon;

        GetText((int)Texts.ItemNameText).text = item.ItemName;
        GetText((int)Texts.ItemTypeText).text = item.ItemType.ToString();
        GetText((int)Texts.ItemGradeText).text = item.ItemGrade.ToString();

        // 아이템 등급에 따른 색깔
        switch(item.ItemGrade)
        {
            case Define.ItemGrade.Common:
                SetColor(Color.white);
                break;
            case Define.ItemGrade.Rare:
                SetColor(Color.green);
                break;
            case Define.ItemGrade.Epic:
                SetColor(Color.blue);
                break;
            case Define.ItemGrade.Legendary:
                SetColor(Color.yellow);
                break;
        }

        // 장비라면 
        if (item is EquipmentData)
        {
            // 강화가 됐다면
            if ((item as EquipmentData).UpgradeCount > 0)
                GetText((int)Texts.ItemNameText).text = item.ItemName +  $" [+{(item as EquipmentData).UpgradeCount}]";
        }    
        
        // 아이템 종류 별로 세팅
        if (item.ItemType == Define.ItemType.Use)
        {
            GetText((int)Texts.ItemLevelText).text = "";
            GetText((int)Texts.ItemStatText).text = item.ItemDesc;
        }
        else if (item.ItemType == Define.ItemType.Armor)
        {
            ArmorItemData armor = item as ArmorItemData;
            GetText((int)Texts.ItemLevelText).text = "최소레벨 " + armor.MinLevel;

            string statStr = "";
            // 강화 확인
            if (armor.UpgradeCount > 0)
            {
                statStr += armor.Defnece    > 0 ? $"방어력 {armor.Defnece} (+{armor.AddDefnece})\n" : "";
                statStr += armor.Hp         > 0 ? $"체력 {armor.Hp} (+{armor.AddHp})\n" : "";
                statStr += armor.Mp         > 0 ? $"마나 {armor.Mp} (+{armor.AddMp})\n" : "";
            }
            else
            {
                statStr += armor.Defnece    > 0 ? $"방어력 {armor.Defnece}\n" : "";
                statStr += armor.Hp         > 0 ? $"체력 {armor.Hp}\n" : "";
                statStr += armor.Mp         > 0 ? $"마나 {armor.Mp}\n" : "";
            }
            
            statStr += armor.MoveSpeed > 0 ? $"이동속도 {armor.MoveSpeed}\n" : "";

            GetText((int)Texts.ItemStatText).text = statStr;
        }
        else if (item.ItemType == Define.ItemType.Weapon)
        {
            WeaponItemData weapon = item as WeaponItemData;
            GetText((int)Texts.ItemLevelText).text = "최소레벨 " + weapon.MinLevel;

            // 강화 확인
            if (weapon.UpgradeCount > 0)
                GetText((int)Texts.ItemStatText).text = $"공격력 {weapon.Attack} (+{weapon.AddAttack})";
            else
                GetText((int)Texts.ItemStatText).text = $"공격력 {weapon.Attack}";
        }
    }

     void SetColor(Color color)
    {
        GetText((int)Texts.ItemNameText).color = color;
        GetText((int)Texts.ItemGradeText).color = color;
    }
}
