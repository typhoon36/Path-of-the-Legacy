using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//��ǰ ����
public class Product_Nd : MonoBehaviour
{
    public Image m_Icon;
    public Text m_ItemTxt;
    public Text m_PriceTxt;
    public Button m_BuyBtn;

    public GameObject m_MoreInfo; //��ǰ �� ���� �˾�

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

    //�ּҸ� �޾ƿ���(ReadOnly�� ������ �ٸ� ������ ���� �������� ���ϰ� �ϱ� ����)
    static readonly Dictionary<int, string> IconPathMap = new Dictionary<int, string>
    {
        { 1,    "Items/Potions/grass_potion" },
        { 2,    "Items/Potions/wind_potion" },
        { 3,    "Items/Armor/01_Leather_chest" },
        { 4,    "Items/Armor/01_plate_chest" },
        { 5,    "Items/Armor/06_leather_pants" },
        { 6,    "Items/Armor/06_plate_pants" },
        { 7,    "Items/Armor/05_leather_boots" },
        {8,     "Items/Armor/05_plate_boots" },
        {9,     "Items/Weapons/Sword_1" },
        {10,    "Items/Weapons/Sword_2" },
        {11,    "Items/Weapons/Ax_1" },
        {12,    "Items/Weapons/Ax_2" },
        {13,    "Items/Weapons/Ax_3" }
    };

    public void Init(ItemData a_ItemData)
    {
        m_ItemData = a_ItemData;
        if (IconPathMap.TryGetValue(a_ItemData.Id, out string iconPath))
        {
            m_Icon.sprite = Resources.Load<Sprite>(iconPath); // ������ �ε�
        }
        m_ItemTxt.text = a_ItemData.ItemName;
        m_PriceTxt.text = a_ItemData.ItemPrice.ToString();
    }
}
