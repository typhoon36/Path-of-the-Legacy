using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Resource_Mgr
{
    private static Resource_Mgr Instance;
    public static Resource_Mgr Inst
    {
        get
        {
            if (Inst == null)
            {
                Instance = new Resource_Mgr();
            }
            return Inst;
        }
    }

    public GameObject Instatiate(GameObject obj, Transform parent = null)
    {
        if (obj == null)
        {
            Debug.LogError("객체가 존재하지 않습니다.");
            return null;
        }

        // 해당 original 프리팹을 parent의 자식 객체로 생성하기
        GameObject prefab = Object.Instantiate(obj, parent);
        if (prefab == null)
        {
            Debug.LogError("Failed to instantiate prefab.");
            return null;
        }
        prefab.name = obj.name;

        return prefab;
    }

    public GameObject Instatiate(string a_Path, Transform parent = null)
    {
        // original 프리팹 객체 읽어오기.
        GameObject original = Resources.Load<GameObject>($"Prefabs/{a_Path}");

        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {a_Path}");
            return null;
        }

        // 해당 original 프리팹을 parent의 자식 객체로 생성하기
        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;    // (Clone) 이름을 없애기 위한 코드

        return go;
    }
}
