using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttColl : MonoBehaviour
{
    public int m_Damage;

    [SerializeField]
    private BoxCollider m_Collider;

    public void IsCollider(bool isActive) { m_Collider.enabled = isActive; }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            Player_Ctrl player = coll.GetComponent<Player_Ctrl>();
            if (player != null)
            {
                player.CurHp -= m_Damage;
            }
        }
    }
}
