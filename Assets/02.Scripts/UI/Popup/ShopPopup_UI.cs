using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPopup_UI : MonoBehaviour
{
    public GameObject m_ShopPopup;
    public Button m_CloseBtn;

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
            m_ShopPopup.SetActive(false);
            InvenPopup_UI.Inst.m_InvenPopup.gameObject.SetActive(false);
        });
    }
}
