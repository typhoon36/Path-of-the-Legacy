using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 몬스터 공격 충돌 처리

public class MonsterAttackCollistion : MonoBehaviour
{
    public int          damage;

    [SerializeField] BoxCollider boxCollider;

    public void IsCollider(bool isActive) { boxCollider.enabled = isActive; }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == true)
            Managers.Game.OnAttacked(damage);
    }
}
