using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSkillColl : MonoBehaviour
{
    public int m_Damage;
    [SerializeField] ParticleSystem m_Particle;
    bool IsDamage = true;
    float m_DmgCool = 1.2f; // 1ÃÊ Äð´Ù¿î

    private void Start()
    {
        var coll = m_Particle.collision;
        coll.enabled = true;
        coll.type = ParticleSystemCollisionType.World;
        coll.mode = ParticleSystemCollisionMode.Collision3D;
        coll.sendCollisionMessages = true;
    }

    public void IsParticle(bool isActive) { m_Particle.gameObject.SetActive(isActive); }

    void OnParticleCollision(GameObject coll)
    {
        if (coll.CompareTag("Player") && IsDamage)
        {
            Player_Ctrl player = coll.GetComponent<Player_Ctrl>();
            if (player != null)
            {
                player.CurHp -= m_Damage;
                StartCoroutine(DamageCooldown());
            }
        }
    }

    private IEnumerator DamageCooldown()
    {
        IsDamage = false;
        yield return new WaitForSeconds(m_DmgCool);
        IsDamage = true;
    }
}
