using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



// Name바 UI

public class UI_NameBar : UI_Base
{
    enum Gameobjects
    {
        Background,
    }

    enum Texts
    {
        NameText,
    }

    public string       m_NameText;

    public override bool Init()
    {
        if (base.Init() == false) return false;

        BindObject(typeof(Gameobjects));
        BindText(typeof(Texts));

        GetText((int)Texts.NameText).text = m_NameText;

        return true;
    }

    void FixedUpdate()
    {
        Transform a_Parent = transform.parent;
        float a_ValueY = (a_Parent.GetComponent<Collider>().bounds.size.y * 1.3f);

        transform.position = a_Parent.position + Vector3.up * a_ValueY;
        GetObject((int)Gameobjects.Background).transform.rotation = Camera.main.transform.rotation;
    }
}
