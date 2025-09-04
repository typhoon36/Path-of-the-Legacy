using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// Monster 위에 생성되는 Hp바 UI

public class UI_HpBar : UI_Base
{
     MonsterStat     m_Stat;

    enum GameObjects
    {
        HpBar
    }

    public override bool Init()
    {
        if (base.Init() == false) return false;

        Bind<GameObject>(typeof(GameObjects));

        m_Stat = transform.parent.GetComponent<MonsterStat>();
        gameObject.SetActive(false);

        return true;
    }

    void FixedUpdate()
    {
        // 체력 설정
        Transform a_Parent = transform.parent;
        transform.position = a_Parent.position + Vector3.up * (a_Parent.GetComponent<Collider>().bounds.size.y);
        GetObject((int)GameObjects.HpBar).transform.rotation = Camera.main.transform.rotation;

        float a_Ratio = (float)m_Stat.Hp / m_Stat.MaxHp;
        
        GetObject((int)GameObjects.HpBar).GetComponent<Slider>().value = a_Ratio;
    }
}
