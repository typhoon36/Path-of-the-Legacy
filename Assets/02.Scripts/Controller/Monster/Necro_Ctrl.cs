using System.Collections;
using UnityEngine;
using static Define_S;


// ���� ����
public class Necro_Ctrl : Monster_Ctrl
{
    [SerializeField] GameObject m_SkillRange;//��ų ����
    [SerializeField] GameObject m_Trail;
    [SerializeField] GameObject[] m_SkillEff;//��ų�� ���� ����Ʈ��
    [SerializeField] MonsterAttColl m_AttColl;

    string Skills = "Skill1";
    string[] Attacks = new string[] { "Attack1", "Attack2", "Attack3" };

    int AttackCnt = 0;
    int SkillCnt = 3;//������ 3�� �̻��϶� ��ų ���

    bool IsSkill = false;

    Portal m_ExitPortal;

    public override void Init()
    {
        base.Init();
        // ������ ���� ����
        m_AttColl.m_Damage = m_Stat.Attack;

        // ��Ż ��ü ã�ƿ���
        m_ExitPortal = GameObject.FindObjectOfType<Portal>();
        if (m_ExitPortal != null)
            m_ExitPortal.gameObject.SetActive(false);

        m_MonsterType = Define_S.MonsterType.Boss;
    }

    protected override void IdletoDetective()
    {
        base.IdletoDetective();
    }

    protected override void Move()
    {
        m_Nav.SetDestination(m_Target.transform.position);

        m_Dist = TargetDist(m_Target);


    }

    protected override void Attack() { }

    protected override void Skill() { }

    protected override void Die()
    {
        base.Die();

    }

    Coroutine WeaponCo;

}