using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameScene : BaseScene
{
    [SerializeField]
    Transform playerSpawn;

    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Game;  // 타입 설정

        Managers.Game.defualtSpawn = playerSpawn.position;

        // 플레이어 캐릭터 생성
        if (Managers.Game.GetPlayer().IsFakeNull() == true)
        {
            GameObject a_Player = Managers.Game.Spawn(Define.WorldObject.Player, "Player");
            a_Player.transform.position = playerSpawn.position;
            DontDestroyOnLoad(a_Player);
        }

        // UI 생성
        if (Managers.Game.m_PlayScene.IsFakeNull() == true)
        {
            Managers.Game.Init();
            Managers.Game.m_PlayScene = Managers.UI.ShowSceneUI<UI_PlayScene>();
            DontDestroyOnLoad(Managers.Game.m_PlayScene.gameObject);
        }
        else
            Managers.Game.m_PlayScene.IsMiniMap(true);

        // 플레이어 세이브 위치 이동
        if (Managers.Game.CurrentPos != Vector3.zero)
            Managers.Game.GetPlayer().transform.position = Managers.Game.CurrentPos;

        // 클릭 Effect 생성
        if (Managers.Game.GetPlayer().IsFakeNull() == false)
        {
            GameObject a_ClickMoveEff = Managers.Resource.Instantiate("Effect/ClickMoveEffect");
            a_ClickMoveEff.SetActive(false);

            Managers.Game.GetPlayer().GetComponent<Player_Ctrl>().m_ClickMoveObj = a_ClickMoveEff;
        }
        
        // 카메라 조정
        Camera.main.gameObject.GetOrAddComponent<Camera_Ctrl>().SetPlayer(Managers.Game.GetPlayer());
    }

    public override void Clear()
    {

    }
}
