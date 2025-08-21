using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



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

    int itemCount = 0;      // 현재 개수
    int itemMaxCount = 0;   // 최대 개수

    Action<int> _onClickYesButton;
    UI_InvenSlot _invenItem;

    [SerializeField] Slider NumberSlider;

    [SerializeField] Text _itemCountText;

    public override bool Init()
    {
        if (base.Init() == false) return false;

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
        NumberSlider.onValueChanged.AddListener((float value) =>
        {
            itemCount = (int)value;
            _itemCountText.text = itemCount.ToString();
        });

        return true;
    }

    // 인벤토리 받으며 세팅 (판매할 때 사용 중)
    public void SetInfo(UI_InvenSlot invenItem, Action<int> onClickYesButton)
    {
        _onClickYesButton = onClickYesButton;
        _invenItem = invenItem;

        itemMaxCount = invenItem.ItemCount;

        RefreshUI();
    }

    // 아이템 받으며 세팅 (구매할 때 사용 중)
    public void SetInfo(ItemData item, Action<int> onClickYesButton)
    {
        _onClickYesButton = onClickYesButton;

        itemMaxCount = (int)(Managers.Game.Gold / item.ItemPrice);

        RefreshUI();
    }

    // 마이너스 버튼
    void OnClickMinusButton()
    {
        itemCount = Mathf.Clamp(--itemCount, 1, itemMaxCount);
        NumberSlider.value = itemCount;
        _itemCountText.text = itemCount.ToString();
    }

    // 플러스 버튼
    void OnClickPlusButton()
    {
        itemCount = Mathf.Clamp(++itemCount, 1, itemMaxCount);
        NumberSlider.value = itemCount;
        _itemCountText.text = itemCount.ToString();
    }

    // 확인 버튼
    void OnClickYesButton()
    {
        Managers.UI.ClosePopupUI(this);

        if (_onClickYesButton.IsNull() == false)
            _onClickYesButton.Invoke(itemCount);
    }

    // 취소 버튼
    void OnClickNoButton()
    {
        Managers.UI.ClosePopupUI(this);
    }

    void RefreshUI()
    {
        Managers.UI.SetOrder(GetComponent<Canvas>());

        itemCount = 1;

        NumberSlider.minValue = itemCount;
        NumberSlider.maxValue = itemMaxCount;
        NumberSlider.value = itemCount;

        _itemCountText.text = itemCount.ToString();
    }
}
