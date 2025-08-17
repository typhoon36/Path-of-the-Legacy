using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//매니저 관리 
public class Managers : MonoBehaviour
{
    static Managers s_instance;
    static Managers Instance { get { Init(); return s_instance; } }


    Game_Mgr _game = new Game_Mgr();

    public static Game_Mgr Game { get { return Instance._game; } }



    DataManager _data = new DataManager();
    Input_Mgr _input = new Input_Mgr();
    PoolManager _pool = new PoolManager();
    Resource_Mgr _resource = new Resource_Mgr();
    Scene_Mgr _scene = new Scene_Mgr();
    UI_Mgr _ui = new UI_Mgr();

    public static DataManager Data { get { return Instance._data; } }
    public static Input_Mgr Input { get { return Instance._input; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static Resource_Mgr Resource { get { return Instance._resource; } }
    public static Scene_Mgr Scene { get { return Instance._scene; } }
    public static UI_Mgr UI { get { return Instance._ui; } }



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
                go = new GameObject { name = "@Manager" };
                go.AddComponent<Managers>();
                Debug.Log("@Manager 생성.");
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();

            s_instance._data.Init();
            s_instance._game.Init();
            s_instance._pool.Init();
        }
    }

    public static void Clear()
    {
        UI.Clear();
        Scene.Clear();
        Game.Clear();
    }
}
