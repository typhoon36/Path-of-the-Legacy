using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//데미지 반영(기본 공격, 스킬 공격)
public class PlayerAttackCollistion : MonoBehaviour
{
    int skillIndex = 0;     // 스킬 콤보 공격력 List index

    CapsuleCollider capsuleCollider;

    [SerializeField] PlayerController m_player;

    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();

        skillIndex = 0;

        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        Invoke("DelayActiveFalse", 0.1f);
    }

    void OnDisable()
    {
        BasicColliderSize();

        // 마지막 스킬 공격이라면 index 초기화 
        if (m_player.currentSkill.IsNull() == false)
        {
            if (skillIndex == m_player.currentSkill.powerList.Count - 1)
                skillIndex = 0;
            else
                skillIndex++;
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Monster"))
        {
            if (m_player.State == Define.State.Skill)
            {
                if (m_player.currentSkill.powerList.Contains(skillIndex) == false)
                    skillIndex = 0;

                // 스킬 공격
                int a_SkillDamage = m_player.currentSkill.powerList[skillIndex] * (Managers.Game.Attack / 2);
                coll.GetComponent<MonsterStat>().OnAttacked(a_SkillDamage);
            }
            //일반 공격
            else
            {
                coll.GetComponent<MonsterStat>().OnAttacked(); // 기본 공격
                Debug.Log("PlayerAttackCollistion - 기본 공격");
            }
        }
    }

    // Invoke 호출
    void DelayActiveFalse() { gameObject.SetActive(false); }

    // 기본 콜라이더 사이즈
    void BasicColliderSize()
    {

        if (capsuleCollider.IsNull() == true) return;

        capsuleCollider.center = new Vector3(0, 1, 0.4f);
        capsuleCollider.radius = 1.6f;
        capsuleCollider.height = 2.4f;
    }
}
