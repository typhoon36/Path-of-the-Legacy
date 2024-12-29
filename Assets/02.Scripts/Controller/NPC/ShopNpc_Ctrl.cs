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
        // �˾� Ȱ��ȭ
        ShopPopup_UI.Inst.m_ShopPopup.gameObject.SetActive(true);
        InvenPopup_UI.Inst.m_InvenPopup.gameObject.SetActive(true);

        // ���� Ÿ�Կ� ���� ��ǰ �ε�
        ShopPopup_UI.Inst.LoadProducts(m_ShopType);
    }

    void ExitShop()
    {
        // �˾� ��Ȱ��ȭ
        ShopPopup_UI.Inst.m_ShopPopup.gameObject.SetActive(false);
        InvenPopup_UI.Inst.m_InvenPopup.gameObject.SetActive(false);
    }
}
