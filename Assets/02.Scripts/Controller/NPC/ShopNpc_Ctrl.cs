using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopNpc_Ctrl : Npc_Ctrl
{
    //상점 타입
    public Define_S.ShopType m_ShopType = Define_S.ShopType.Unknown;

    //상점 Id
    [SerializeField]
    int m_ShopId;

    protected override void OpenPopup()
    {
        // UI에 마우스가 올라가 있는지 체크
        if (EventSystem.current.IsPointerOverGameObject()) return;

        // 상점 열기
        OpenShop();
    }

    protected override void ExitPopup()
    {
        //팝업 닫기
        ExitShop();
    }

    void OpenShop()
    {
        // 팝업 활성화
        ShopPopup_UI.Inst.m_ShopPopup.gameObject.SetActive(true);
        InvenPopup_UI.Inst.m_InvenPopup.gameObject.SetActive(true);//인벤토리 활성화

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
