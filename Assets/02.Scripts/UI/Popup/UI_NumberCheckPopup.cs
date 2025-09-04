using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;



// 개수 확인 Popup UI 
public class UI_NumberCheckPopup : UI_Popup
{
    enum Gameobjects
    {
        Background,
    }

    enum Buttons
    {
        MinusButton,
        PlusButton,
        NoButton,
        YesButton,
    }

    int ItemCount = 0;      // 현재 개수
    int ItemMaxCount = 0;   // 최대 개수

    Action<int> _onClickYesButton;  // 확인 버튼 누를 시 호출
    UI_InvenSlot m_InvenItem;         // 인벤토리 슬롯

    [SerializeField] Slider m_NumberSlider;

    [SerializeField] Text m_ItemCountText;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        // 자식 객체 불러오기
        BindObject(typeof(Gameobjects));
        BindButton(typeof(Buttons));

        // 버튼 기능 등록
        GetButton((int)Buttons.MinusButton).onClick.AddListener(OnClickMinusButton);
        GetButton((int)Buttons.PlusButton).onClick.AddListener(OnClickPlusButton);
        GetButton((int)Buttons.NoButton).onClick.AddListener(OnClickNoButton);
        GetButton((int)Buttons.YesButton).onClick.AddListener(OnClickYesButton);

        // Order 설정
        GetObject((int)Gameobjects.Background).BindEvent((PointerEventData eventData) =>
        {
            Managers.UI.SetOrder(GetComponent<Canvas>());
        }, Define.UIEvent.Click);

        // 슬라이더 사용 시 기능 등록
        m_NumberSlider.onValueChanged.AddListener((float value) =>
        {
            ItemCount = (int)value;
            m_ItemCountText.text = ItemCount.ToString();
        });

        return true;
    }

    // 인벤토리 받으며 세팅 (판매할 때 사용 중)
    public void SetInfo(UI_InvenSlot a_InvenItem, Action<int> onClickYesButton)
    {
        _onClickYesButton = onClickYesButton;
        m_InvenItem = a_InvenItem;

        ItemMaxCount = a_InvenItem.ItemCount;

        RefreshUI();
    }

    // 아이템 받으며 세팅 (구매할 때 사용 중)
    public void SetInfo(ItemData item, Action<int> onClickYesButton)
    {
        _onClickYesButton = onClickYesButton;

        ItemMaxCount = (int)(Managers.Game.Gold / item.itemPrice);

        RefreshUI();
    }

    // 마이너스 버튼
    void OnClickMinusButton()
    {
        ItemCount = Mathf.Clamp(--ItemCount, 1, ItemMaxCount);
        m_NumberSlider.value = ItemCount;
        m_ItemCountText.text = ItemCount.ToString();
    }

    // 플러스 버튼
    void OnClickPlusButton()
    {
        ItemCount = Mathf.Clamp(++ItemCount, 1, ItemMaxCount);
        m_NumberSlider.value = ItemCount;
        m_ItemCountText.text = ItemCount.ToString();
    }

    // 확인 버튼
    void OnClickYesButton()
    {
        Managers.UI.ClosePopupUI(this);

        if (_onClickYesButton.IsNull() == false)
            _onClickYesButton.Invoke(ItemCount);
    }

    // 취소 버튼
    void OnClickNoButton()
    {
        Managers.UI.ClosePopupUI(this);
    }

    void RefreshUI()
    {
        Managers.UI.SetOrder(GetComponent<Canvas>());

        ItemCount = 1;

        m_NumberSlider.minValue = ItemCount;
        m_NumberSlider.maxValue = ItemMaxCount;
        m_NumberSlider.value = ItemCount;

        m_ItemCountText.text = ItemCount.ToString();
    }
}
