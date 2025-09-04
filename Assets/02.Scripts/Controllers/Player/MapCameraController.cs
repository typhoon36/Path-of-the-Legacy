using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//미니 맵 카메라 제어
public class MapCameraController : MonoBehaviour
{
    [SerializeField] float m_Height;
    
    void FixedUpdate()
    {
        if (Managers.Game.GetPlayer().isValid() == false) return;

        // 플레이어 따라다니기
        transform.position = Managers.Game.GetPlayer().transform.position + (Vector3.up * m_Height);
    }
}
