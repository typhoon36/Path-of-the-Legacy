using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttColl : MonoBehaviour
{
    private int skillIndex = 0;     // ��ų �޺� ���ݷ� List index

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

        // ������ ��ų �����̶�� index �ʱ�ȭ 
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

                // ��ų ����
                int skillDamage = player.m_CurSkill.powerList[skillIndex] * (player.m_Att / 2);
                coll.GetComponent<MonsterStat>().OnAttacked(skillDamage);
            }
            else
                coll.GetComponent<MonsterStat>().OnAttacked(player.m_Att); // �⺻ ����
        }
    }

    // �⺻ �ݶ��̴� ������
    private void BasicColliderSize()
    {
        m_Collider.center = new Vector3(0, 0, 0.4f);
        m_Collider.radius = 1.2f;
        m_Collider.height = 2.4f;
    }
}
