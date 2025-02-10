using System.Collections;
using UnityEngine;
using static Define_S;


// 보스 몬스터
public class Necro_Ctrl : Monster_Ctrl
{
    [SerializeField] GameObject m_SkillRange;//스킬 범위
    [SerializeField] GameObject m_Trail;
    [SerializeField] GameObject[] m_SkillEff;//스킬시 켜줄 이펙트들
    [SerializeField] MonsterAttColl m_AttColl;

    string Skills = "Skill1";
    string[] Attacks = new string[] { "Attack1", "Attack2", "Attack3" };

    int AttackCnt = 0;
    int SkillCnt = 3;//공격이 3번 이상일때 스킬 사용

    bool IsSkill = false;

    Portal m_ExitPortal;

    public override void Init()
    {
        base.Init();
        // 데미지 스탯 적용
        m_AttColl.m_Damage = m_Stat.Attack;

        // 포탈 객체 찾아오기
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