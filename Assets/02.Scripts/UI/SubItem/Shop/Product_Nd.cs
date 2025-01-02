using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//상품 정보
public class Product_Nd : MonoBehaviour
{
    public Image m_Icon;
    public Text m_ItemTxt;
    public Text m_PriceTxt;
    public Button m_BuyBtn;

    public GameObject m_MoreInfo; //상품 상세 정보 팝업

    ItemData m_ItemData;

    void Start()
    {
        m_BuyBtn.onClick.AddListener(() =>
        {
            if (InvenPopup_UI.Inst != null)
            {
                InvenPopup_UI.Inst.AddItem(m_ItemData);
            }
        });
    }

    public void Init(ItemData a_ItemData)
    {
        m_ItemData = a_ItemData;
        m_Icon.sprite = m_ItemData.ItemIcon;
        m_ItemTxt.text = m_ItemData.ItemName;
        m_PriceTxt.text = m_ItemData.ItemPrice.ToString();
    }
}
