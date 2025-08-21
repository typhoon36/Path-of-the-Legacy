using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class UpgradeNpc_Ctrl : Npc_Ctrl
{
    protected override void OpenPopup() { OpenUpgrade(); }
    protected override void ExitPopup() { ExitUpgrade(); }

    void OpenUpgrade()
    {
        // 업그레이드 Popup 활성화
        Managers.UI.OnPopupUI(Managers.Game.m_PlayScene.Upgrade);

        // 인벤토리 Popup 활성화
        Managers.UI.OnPopupUI(Managers.Game.m_PlayScene.Inventory);
        Managers.Game.m_PlayScene.Inventory.ResetPos();
    }

    void ExitUpgrade()
    {
        Managers.Game.m_PlayScene.Upgrade.ExitUpgrade();
    }
}
