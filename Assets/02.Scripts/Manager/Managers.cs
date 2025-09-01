using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   Managers.cs
 * Desc :   싱글톤 패턴을 사용하여 모든 매니저에 접근 가능
 *          [ Rookiss의 MMORPG Game Part 3 참고. ]
 */

public class Managers : MonoBehaviour
{
     static Managers s_instance;
     static Managers Instance { get { Init(); return s_instance; } }

#region Contents

     GameManager _game = new GameManager();

    public static GameManager Game { get { return Instance._game; } }

#endregion

#region Core

     DataManager _data = new DataManager();
     InputManager _input = new InputManager();
     PoolManager _pool = new PoolManager();
     ResourceManager _resource = new ResourceManager();
     SceneManagerEx _scene = new SceneManagerEx();
     UIManager _ui = new UIManager();

    public static DataManager Data { get { return Instance._data; } }
    public static InputManager Input { get { return Instance._input; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    public static UIManager UI { get { return Instance._ui; } }

#endregion

    void Start()
    {
        Application.targetFrameRate = 50;

        Init();
    }

    void Update()
    {
        Input.OnUpdate();
        Game.OnUpdate();
    }

    // 싱글톤 메소드
    static void Init()
    {
        if (s_instance.IsNull() == true)
        {
            GameObject go = GameObject.Find("@Manager");

            if (go.IsNull() == true)
            {
                go = new GameObject{name = "@Manager"};
                go.AddComponent<Managers>();
                Debug.Log("@Manager 생성.");
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();
            
            s_instance._data.Init();
            s_instance._game.Init();
            s_instance._pool.Init();
            // s_instance._data.Init();
        }
    }

    public static void Clear()
    {
        UI.Clear();
        Scene.Clear();
        Game.Clear();
    }
}
