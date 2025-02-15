using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam_Ctrl : MonoBehaviour
{
    #region 카메라
    GameObject m_Player;//플레이어
    Vector3 m_TargetPos;//플레이어 위치

    //카메라 회전값
    float m_RotH = 0;
    float m_RotV = 0;

    float m_DefRotH = 3;
    float m_DefRotV = 20.0f;

    //카메라 줌 및 거리들
    float ZoomSpeed = 1.0f;
    float minDist = 10.0f;
    float maxDist = 20.0f;
    float m_DefDist = 8f;
    float m_CurDist = 17;

    Quaternion m_BuffRot;
    Vector3 m_BasePos;
    Vector3 m_BuffPos;
    #endregion

    void Start()
    {
        if (m_Player == null) return;

        m_TargetPos = m_Player.transform.position;
        m_TargetPos.y += 2f;

        //카메라 위치 
        m_RotH = m_DefRotH;
        m_RotV = m_DefRotV;
        m_CurDist = m_DefDist;


        m_BuffRot = Quaternion.Euler(m_RotV, m_RotH, 0);
        m_BasePos.x = 20;
        m_BasePos.y = 0;
        m_BasePos.z = -m_CurDist;

        m_BuffPos = m_TargetPos + (m_BuffRot * m_BasePos);


        transform.position = m_BuffPos;
        transform.LookAt(m_TargetPos);

    }

    //카메라 초기화
    public void InitCam(GameObject a_Player)
    {
        m_Player = a_Player;
    }

    void LateUpdate()
    {
        if (m_Player == null) return;

        m_TargetPos = m_Player.transform.position;
        m_TargetPos.y += 1.4f;


        if (Input.GetAxis("Mouse ScrollWheel") < 0 && m_CurDist < maxDist)
        {
            m_CurDist += ZoomSpeed;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && m_CurDist > minDist)
        {
            m_CurDist -= ZoomSpeed;
        }

        m_BuffRot = Quaternion.Euler(m_RotV, m_RotH, 0);

        m_BasePos.x = 0;
        m_BasePos.y = 0;
        m_BasePos.z = -m_CurDist;

        m_BuffPos = m_TargetPos + (m_BuffRot * m_BasePos);

        transform.position = m_BuffPos;
        transform.LookAt(m_TargetPos);
        
    }

}
