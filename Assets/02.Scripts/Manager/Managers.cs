using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//매니저 통합 관리 
public class Managers : MonoBehaviour
{
    static Managers g_Inst;
    static Managers Inst { get { Init(); return g_Inst; } }

    //Content Manager
    Game_Mgr g_Game = new Game_Mgr();

    public static Game_Mgr Game { get { return Inst.g_Game; } }


    //Core Manager
    Data_Mgr g_Data = new Data_Mgr();
    Input_Mgr g_Input = new Input_Mgr();
    Pool_Mgr g_Pool = new Pool_Mgr();
    Resource_Mgr g_Resource = new Resource_Mgr();
    Scene_Mgr g_Scene = new Scene_Mgr();
    UI_Mgr g_Ui = new UI_Mgr();

    public static Data_Mgr Data { get { return Inst.g_Data; } }
    public static Input_Mgr Input { get { return Inst.g_Input; } }
    public static Pool_Mgr Pool { get { return Inst.g_Pool; } }
    public static Resource_Mgr Resource { get { return Inst.g_Resource; } }
    public static Scene_Mgr Scene { get { return Inst.g_Scene; } }
    public static UI_Mgr UI { get { return Inst.g_Ui; } }



    void Start()
    {
       
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
        if (g_Inst.IsNull() == true)
        {
            GameObject a_Obj = GameObject.Find("@Manager");

            if (a_Obj.IsNull() == true)
            {
                a_Obj = new GameObject { name = "@Manager" };
                a_Obj.AddComponent<Managers>();
                Debug.Log("@Manager 생성.");
            }

            DontDestroyOnLoad(a_Obj);
            g_Inst = a_Obj.GetComponent<Managers>();

            g_Inst.g_Data.Init();
            g_Inst.g_Game.Init();
            g_Inst.g_Pool.Init();
        }
    }

    public static void Clear()
    {
        UI.Clear();
        Scene.Clear();
        Game.Clear();
    }
}
