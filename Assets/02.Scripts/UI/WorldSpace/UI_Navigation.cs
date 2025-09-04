using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 퀘스트 장소를 알려주는 네비게이션 UI

public class UI_Navigation : UI_Base
{
    Vector3 m_TargetPos;      // 목표 위치
    float m_EndScan = 7f;   // 목표 위치 스캔 거리

    public void SetInfo(Vector3 a_Pos)
    {
        m_TargetPos = a_Pos;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (Managers.Game.GetPlayer().IsNull() == true || gameObject.activeSelf == false) return;

        Vector3 a_Dir = m_TargetPos - Managers.Game.GetPlayer().transform.position;
        if (a_Dir.magnitude <= m_EndScan)
            gameObject.SetActive(false);

        transform.position = Managers.Game.GetPlayer().transform.position + (a_Dir.normalized * 2f);
    }
}
