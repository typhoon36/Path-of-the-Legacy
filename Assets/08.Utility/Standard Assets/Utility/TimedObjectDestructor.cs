using System;
using UnityEngine;

namespace UnityStandardAssets.Utilityity
{
    public class TimedObjectDestructor : MonoBehaviour
    {
        [SerializeField]  float m_TimeOut = 1.0f;
        [SerializeField]  bool m_DetachChildren = false;


         void Awake()
        {
            Invoke("DestroyNow", m_TimeOut);
        }


         void DestroyNow()
        {
            if (m_DetachChildren)
            {
                transform.DetachChildren();
            }
            Destroy(gameObject);
        }
    }
}
