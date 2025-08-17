using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CustomScene : BaseScene
{
    [SerializeField] Transform m_CharacterPos;

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.PlayerCustom;

        GameObject charCustom = Managers.Resource.Instantiate("CharacterCustom", m_CharacterPos);
        Managers.UI.ShowSceneUI<UI_CustomScene>().m_Custom = charCustom.GetComponent<CharacterCustom>();
    }

    public override void Clear() {}
}
