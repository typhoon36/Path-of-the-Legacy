using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//상품 정보
public class Product_Nd : MonoBehaviour, IPointerClickHandler
{
    public Image m_Icon;
    public Text m_ItemTxt;
    public Text m_PriceTxt;
    public Button m_BuyBtn;

    ItemData m_ItemData;

    void Start()
    {
        m_BuyBtn.onClick.AddListener(() =>
        {
            if (InvenPopup_UI.Inst != null)
            {
                InvenPopup_UI.Inst.AddItem(m_ItemData);
                InvenPopup_UI.Inst.AddGold(-m_ItemData.ItemPrice);
            }
        });
    }
  
    public void Init(ItemData a_ItemData)
    {
        m_ItemData = a_ItemData;
        if (IconPathMap.TryGetValue(a_ItemData.Id, out string iconPath))
            m_Icon.sprite = Resources.Load<Sprite>(iconPath); // 아이콘 로드

        m_ItemTxt.text = a_ItemData.ItemName;
        m_PriceTxt.text = a_ItemData.ItemPrice.ToString();
    }

    //주소를 받아오기(ReadOnly인 이유는 다른 곳에서 값을 변경하지 못하게 하기 위함)
    public static readonly Dictionary<int, string> IconPathMap = new Dictionary<int, string>
    {
        { 1,    "Items/Potions/grass_potion" },
        { 2,    "Items/Potions/wind_potion" },
        { 3,    "Items/Armor/01_Leather_chest" },
        { 4,    "Items/Armor/01_plate_chest" },
        { 5,    "Items/Armor/06_leather_pants" },
        { 6,    "Items/Armor/06_plate_pants" },
        { 7,    "Items/Armor/05_leather_boots" },
        { 8,     "Items/Armor/05_plate_boots" },
        { 9,     "Items/Weapons/Sword_1" },
        { 10,    "Items/Weapons/Sword_2" },
        { 11,    "Items/Weapons/Ax_1" },
        { 12,    "Items/Weapons/Ax_2" },
        { 13,    "Items/Weapons/Ax_3" },
        { 14,     "Items/Weapons/Hammer"},
        { 15,     "Items/Weapons/Shield"}
    };

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (ShopPopup_UI.Inst != null)
            {
                ShopPopup_UI.Inst.ShowDesc(m_ItemData);
            }
        }
    }
}
