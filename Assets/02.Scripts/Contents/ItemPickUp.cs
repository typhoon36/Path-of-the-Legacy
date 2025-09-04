using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 땅에 떨어진 아이템의 이름 생성
public class ItemPickUp : MonoBehaviour
{
    public ItemData Item;
    public int ItemCount = 1;      // 아이템 전용 개수

    float m_ScanRange = 5f;     // 플레이어 스캔 거리

    UI_NameBar NameBarUI = null;

    void Start()
    {
        // 이름바 생성 및 자식으로 배치
        NameBarUI = Managers.UI.MakeWorldSpaceUI<UI_NameBar>(transform);
        if (ItemCount > 1)
            NameBarUI.m_NameText = Item.itemName + $" ({ItemCount})";
        else
            NameBarUI.m_NameText = Item.itemName;

        NameBarUI.m_NameText += " [F]";
    }

    void FixedUpdate()
    {
        // 이름바 Null Check
        if (NameBarUI.IsNull() == false)
        {
            // 플레이어 Null Check
            if (Managers.Game.GetPlayer().IsNull() == true) return;

            // 플레이어와 거리 체크
            float a_Dist = (Managers.Game.GetPlayer().transform.position - transform.position).magnitude;

            // m_ScanRange만큼 가까우면 활성화
            if (a_Dist <= m_ScanRange) NameBarUI.gameObject.SetActive(true);
            else NameBarUI.gameObject.SetActive(false);
        }
    }
}
