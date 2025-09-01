using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//모든 이펙트 공통 부모
public class Effect : MonoBehaviour
{
    void OnEnable() { GetComponent<ParticleSystem>().Play(); } //켜질때 재생
    void OnDisable() { GetComponent<ParticleSystem>().Stop(); } //꺼질때 정지
}
