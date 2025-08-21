using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Camera_Ctrl : MonoBehaviour
{
    [SerializeField] Define.CameraMode _mode = Define.CameraMode.QuarterView;
    [SerializeField] Vector3 _delta;
    [SerializeField] GameObject m_Player = null;

    RaycastHit m_Hit;

    public void SetPlayer(GameObject a_Obj) { m_Player = a_Obj; }

    void LateUpdate() { ViewUpdate();}


 
    void ViewUpdate()
    {
        if (_mode == Define.CameraMode.QuarterView)
        {
            if (m_Player.isValid() == false) return;

            // 플레이어가 오브젝트에 가려져있다면 가깝게 이동
            if (Physics.Raycast(m_Player.transform.position, _delta, out m_Hit, _delta.magnitude, 1 << 10)) // 10 : Block
            {
                float dist = (m_Hit.point - m_Player.transform.position).magnitude * 0.8f;
                transform.position = (m_Player.transform.position + Vector3.up) + _delta.normalized * dist;
            }
            else
            {
                transform.position = m_Player.transform.position + _delta;
                transform.LookAt(m_Player.transform);
            }
        }
    }
}
