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
    ShopPopup_UI m_ShopPopup;

    void Start()
    {

    }

    public void Init(ItemData a_ItemData)
    {
        m_ItemData = a_ItemData;
        m_Icon.sprite = m_ItemData.ItemIcon;
        m_ItemTxt.text = m_ItemData.ItemName;
        m_PriceTxt.text = m_ItemData.ItemPrice.ToString();
    }
}
