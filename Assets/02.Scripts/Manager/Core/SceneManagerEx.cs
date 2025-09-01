using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


//씬 관리
public class SceneManagerEx
{
    // 현재 씬
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

    //씬을 로드하기전 초기화
    public void LoadScene(Define.Scene type)
    {
        Managers.Clear();
        SceneManager.LoadScene(GetSceneName(type));
    }

    //비동기 씬 로드
    public AsyncOperation LoadAsynScene(Define.Scene type)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(Managers.Scene.GetSceneName(type));
        operation.allowSceneActivation = false;

        return operation;
    }

    //씬 이름 가져오기(씬이름이 같아야함.)
    string GetSceneName(Define.Scene type)
    {
        string name = System.Enum.GetName(typeof(Define.Scene), type);
        return name;
    }

    public void Clear() { CurrentScene.Clear();}
}
