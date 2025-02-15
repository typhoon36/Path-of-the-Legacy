using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopNpc_Ctrl : Npc_Ctrl
{
    //���� Ÿ��
    public Define_S.ShopType m_ShopType = Define_S.ShopType.Unknown;

    //���� Id
    [SerializeField]
    int m_ShopId;

    protected override void OpenPopup()
    {
        // UI�� ���콺�� �ö� �ִ��� üũ
        if (EventSystem.current.IsPointerOverGameObject()) return;

        // ���� ����
        OpenShop();
    }

    protected override void ExitPopup()
    {
        //�˾� �ݱ�
        ExitShop();
    }

    void OpenShop()
    {
        // �˾� Ȱ��ȭ
        ShopPopup_UI.Inst.m_ShopPopup.gameObject.SetActive(true);
        InvenPopup_UI.Inst.m_InvenPopup.gameObject.SetActive(true);//�κ��丮 Ȱ��ȭ

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
