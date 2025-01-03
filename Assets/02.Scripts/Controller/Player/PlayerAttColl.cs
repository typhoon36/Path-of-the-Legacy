using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttColl : MonoBehaviour
{
    private int skillIndex = 0;     // 스킬 콤보 공격력 List index

    private CapsuleCollider m_Collider;

    [SerializeField]
    private Player_Ctrl player;

    void Start()
    {
        m_Collider = GetComponent<CapsuleCollider>();

        skillIndex = 0;

        gameObject.SetActive(true);
    }

    void OnDisable()
    {
        BasicColliderSize();

        // 마지막 스킬 공격이라면 index 초기화 
        if (player.m_CurSkill != null)
        {
            if (skillIndex == player.m_CurSkill.powerList.Count - 1)
                skillIndex = 0;
            else
                skillIndex++;
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Monster"))
        {
            if (player.State == Define_S.AllState.Skill)
            {
                if (player.m_CurSkill.powerList.Contains(skillIndex) == false)
                    skillIndex = 0;

                // 스킬 공격
                int skillDamage = player.m_CurSkill.powerList[skillIndex] * (player.m_Att / 2);
                coll.GetComponent<MonsterStat>().OnAttacked(skillDamage);
            }
            else
                coll.GetComponent<MonsterStat>().OnAttacked(player.m_Att); // 기본 공격
        }
    }

    // 기본 콜라이더 사이즈
    private void BasicColliderSize()
    {
        m_Collider.center = new Vector3(0, 0, 0.4f);
        m_Collider.radius = 1.2f;
        m_Collider.height = 2.4f;
    }
}
