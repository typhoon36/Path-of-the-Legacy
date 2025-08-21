using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ItemPickUp : MonoBehaviour
{
    public ItemData m_Item;
    public int m_ItemCount = 1;      // 아이템 전용 개수

    float m_ScanRange = 5f;     // 플레이어 스캔 거리

    UI_NameBar m_NameBarUI = null;

    void Start()
    {
        // 이름바 생성 및 자식으로 배치
        m_NameBarUI = Managers.UI.MakeWorldSpaceUI<UI_NameBar>(transform);

        if (m_ItemCount > 1)
            m_NameBarUI.m_NameText = m_Item.ItemName + $" ({m_ItemCount})";

        else
            m_NameBarUI.m_NameText = m_Item.ItemName;

        m_NameBarUI.m_NameText += "[줍기]";
    }

    void FixedUpdate()
    {
        // 이름바 Null Check
        if (m_NameBarUI.IsNull() == false)
        {
            // 플레이어 Null Check
            if (Managers.Game.GetPlayer().IsNull() == true) return;

            // 플레이어와 거리 체크
            float a_Dist = (Managers.Game.GetPlayer().transform.position - transform.position).magnitude;


            if (a_Dist <= m_ScanRange)
                m_NameBarUI.gameObject.SetActive(true);

            else
                m_NameBarUI.gameObject.SetActive(false);
        }
    }
}
