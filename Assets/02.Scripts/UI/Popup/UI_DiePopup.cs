using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class UI_DiePopup : UI_Popup
{
    enum Buttons
    {
        ResurrectionButton,
        ReSpawnButton,
    }

    enum Images { Background }

    public override bool Init()
    {
        if (base.Init() == false) return false;

        // 자식 객체 불러오기
        BindButton(typeof(Buttons));
        BindImage(typeof(Images));

        // 즉시 부활 버튼
        GetButton((int)Buttons.ResurrectionButton).onClick.AddListener(()=>
        {
            // 제자리 부활 + 체력/마나 50% 회복 + 100골드 차감
            if (Managers.Game.Gold < 100)
            {
                Managers.UI.MakeSubItem<UI_Guide>().SetInfo("골드가 부족합니다!", Color.yellow);
                return;
            }

            Managers.Game.Gold -= 100;

            Managers.Game.OnResurrection(0.5f);

            Managers.UI.ClosePopupUI(this);
        });

        // 마을 부활 버튼 
        GetButton((int)Buttons.ReSpawnButton).onClick.AddListener(()=>
        {
            // 마을 부활 + 체력/마나 20% 회복
            Managers.Game.OnResurrection(0.2f);

            // 현재 씬이 게임 씬이 아니라면 게임 씬으로 로드
            if (Managers.Scene.m_CurScene.SceneType != Define.Scene.Game)
                Managers.Scene.LoadScene(Define.Scene.Game);

            // 플레이어 위치를 기본 스폰 위치로 설정
            Managers.Game.GetPlayer().transform.position = Managers.Game.defualtSpawn;

            // UI 팝업 닫기            
            Managers.UI.ClosePopupUI(this);
        });

        return true;
    }
}
