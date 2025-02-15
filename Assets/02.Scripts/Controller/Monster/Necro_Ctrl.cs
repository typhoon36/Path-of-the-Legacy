using I18N.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 보스 몬스터
public class Necro_Ctrl : Monster_Ctrl
{
    [SerializeField] Transform m_RangeObj;// 스킬 범위 표시 오브젝트
    [SerializeField] MonsterAttColl m_AttColl; // 일반 공격 사용 접촉
    [SerializeField] MonsterSkillColl m_SkillColl;  // 스킬 사용 접촉
    [SerializeField] float m_SkillRange = 5f; // 공격 범위

    // 애니메이션 이름
    string[] Attacks = new string[] { "Attack1", "Attack2" };
    string skill = "Skill"; // 하나의 스킬만 사용

    // 횟수
    int m_AttCnt = 0;
    int m_SkillCnt = 3;

    // 체크
    bool IsSkill = false;


    Portal m_ExitPortal; // 포탈

    public override void Init()
    {
        base.Init();

        //데미지 적용
        m_AttColl.m_Damage = m_Stat.Attack;
        m_SkillColl.m_Damage = (int)(m_Stat.Attack * 1.2f);

        //나가는 포탈 찾기
        m_ExitPortal = GameObject.FindObjectOfType<Portal>();
        if (m_ExitPortal != null)
            m_ExitPortal.gameObject.SetActive(false);

        m_MonsterType = Define_S.MonsterType.Boss;
    }

    #region State
    protected override void IdletoDetective()
    {
        base.IdletoDetective();
    }

    protected override void Move()
    {
        m_Nav.SetDestination(m_Target.transform.position);

        m_Dist = TargetDist(m_Target);

        OnRotate();
        AttCheck();
    }

    // 공격/스킬 상태(State)일 때 애니메이션 움직임에 따르기
    protected override void Attack() { OnAnimMove(); }
    protected override void Skill() { OnAnimMove(); }

    protected override void Die()
    {
        base.Die();

        if (m_ExitPortal == null) return; // 포탈이 없으면 리턴

        if (m_ExitPortal.gameObject.activeSelf == false)
            // 포탈 활성화
            m_ExitPortal.gameObject.SetActive(true);
    }
    #endregion


    #region Animations
    Coroutine a_WeaponCo;
    protected override void OnAttEvent()
    {
        // 무기 콜라이더 활성화
        m_AttColl.IsCollider(true);

        // 무기 콜라이더 비활성화 코루틴 실행
        if (a_WeaponCo != null) StopCoroutine(WeaponColl());
        a_WeaponCo = StartCoroutine(WeaponColl());
    }

    protected override void ExitAttEvent()
    {
        // 2~3번 일반 공격 시 다음 공격 스킬 진행
        if (m_AttCnt++ >= Random.Range(2, m_SkillCnt + 1))
        {
            IsSkill = true;
            m_AttCnt = 0; 
        }

        // 무기 콜라이더 비활성화
        m_AttColl.IsCollider(false);
        State = Define_S.AllState.Idle;
    }
    #endregion

    // 다음 공격 확인
    void AttCheck()
    {
        // 스킬 공격이 가능하다면
        if (IsSkill == true)
        {
            if (m_Dist <= m_SkillRange + 1)
                SetSkill(skill);

            return;
        }
        else
        {
            if (m_Dist <= m_AttRange)
                SetAttack(Attacks[Random.Range(0, 2)]);
        }
    }

    // 공격 시작
    void SetAttack(string a_AttName)
    {
        // 공격 애니메이션 실행
        SetAnim(a_AttName);
        State = Define_S.AllState.Attack;
    }

    // 스킬 시작
    void SetSkill(string a_SkillName)
    {
        // 스킬 애니메이션 실행
        SetAnim(a_SkillName);

        // 스킬 코루틴 실행
        if (a_SkillName == "Skill")
        {
            StopCoroutine(Skill_Co());
            StartCoroutine(Skill_Co());
        }

        IsSkill = false;
        State = Define_S.AllState.Skill;
    }

    #region coroutines
    private IEnumerator Skill_Co()
    {
        // 공격 예상 범위 사이즈 및 위치 설정
        m_RangeObj.localPosition = new Vector3(0, 0.16f, -0.44f);
        m_RangeObj.localScale = new Vector3(1, 0.00055f, 4.66f);

        yield return new WaitForSeconds(1f);

        m_RangeObj.gameObject.SetActive(true);

        m_SkillColl.IsParticle(true);

        yield return new WaitForSeconds(0.8f);

        m_SkillColl.IsParticle(false);
        m_RangeObj.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.2f);

        State = Define_S.AllState.Idle;
    }
    // 무기 콜라이더 비활성화 코루틴
    IEnumerator WeaponColl()
    {
        // 0.15초 뒤 비활성화
        yield return new WaitForSeconds(0.15f);

        m_AttColl.IsCollider(false);
    }
    #endregion


    // 애니메이션 및 방향 설정
    void SetAnim(string a_AnimName)
    {
        // 플레이어와 거리값
        Vector3 a_Dir = m_Target.transform.position - transform.position;

        // Nav 도착 좌표 설정
        m_Nav.SetDestination(transform.position);
        transform.rotation = Quaternion.LookRotation(a_Dir);

        // 애니메이션 실행
        m_Anim.CrossFade(a_AnimName, 0.1f, -1, 0);
    }


    //자연스러운 회전
    void OnRotate()
    {
        //2D 벡터로 x,z를 알아낸다.
        Vector2 a_Forward = new Vector2(transform.position.z, transform.position.x);
        // 2D 벡터로 타겟(플레이어)의 x,z를 알아낸다.
        /// NavMeshAgent의 steeringTarget은 경로를 따라 이동할때 회전할 방향을 나타내기 위해 사용된다.
        Vector2 a_Target = new Vector2(m_Nav.steeringTarget.z, m_Nav.steeringTarget.x);

        // 타겟과 현재 위치의 차이값을 구하고, Atan2를 이용하여 각도를 구한다.
        Vector2 a_Dir = a_Target - a_Forward;
        float a_Angle = Mathf.Atan2(a_Dir.y, a_Dir.x) * Mathf.Rad2Deg;

        // 회전값을 설정한다.
        transform.eulerAngles = Vector3.up * a_Angle;
    }

    // 애니메이션 움직임으로 설정
    void OnAnimMove()
    {
        Vector3 a_Pos = m_Anim.targetPosition; // 애니메이션의 다음 위치
        a_Pos.y = m_Nav.nextPosition.y;        // Nav Y

        transform.position = a_Pos;
        m_Nav.SetDestination(a_Pos);
    }


}