using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 상점 Npc
public class ShopNpcController : NpcController
{
    public Define.ShopType shopType = Define.ShopType.Unknown;

    [SerializeField] int m_ShopBuyId;      // Shop Npc 구매 목록 Id

    protected override void OpenPopup() { OpenShop(); }
    protected override void ExitPopup() { ExitShop(); }

    void OpenShop()
    {
        // 상점 Popup 활성화
        Managers.UI.OnPopupUI(Managers.Game._playScene._shop);
        Managers.Game._playScene._shop.RefreshUI(this, m_ShopBuyId);

        // 인벤토리 Popup 활성화
        Managers.UI.OnPopupUI(Managers.Game._playScene._inventory);
        Managers.Game._playScene._inventory.ResetPos();
    }

    void ExitShop()
    {
        Managers.Game._playScene._shop.ExitShop();
    }
}
