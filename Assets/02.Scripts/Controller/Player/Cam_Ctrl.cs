using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam_Ctrl : MonoBehaviour
{
    #region ī�޶�
    GameObject m_Player;//�÷��̾�
    Vector3 m_TargetPos;//�÷��̾� ��ġ

    //ī�޶� ȸ����
    float m_RotH = 0;
    float m_RotV = 0;

    float m_DefRotH = 3;
    float m_DefRotV = 15.0f;

    //ī�޶� �� �� �Ÿ���
    float ZoomSpeed = 1.0f;
    float minDist = 3.0f;
    float maxDist = 10.0f;
    float m_DefDist = 5.2f;
    float m_CurDist = 17;

    Quaternion m_BuffRot;
    Vector3 m_BasePos;
    Vector3 m_BuffPos;
    #endregion

    void Start()
    {
        if (m_Player == null) return;

        m_TargetPos = m_Player.transform.position;
        m_TargetPos.y += 1.4f;

        //ī�޶� ��ġ 
        m_RotH = m_DefRotH;
        m_RotV = m_DefRotV;
        m_CurDist = m_DefDist;


        m_BuffRot = Quaternion.Euler(m_RotV, m_RotH, 0);
        m_BasePos.x = 0;
        m_BasePos.y = 0;
        m_BasePos.z = -m_CurDist;

        m_BuffPos = m_TargetPos + (m_BuffRot * m_BasePos);


        transform.position = m_BuffPos;
        transform.LookAt(m_TargetPos);

    }

    //ī�޶� �ʱ�ȭ
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

    float ClampAngle(float a_Angle, float VMin, float VMax)
    {
        if (a_Angle < -360)
            a_Angle += 360;

        if (a_Angle > 360)
            a_Angle -= 360;

        return Mathf.Clamp(a_Angle, VMin, VMax);

    }


}
