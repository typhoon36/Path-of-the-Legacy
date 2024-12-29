using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPopup_UI : MonoBehaviour
{
    [Header("ShopPopup")]
    public GameObject m_ShopPopup;
    public GameObject m_ShopContent; // ScrollView Content
    public Button m_CloseBtn;

    [Header("Products")]
    public GameObject m_Products;

    ItemData m_ItemData;

    #region SingleTon
    public static ShopPopup_UI Inst;
    private void Awake()
    {
        Inst = this;
    }
    #endregion

    private void Start()
    {
        m_CloseBtn.onClick.AddListener(() =>
        {
            m_ShopPopup.SetActive(false);
            InvenPopup_UI.Inst.m_InvenPopup.SetActive(true);
        });
    }

    public void LoadProducts(Define_S.ShopType shopType)
    {
        // 扁粮 惑前 力芭
        foreach (Transform child in m_ShopContent.transform)
        {
            Destroy(child.gameObject);
        }

        // 惑前 积己
        foreach (var itemData in Data_Mgr.m_ItemData)
        {
            if (shopType == Define_S.ShopType.Armor && itemData.ItemType != Define_S.ItemType.Armor)
                continue;
            if (shopType == Define_S.ShopType.Weapon && itemData.ItemType != Define_S.ItemType.Weapon)
                continue;
            if (shopType == Define_S.ShopType.Used && itemData.ItemType != Define_S.ItemType.Use)
                continue;

            GameObject t_Product = Instantiate(m_Products, m_ShopContent.transform);
            Product_Nd productNd = t_Product.GetComponent<Product_Nd>();
            productNd.Init(itemData);
        }
    }
}
