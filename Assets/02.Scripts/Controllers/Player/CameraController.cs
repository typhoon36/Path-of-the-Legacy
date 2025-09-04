using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어를 따라다니는 카메라

public class CameraController : MonoBehaviour
{
    [SerializeField] Define.CameraMode m_Mode = Define.CameraMode.QuarterView;
    [SerializeField] Vector3 m_Delta; // 각도 조절
    [SerializeField] GameObject m_Player = null; //자동 할당

    RaycastHit hit;

    public void SetPlayer(GameObject a_Obj) { m_Player = a_Obj; }

    void LateUpdate() { QuarterViewUpdate(); }


    void QuarterViewUpdate()
    {
        if (m_Mode == Define.CameraMode.QuarterView)
        {
            if (m_Player.isValid() == false) return;

            // 플레이어가 오브젝트에 가려져있다면 가깝게 이동
            if (Physics.Raycast(m_Player.transform.position, m_Delta, out hit, m_Delta.magnitude, 1 << 10)) // 10 : Block
            {
                float a_Dist = (hit.point - m_Player.transform.position).magnitude * 0.8f;
                transform.position = (m_Player.transform.position + Vector3.up) + m_Delta.normalized * a_Dist;
            }
            else
            {
                transform.position = m_Player.transform.position + m_Delta;
                transform.LookAt(m_Player.transform);
            }
        }
    }
}
