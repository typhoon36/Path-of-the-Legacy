using System;
using UnityEngine;


namespace UnityStandardAssets.Utilityity
{
    public class FollowTarget : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset = new Vector3(0f, 7.5f, 0f);


         void LateUpdate()
        {
            transform.position = target.position + offset;
        }
    }
}
