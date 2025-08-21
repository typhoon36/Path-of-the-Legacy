using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UI_HpBar : UI_Base
{
     MonsterStat     m_Stat;

    enum GameObjects
    {
        HpBar
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<GameObject>(typeof(GameObjects));

        m_Stat = transform.parent.GetComponent<MonsterStat>();
        gameObject.SetActive(false);

        return true;
    }

    void FixedUpdate()
    {
        // 체력 설정
        Transform parent = transform.parent;
        transform.position = parent.position + Vector3.up * (parent.GetComponent<Collider>().bounds.size.y);
        GetObject((int)GameObjects.HpBar).transform.rotation = Camera.main.transform.rotation;

        //몬스터의 스텟상 HP 적용
        float ratio = (float)m_Stat.Hp / m_Stat.MaxHp;
        
        GetObject((int)GameObjects.HpBar).GetComponent<Slider>().value = ratio;
    }
}
