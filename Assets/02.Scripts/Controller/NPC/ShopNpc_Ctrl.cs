using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopNpc_Ctrl : Npc_Ctrl
{
    public Define_S.ShopType m_ShopType = Define_S.ShopType.Unknown;

    [SerializeField]
    int m_ShopId;

    protected override void OpenPopup()
    {
        OpenShop();
    }

    protected override void ExitPopup()
    {
        ExitShop();
    }

    void OpenShop()
    {
        // 팝업 활성화
        ShopPopup_UI.Inst.m_ShopPopup.gameObject.SetActive(true);
        InvenPopup_UI.Inst.m_InvenPopup.gameObject.SetActive(true);

        // 상점 타입에 따라 상품 로드
        ShopPopup_UI.Inst.LoadProducts(m_ShopType);
    }

    void ExitShop()
    {
        // 팝업 비활성화
        ShopPopup_UI.Inst.m_ShopPopup.gameObject.SetActive(false);
        InvenPopup_UI.Inst.m_InvenPopup.gameObject.SetActive(false);
    }
}
