using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductInfo : MonoBehaviour
{
    public Text m_ItemNameTxt;
    public Text m_ItemDescTxt;

    ItemData m_ItemData;

    void Start()
    {
        m_ItemNameTxt.text = m_ItemData.ItemName;
        m_ItemDescTxt.text = m_ItemData.ItemDesc;
    }


}
