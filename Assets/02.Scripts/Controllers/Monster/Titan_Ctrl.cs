using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Titan_Ctrl : Monster_Ctrl
{
    // 스킬, 공격 애니메이션 이름
    string[] m_Skills = new string[] { "Skill1", "Skill2" };
    string[] m_MeleeAttacks = new string[] { "Attack1", "Attack2" };
    string[] m_RangedAttacks = new string[] { "Attack3", "Attack4", "Attack5" };

    int m_AttackCount = 0;        // 공격 횟수 ( 스킬을 사용하기 위함. )
    int m_SkillCount = 3;        // 스킬 시작 횟수 

    bool IsRangedAttack = false;    // 원거리 공격 체크
    bool IsSkill = false;    // 다음 스킬 공격 확인

    [SerializeField] float rangedAttackRange = 5f;     // 원거리 수치

    Portal exitPortal;                 // 포탈 Prefab

    [SerializeField] EffectParticle particleCollider;           // 파티클 접촉 확인

    [SerializeField] GameObject swingTrail;                 // 검기 Trail

    [SerializeField] Transform attackRangeObj;             // 공격 예상 범위 오브젝트

    [SerializeField] MonsterAttackCollistion skillCollider;      // 스킬 사용 접촉 확인

    [SerializeField] MonsterAttackCollistion attackCollider;     // 일반 공격 사용 접촉 확인

    public override void Init()
    {
        base.Init();

        // 파티클 피격 설정
        particleCollider.SetInfo(() => { m_LockTarget.GetComponent<Player_Ctrl>().OnHitDown(m_Stat, (int)(m_Stat.Attack * 0.8f)); });

        // 데미지 스탯 적용
        skillCollider.damage = (int)(m_Stat.Attack * 1.5f);
        attackCollider.damage = m_Stat.Attack;

        // 포탈 객체 찾아오기 ( 사망 시 활성화하기 위함 )
        exitPortal = GameObject.FindObjectOfType<Portal>();
        if (exitPortal.IsNull() == false)
            exitPortal.gameObject.SetActive(false);

        m_MonsterType = Define.MonsterType.Boss;
    }

    // Idle 상태에서 타겟 감지
    protected override void IdleTargetDetection()
    {
        // 50% 확률로 원거리, 근거리 공격 결정
        IsRangedAttack = Random.Range(0, 2) == 0;

        base.IdleTargetDetection();
    }

    protected override void UpdateMoving()
    {
        // Scene UI 몬스터 정보 활성화
        Managers.Game.m_PlayScene.OnMonsterBar(m_Stat);

        // 도착좌표 설정
        m_Nav.SetDestination(m_LockTarget.transform.position);

        // 거리 체크
        m_Dist = TargetDistance(m_LockTarget);

        OnRotation();   // 회전
        AttackCheck();  // 거리에 따른 공격 체크
    }

    // 공격/스킬 상태(State)일 때 애니메이션 움직임에 따르기
    protected override void UpdateAttack() { OnAnimationMove(); }
    protected override void UpdateSkill() { OnAnimationMove(); }

    protected override void UpdateDie()
    {
        base.UpdateDie();

        // 포탈이 활성화되면 Return
        if (exitPortal.IsNull() == true) return;

        // 포탈이 비활성화라면 True
        if (exitPortal.gameObject.activeSelf == false)
        {
            // 클리어 안내문 생성
            string message = $"<size=170%>Clear!!</size> \n<color=yellow>Gold: 100</color> <color=green>Exp: 200</color>";
            Managers.UI.MakeSubItem<UI_Guide>().SetInfo(message, new Color(1f, 0.5f, 0f));

            // 포탈 활성화
            exitPortal.gameObject.SetActive(true);
        }
    }

    // 공격할 때 (Animation Event)
    Coroutine weaponDisableCo;
    protected override void OnAttackEvent()
    {
        // 무기 콜라이더 활성화
        attackCollider.IsCollider(true);

        // 무기 콜라이더 비활성화 코루틴 실행
        if (weaponDisableCo.IsNull() == false) StopCoroutine(WeaponColliderDisable());
        weaponDisableCo = StartCoroutine(WeaponColliderDisable());
    }

    // 공격이 끝날때 (Animation Event)
    protected override void ExitAttack()
    {
        // 2~3번 일반 공격 시 다음 공격 스킬 진행
        if (++m_AttackCount >= Random.Range(2, m_SkillCount + 1))
        {
            IsSkill = true;
            m_AttackCount = 0;
        }

        // 무기 콜라이더 비활성화
        attackCollider.IsCollider(false);
        State = Define.State.Idle;
    }

    // 다음 공격 확인
    void AttackCheck()
    {
        // 스킬 공격이 가능하다면
        if (IsSkill == true)
        {
            if (m_Dist <= rangedAttackRange + 1)
                OnSkill(m_Skills[Random.Range(0, 2)]);

            return;
        }

        // 원거리에서 공격 시작할지
        if (IsRangedAttack == true)
        {
            if (m_Dist <= rangedAttackRange)
                OnAttack(m_RangedAttacks[Random.Range(0, 3)]);
        }
        else
        {
            if (m_Dist <= m_AttRange)
                OnAttack(m_MeleeAttacks[Random.Range(0, 2)]);
        }
    }

    // 공격 시작
    void OnAttack(string attackName)
    {
        // 공격 애니메이션 실행
        SetAnimation(attackName);
        State = Define.State.Attack;
    }

    // 스킬 시작
    void OnSkill(string skillName)
    {
        // 스킬 애니메이션 실행
        SetAnimation(skillName);

        // 찌르기 스킬
        if (skillName == m_Skills[0])
        {
            StopCoroutine(Skill01_Prick());
            StartCoroutine(Skill01_Prick());
        }
        // 내려찍기 스킬
        else if (skillName == m_Skills[1])
        {
            StopCoroutine(Skill02_WeaponDown());
            StartCoroutine(Skill02_WeaponDown());
        }

        IsSkill = false;
        State = Define.State.Skill;
    }

    // 찌르기 스킬 코루틴
    IEnumerator Skill01_Prick()
    {
        // 공격 예상 범위 사이즈 설정
        attackRangeObj.localPosition = new Vector3(0, 0, 2.33f);
        attackRangeObj.localScale = new Vector3(1, 0.00055f, 4.66f);

        yield return new WaitForSeconds(0.4f);  // 찌르기 준비

        skillCollider.IsCollider(true);         // 스킬 콜라이더 활성화

        yield return new WaitForSeconds(0.8f);  // 찌르기

        skillCollider.IsCollider(false);        // 스킬 콜라이더 비활성화

        yield return new WaitForSeconds(1.2f);  // 가만히 있기

        State = Define.State.Idle;
    }

    // 내려찍기 스킬 코루틴
    IEnumerator Skill02_WeaponDown()
    {
        // 공격 예상 범위 사이즈 설정
        attackRangeObj.localPosition = new Vector3(0, 0, 4.5f);
        attackRangeObj.localScale = new Vector3(1, 0.00055f, 9f);

        // 0.9초 동안 플레이어를 바라본 후 공격
        float currentTime = 0f;
        while (true)
        {
            if (currentTime >= 0.9f)
                break;

            currentTime += Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(m_LockTarget.transform.position - transform.position);

            yield return null;
        }

        yield return new WaitForSeconds(2f);  // 가만히 있기

        State = Define.State.Idle;
    }

    // 무기 콜라이더 비활성화 코루틴
    IEnumerator WeaponColliderDisable()
    {
        // 0.15초 뒤 비활성화
        yield return new WaitForSeconds(0.15f);

        attackCollider.IsCollider(false);
    }

    // 애니메이션 및 방향 설정
    void SetAnimation(string animName)
    {
        // 플레이어와 거리값
        Vector3 distance = m_LockTarget.transform.position - transform.position;

        // Nav 도착 좌표 설정
        m_Nav.SetDestination(transform.position);
        transform.rotation = Quaternion.LookRotation(distance);

        // 애니메이션 실행
        m_Anim.CrossFade(animName, 0.1f, -1, 0);
    }

    // 네비게이션 자연스러운 즉각 회전 (떨림 완화)
    void OnRotation()
    {
        Vector2 forward = new Vector2(transform.position.z, transform.position.x);
        Vector2 steeringTarget = new Vector2(m_Nav.steeringTarget.z, m_Nav.steeringTarget.x);

        //방향을 구한 뒤, 역함수로 각을 구한다.
        Vector2 dir = steeringTarget - forward;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        //방향 적용
        transform.eulerAngles = Vector3.up * angle;
    }

    // 애니메이션 움직임으로 설정
    void OnAnimationMove()
    {
        Vector3 rootPosition = m_Anim.targetPosition; // 애니메이션의 다음 위치
        rootPosition.y = m_Nav.nextPosition.y;        // Nav Y

        // 현재 위치와 Nav 도착좌표 rootPosition으로 설정
        transform.position = rootPosition;
        m_Nav.SetDestination(rootPosition);
    }

    // 검기 Animation Event
    void OnTrail() { swingTrail.SetActive(true); }
    void OffTrail() { swingTrail.SetActive(false); }

    // 보스는 변칙적인 공격이 있기 때문에 사용 x (이대로만 두기)
    protected override void AnimAttack() { }
}
