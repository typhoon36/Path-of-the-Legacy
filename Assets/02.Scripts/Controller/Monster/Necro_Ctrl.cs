using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//보스 몬스터 
public class Necro_Ctrl : Monster_Ctrl
{
    [SerializeField] float m_ArrowRange = 5f; //원거리 공격 범위

    [SerializeField] GameObject m_ScytheTrail; //무기(대낫)트레일

    [SerializeField] Transform m_RangeObj; //스킬 범위(범위를 통해 플레이어가 공격을 피할 수 있도록)

    [SerializeField] EffParticles m_ParticleColl;//피격 이펙트

    [SerializeField] MonsterAttColl m_AttColl; //무기 콜라이더
    [SerializeField] MonsterAttColl m_SkillColl;//스킬 콜라이더


    // 스킬, 공격 애니메이션 이름(보스 전용 스킬,공격 애니메이션)
    string[] m_Skills = new string[] { "Skill1", "Skill2" };
    string[] m_MeleeAtt = new string[] { "Attack1" };
    string[] m_RangeAtt = new string[] { "Attack2" };

    Portal m_ExitPortal; //나가는 포탈

    int m_AttCnt = 0;
    int m_SkillCnt = 3;

    bool IsRAtt = false;//원거리 공격중인지
    bool IsSkill = false;//스킬중인지



    public override void Init()
    {
        base.Init();  //부모인 Monster_Ctrl의 Init() 호출

        //파티클 피격 설정
        m_ParticleColl.SetInfo(() => { m_Target.GetComponent<Player_Ctrl>().OnHit(m_Stat, (int)(m_Stat.Attack * 0.8f)); });

        //데미지 스텟 설정
        m_SkillColl.m_Damage = (int)(m_Stat.Attack * 1.5f);
        m_AttColl.m_Damage = m_Stat.Attack;

        //나가는 포탈 설정
        m_ExitPortal = GameObject.FindObjectOfType<Portal>();

        if (m_ExitPortal != null) m_ExitPortal.gameObject.SetActive(false);

        //타입 설정
        m_MonsterType = Define_S.MonsterType.Boss;

    }

    #region State
    protected override void IdletoDetective()
    {
        //삼항 연산으로 40% 확률로 true, 60% 확률로 false로 해서 공격
        IsRAtt = Random.Range(0, 10) < 4 ? true : false;

        base.IdletoDetective(); //부모인 Monster_Ctrl의 IdletoDetective() 호출
    }

    protected override void Move()
    {
        //도착 설정
        m_Nav.SetDestination(m_Target.transform.position);

        //Monster_Ctrl의 TargetDist()를 이용해 타겟과의 거리 설정
        m_Dist = TargetDist(m_Target);

        //공격했는지 확인
        AttackCheck();
    }

    protected override void Attack() { OnAnimMove(); }

    //스킬 사용
    protected override void Skill() { OnAnimMove(); }

    protected override void Die()
    {
        base.Die();

        if (m_ExitPortal != null) return;

        //나가는 포탈 활성화
        if (m_ExitPortal == null) m_ExitPortal.gameObject.SetActive(true);
    }
    #endregion


    /* 공격 이벤트 */
    Coroutine m_DisableCo;
    protected override void OnAttEvent()
    {
        m_AttColl.IsCollider(true);

        if (m_DisableCo != null) StopCoroutine(m_DisableCo);

        m_DisableCo = StartCoroutine(DisableColl());

    }

    protected override void ExitAttEvent()
    {
        //공격 종료
        if (m_AttCnt++ >= Random.Range(2, m_SkillCnt + 1))
        {
            IsSkill = true; m_AttCnt = 0;
        }

        //종료했다면 콜리더 비활성화
        m_AttColl.IsCollider(false);
        //Idle상태 
        State = Define_S.AllState.Idle;
    }

    //공격 확인
    void AttackCheck()
    {
        if (IsSkill == true)
        {
            if (m_Dist <= m_ArrowRange + 1)
                OnSkill(m_Skills[Random.Range(0, 2)]);

            return;

        }

        if (IsRAtt == true)
            if (m_Dist <= m_ArrowRange)
                OnAttack(m_RangeAtt[Random.Range(0, 2)]);

            else
            if (m_Dist <= m_AttRange)
                OnAttack(m_MeleeAtt[Random.Range(0, 2)]);


    }

    //공격 시작
    void OnAttack(string a_AttName)
    {
        SetAnim(a_AttName);
        State = Define_S.AllState.Attack;
    }

    void OnSkill(string a_SkillName)
    {
        SetAnim(a_SkillName);

        if (a_SkillName == m_Skills[0])
        {
            StopCoroutine(Skill1());
            StartCoroutine(Skill1());
        }
        else if (a_SkillName == m_Skills[1])
        {
            StopCoroutine(Skill2());
            StartCoroutine(Skill2());
        }

        IsSkill = false;
        State = Define_S.AllState.Skill;
    }

    //애니메이션(방향설정) 
    void SetAnim(string a_AnimName)
    {
        Vector3 a_Dist = m_Target.transform.position - transform.position;

        m_Nav.SetDestination(transform.position);
        transform.rotation = Quaternion.LookRotation(a_Dist);

        //애니메이션 실행
        m_Anim.CrossFade(a_AnimName, 0.1f, -1);
    }

    void OnAnimMove()
    {
        Vector3 a_RtPos = m_Anim.targetPosition; //targetPosition은 애니메이션에서 설정한 위치
        a_RtPos.y = m_Nav.nextPosition.y;

        transform.position = a_RtPos;
        m_Nav.SetDestination(a_RtPos);
    }

    #region Corutines
    IEnumerator DisableColl()
    {
        yield return new WaitForSeconds(0.15f);
        m_AttColl.IsCollider(false);
    }
    IEnumerator Skill1() //횡베기 스킬
    {
        //공격 예상 위치 설정
        m_RangeObj.localPosition = new Vector3(0, 0, 4.5f);
        m_RangeObj.localScale = new Vector3(1, 1, 1);

        //1초후 공격
        float a_Time = 0f;
        while (true)
        {
            if (a_Time >= 1f) break;

            a_Time += Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(m_Target.transform.position - transform.position);

            yield return null;
        }

        yield return new WaitForSeconds(2);

        State = Define_S.AllState.Idle;
    }

    IEnumerator Skill2()
    {
        //공격 예상 위치 설정
        m_RangeObj.localPosition = new Vector3(0, 0, 4.5f);
        m_RangeObj.localScale = new Vector3(1, 1, 1);

        //1초후 공격
        float a_Time = 0f;
        while (true)
        {
            if (a_Time >= 1f) break;

            a_Time += Time.deltaTime;

            transform.rotation = Quaternion.LookRotation(m_Target.transform.position - transform.position);

            yield return null;
        }

        yield return new WaitForSeconds(2);

        State = Define_S.AllState.Idle;
    }
    #endregion


    /*트레일 관련 */
    void OnTrail() { m_ScytheTrail.SetActive(true); }

    void OffTrail() { m_ScytheTrail.SetActive(false); }

}
