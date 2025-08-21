using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//¾À °ü¸® ¸Å´ÏÀú
public class Scene_Mgr
{
    public BaseScene m_CurScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

    // ¾À ·Îµå
    public void LoadScene(Define.Scene type)
    {
        Managers.Clear();
        SceneManager.LoadScene(GetSceneName(type));
    }

    // ºñµ¿±â ¾À ·Îµå
    public AsyncOperation LoadAsynScene(Define.Scene type)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(Managers.Scene.GetSceneName(type));
        operation.allowSceneActivation = false;

        return operation;
    }

    // ¾À ÀÌ¸§ ¹ÝÈ¯
    string GetSceneName(Define.Scene type)
    {
        string name = System.Enum.GetName(typeof(Define.Scene), type);
        return name;
    }

    public void Clear() { m_CurScene.Clear(); }
}
