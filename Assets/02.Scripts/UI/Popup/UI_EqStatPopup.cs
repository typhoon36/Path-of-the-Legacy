using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//장비&스탯 팝업
public class UI_EqStatPopup : UI_Popup
{
    enum Gameobjects
    {
        Background,
        Title,
        EqSlot,
        ExitButton,
        StatBackground,
    }

    enum Buttons
    {
        HpAddPointButton,
        MpAddPointButton,
        STRAddPointButton,
        LUKAddPointButton,
        StatButton,
    }

    enum Texts
    {
        StatPointText,
        HpStatPointText,
        MpStatPointText,
        STRStatPointText,
        LUKStatPointText,
        StatNameText,
        StatText,
    }

    public List<UI_ArmorSlot> ArmorSlots; // 방어구 슬롯 List
    public UI_WeaponSlot WeaponSlot; // 무기 슬롯

    bool IsClickStatButton = false;  // 스탯 버튼을 눌렀는가?

    public override bool Init()
    {
        if (base.Init() == false) return false;

        ArmorSlots = new List<UI_ArmorSlot>();
        popupType = Define.Popup.Equipment;

        // 자식 객체 불러오기
        BindObject(typeof(Gameobjects));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        // InputManager에 입력 등록
        Managers.Input.KeyAction -= OnEquipmentUI;
        Managers.Input.KeyAction += OnEquipmentUI;

        SetInfo();

        Invoke("DelayInit", 0.0001f);

        return true;
    }
    void DelayInit() { Managers.UI.ClosePopupUI(this); }

    void Update()
    {
        // 장비창 활성화되면 실시간 새로고침
        if (Managers.Game.isPopups[Define.Popup.Equipment] == true)
            RefreshUI();
    }

    // 장비 장착
    public void SetEquipment(UI_ItemSlot a_ItemSlot)
    {
        // 무기, 장비 확인
        if (a_ItemSlot.Item.itemType == Define.ItemType.Armor)
        {
            ArmorItemData armor = a_ItemSlot.Item as ArmorItemData;

            // 장비 부위 체크
            foreach (UI_ArmorSlot armorSlot in ArmorSlots)
            {
                // 같은 부위면 장착
                if (armorSlot.ArmorType == armor.armorType)
                {
                    armorSlot.ChangeArmor(a_ItemSlot);
                    break;
                }
            }
        }
        // 무기면 장착
        else if (a_ItemSlot.Item.itemType == Define.ItemType.Weapon)
        {
            WeaponSlot.ChangeWeapon(a_ItemSlot);
        }
    }

    // 장비/스탯 Popup 활성화
    void OnEquipmentUI()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Managers.Game.isPopups[Define.Popup.Equipment] = !Managers.Game.isPopups[Define.Popup.Equipment];

            // 장비/스탯 Popup On/Off
            if (Managers.Game.isPopups[Define.Popup.Equipment])
                Managers.UI.OnPopupUI(this);
            else
                Exit();
        }
    }

    void SetInfo()
    {
        // 버튼 기능 등록
        GetButton((int)Buttons.HpAddPointButton).onClick.AddListener(() => { AddStat(Buttons.HpAddPointButton); });
        GetButton((int)Buttons.MpAddPointButton).onClick.AddListener(() => { AddStat(Buttons.MpAddPointButton); });
        GetButton((int)Buttons.STRAddPointButton).onClick.AddListener(() => { AddStat(Buttons.STRAddPointButton); });
        GetButton((int)Buttons.LUKAddPointButton).onClick.AddListener(() => { AddStat(Buttons.LUKAddPointButton); });

        GetButton((int)Buttons.StatButton).onClick.AddListener(OnClickStatButton);
        GetObject((int)Gameobjects.StatBackground).SetActive(false);

        // 스탯 정보 string
        string statNameText =
$@"<color=white>이름</color>
<color=yellow>Lv.</color>
<color=white>체력</color>
<color=white>마나</color>
<color=white>이동속도</color>
<color=red>공격력</color>
<color=blue>방어력</color>";

        GetText((int)Texts.StatNameText).text = statNameText;

        SetEventHandler();
    }

    // 스탯 포인트 적용
    void AddStat(Buttons stat)
    {
        if (Managers.Game.StatPoint == 0)
            return;

        switch (stat)
        {
            case Buttons.HpAddPointButton:  // Hp
                Managers.Game.HpPoint++;
                break;
            case Buttons.MpAddPointButton:  // Mp
                Managers.Game.MpPoint++;
                break;
            case Buttons.STRAddPointButton: // STR
                Managers.Game.STR++;
                break;
            case Buttons.LUKAddPointButton: // LUK
                Managers.Game.LUK++;
                break;
        }

        Managers.Game.StatPoint--;          // 스탯 포인트 차감
        RefreshUI();                        // UI 새로고침
    }

    // 스탯 버튼 클릭
    void OnClickStatButton()
    {
        IsClickStatButton = !IsClickStatButton;

        // 스탯 정보 활성화
        GetObject((int)Gameobjects.StatBackground).SetActive(IsClickStatButton);
    }

    void SetEventHandler()
    {
        // Title 잡고 인벤토리 이동
        RectTransform  a_EqStatPos = GetObject((int)Gameobjects.Background).GetComponent<RectTransform>();
        GetObject((int)Gameobjects.Title).BindEvent((PointerEventData eventData) =>
        {
            a_EqStatPos.anchoredPosition = new Vector2
            (
                Mathf.Clamp(a_EqStatPos.anchoredPosition.x + eventData.delta.x, -947, 655),
                Mathf.Clamp(a_EqStatPos.anchoredPosition.y + eventData.delta.y, -200, 100)
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
            Managers.UI.ClosePopupUI(this);
        }, Define.UIEvent.Click);
    }

    void RefreshUI()
    {
        // 현재 스탯 불러오기
        GetText((int)Texts.StatPointText).text = "소지포인트 : " + Managers.Game.StatPoint.ToString();
        GetText((int)Texts.HpStatPointText).text = Managers.Game.HpPoint.ToString();
        GetText((int)Texts.MpStatPointText).text = Managers.Game.MpPoint.ToString();
        GetText((int)Texts.STRStatPointText).text = Managers.Game.STR.ToString();
        GetText((int)Texts.LUKStatPointText).text = Managers.Game.LUK.ToString();

        // 스탯 정보
        string statText =
$@"<color=white>{Managers.Game.Name}</color>
<color=white>{Managers.Game.Level}</color>
<color=white>{Managers.Game.MaxHp} {(Managers.Game.addHp != 0 ? $"(+{Managers.Game.addHp})" : "")}</color>
<color=white>{Managers.Game.MaxMp} {(Managers.Game.addMp != 0 ? $"(+{Managers.Game.addMp})" : "")}</color>
<color=white>{Managers.Game.MoveSpeed} {(Managers.Game.addMoveSpeed != 0 ? $"(+{Managers.Game.addMoveSpeed})" : "")}</color>
<color=white>{Managers.Game.Attack}</color>
<color=white>{Managers.Game.Defense}</color>";

        GetText((int)Texts.StatText).text = statText;
    }

    void Exit()
    {
        IsClickStatButton = false;
        GetObject((int)Gameobjects.StatBackground).SetActive(IsClickStatButton);

        Managers.Game._playScene._slotTip.OnSlotTip(false);
        Managers.UI.ClosePopupUI(this);
    }
}
