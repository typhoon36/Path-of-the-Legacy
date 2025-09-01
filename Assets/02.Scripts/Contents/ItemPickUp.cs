using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 땅에 떨어진 아이템의 이름 생성

public class ItemPickUp : MonoBehaviour
{
    public ItemData Item;
    public int ItemCount = 1;      // 아이템 전용 개수

    float scanRange = 5f;     // 플레이어 스캔 거리

    UI_NameBar nameBarUI = null;

    void Start()
    {
        // 이름바 생성 및 자식으로 배치
        nameBarUI = Managers.UI.MakeWorldSpaceUI<UI_NameBar>(transform);
        if (ItemCount > 1)
            nameBarUI.nameText = Item.itemName + $" ({ItemCount})";
        else
            nameBarUI.nameText = Item.itemName;

        nameBarUI.nameText += " [F]";
    }

    void FixedUpdate()
    {
        // 이름바 Null Check
        if (nameBarUI.IsNull() == false)
        {
            // 플레이어 Null Check
            if (Managers.Game.GetPlayer().IsNull() == true)
                return;

            // 플레이어와 거리 체크
            float distance = (Managers.Game.GetPlayer().transform.position - transform.position).magnitude;

            // scanRange만큼 가까우면 활성화
            if (distance <= scanRange)
                nameBarUI.gameObject.SetActive(true);
            else
                nameBarUI.gameObject.SetActive(false);
        }
    }
}
