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
            Debug.LogError("��ü�� �������� �ʽ��ϴ�.");
            return null;
        }

        // �ش� original �������� parent�� �ڽ� ��ü�� �����ϱ�
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
        // original ������ ��ü �о����.
        GameObject original = Resources.Load<GameObject>($"Prefabs/{a_Path}");

        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {a_Path}");
            return null;
        }

        // �ش� original �������� parent�� �ڽ� ��ü�� �����ϱ�
        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;    // (Clone) �̸��� ���ֱ� ���� �ڵ�

        return go;
    }
}
