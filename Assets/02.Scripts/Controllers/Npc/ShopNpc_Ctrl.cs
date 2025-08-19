using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   ShopNpcController.cs
 * Desc :   상점 Npc 기능 구현
 *
 & Functions
 &  [Protected]
 &  : OpenPopup()   - Popup 활성화   (OpenShop() 호출)
 &  : ExitPopup()   - Popup 비활성화 (ExitShop() 호출)
 &
 &  []
 &  : OpenShop()    - 상점 Popup 열기
 &  : ExitShop()    - 상점 Popup 나가기
 *
 */

public class ShopNpc_Ctrl : Npc_Ctrl
{
    public Define.ShopType  shopType = Define.ShopType.Unknown;

    [SerializeField]
     int             shopBuyId;      // Shop Npc 구매 목록 Id

    protected override void OpenPopup() { OpenShop(); }
    protected override void ExitPopup() { ExitShop(); }

     void OpenShop()
    {
        // 상점 Popup 활성화
        Managers.UI.OnPopupUI(Managers.Game.m_PlayScene.Shop);
        Managers.Game.m_PlayScene.Shop.RefreshUI(this, shopBuyId);

        // 인벤토리 Popup 활성화
        Managers.UI.OnPopupUI(Managers.Game.m_PlayScene.Inventory);
        Managers.Game.m_PlayScene.Inventory.ResetPos();
    }

     void ExitShop()
    {
        Managers.Game.m_PlayScene.Shop.ExitShop();
    }
}
