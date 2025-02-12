using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopPopup_UI : MonoBehaviour
{
    [Header("ShopPopup")]
    public GameObject m_ShopPopup;
    public GameObject m_ShopBar;
    public GameObject m_ShopContent; // ScrollView Content
    public Button m_CloseBtn;

    [Header("Products")]
    public GameObject m_Products;


    #region SingleTon
    public static ShopPopup_UI Inst;
    private void Awake()
    {
        Inst = this;
    }
    #endregion

    void Start()
    {
        m_CloseBtn.onClick.AddListener(() =>
        {
            if (EventSystem.current.IsPointerOverGameObject() == false) return;

            m_ShopPopup.SetActive(false);
            InvenPopup_UI.Inst.m_InvenPopup.SetActive(false);
        });
    }
   
    public void LoadProducts(Define_S.ShopType shopType)
    {
        // 扁粮 惑前 力芭
        foreach (Transform child in m_ShopContent.transform)
        {
            Destroy(child.gameObject);
        }

        HashSet<int> addedItemIds = new HashSet<int>();

        // 惑前 积己
        foreach (var itemData in Data_Mgr.m_ItemData)
        {
            if (shopType == Define_S.ShopType.Armor && itemData.ItemType != Define_S.ItemType.Armor)
                continue;
            if (shopType == Define_S.ShopType.Weapon && itemData.ItemType != Define_S.ItemType.Weapon)
                continue;
            if (shopType == Define_S.ShopType.Used && itemData.ItemType != Define_S.ItemType.Use)
                continue;

            if (addedItemIds.Contains(itemData.Id))
                continue;

            GameObject t_Product = Instantiate(m_Products, m_ShopContent.transform);
            Product_Nd productNd = t_Product.GetComponent<Product_Nd>();
            productNd.Init(itemData);

            addedItemIds.Add(itemData.Id);
        }
    }

    public void ShowDesc(ItemData itemData)
    {
        if (Desc_Nd.Inst != null && itemData != null)
        {
            Desc_Nd.Inst.m_DescObj.SetActive(true);
            Desc_Nd.Inst.m_NameTxt.text = itemData.ItemName;
            Desc_Nd.Inst.m_DescText.text = itemData.ItemDesc;
            if (Product_Nd.IconPathMap.TryGetValue(itemData.Id, out string iconPath))
            {
                Desc_Nd.Inst.m_Icon.sprite = Resources.Load<Sprite>(iconPath);
            }
        }
    }

}
