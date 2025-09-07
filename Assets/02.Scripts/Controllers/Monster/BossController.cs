using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//보스 몬스터의 모든 상태를 관리

public class BossController : MonsterController
{
    // 스킬, 공격 애니메이션 이름
    private string[] skills = new string[] { "Skill1", "Skill2" };
    private string[] meleeAttacks = new string[] { "Attack1", "Attack2" };

    private int attackCount = 0;        // 공격 횟수 ( 스킬을 사용하기 위함. )
    private int onSkillCount = 3;        // 스킬 시작 횟수 

    private bool isSkill = false;    // 다음 스킬 공격 확인

    private Portal exitPortal;                 // 포탈 Prefab

    [SerializeField]
    private EffectParticle particleCollider;           // 파티클 접촉 확인

    [SerializeField]
    private GameObject swingTrail;                 // 검기 Trail

    [SerializeField]
    private Transform attackRangeObj;             // 공격 예상 범위 오브젝트

    [SerializeField]
    private MonsterAttackCollistion skillCollider;      // 스킬 사용 접촉 확인

    [SerializeField]
    private MonsterAttackCollistion attackCollider;     // 일반 공격 사용 접촉 확인

    public override void Init()
    {
        base.Init();

        // 파티클 피격 설정
        particleCollider.SetInfo(() => { m_LockTarget.GetComponent<PlayerController>().OnHitDown(_stat, (int)(_stat.Attack * 0.8f)); });

        // 데미지 스탯 적용
        skillCollider.damage = (int)(_stat.Attack * 1.5f);
        attackCollider.damage = _stat.Attack;

        // 포탈 객체 찾아오기 ( 사망 시 활성화하기 위함 )
        exitPortal = GameObject.FindObjectOfType<Portal>();
        if (exitPortal.IsNull() == false)
            exitPortal.gameObject.SetActive(false);

        monsterType = Define.MonsterType.Boss;
    }

    // Idle 상태에서 타겟 감지
    protected override void IdleTargetDetection()
    {
   
        base.IdleTargetDetection();
    }

    protected override void UpdateMoving()
    {
        // Scene UI 몬스터 정보 활성화
        Managers.Game._playScene.OnMonsterBar(_stat);

        // 도착좌표 설정
        nav.SetDestination(m_LockTarget.transform.position);

        // 거리 체크
        distance = TargetDistance(m_LockTarget);

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
        if (exitPortal.IsNull() == true)
            return;

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
    public override void OnAttackEvent()
    {
        // 무기 콜라이더 활성화
        attackCollider.IsCollider(true);

        // 무기 콜라이더 비활성화 코루틴 실행
        if (weaponDisableCo.IsNull() == false) StopCoroutine(WeaponColliderDisable());
        weaponDisableCo = StartCoroutine(WeaponColliderDisable());
    }

    // 공격이 끝날때 (Animation Event)
    public override void ExitAttack()
    {
        // 2~3번 일반 공격 시 다음 공격 스킬 진행
        if (++attackCount >= Random.Range(2, onSkillCount + 1))
        {
            isSkill = true;
            attackCount = 0;
        }

        // 무기 콜라이더 비활성화
        attackCollider.IsCollider(false);
        State = Define.State.Idle;
    }

    // 다음 공격 확인
    private void AttackCheck()
    {
        // 스킬 공격이 가능하다면
        if (isSkill == true)
        {
            if (distance <= attackRange)
                OnSkill(skills[Random.Range(0, 2)]);

            return;
        }


        if (distance <= attackRange)
            OnAttack(meleeAttacks[Random.Range(0, 2)]);

    }

    // 공격 시작
    private void OnAttack(string attackName)
    {
        // 공격 애니메이션 실행
        SetAnimation(attackName);
        State = Define.State.Attack;
    }

    // 스킬 시작
    private void OnSkill(string skillName)
    {
        // 스킬 애니메이션 실행
        SetAnimation(skillName);

        // 찌르기 스킬
        if (skillName == skills[0])
        {
            StopCoroutine(Skill01_Prick());
            StartCoroutine(Skill01_Prick());
        }
        // 내려찍기 스킬
        else if (skillName == skills[1])
        {
            StopCoroutine(Skill02_WeaponDown());
            StartCoroutine(Skill02_WeaponDown());
        }

        isSkill = false;
        State = Define.State.Skill;
    }

    // 찌르기 스킬 코루틴
    private IEnumerator Skill01_Prick()
    {
        // 공격 예상 범위 사이즈 설정
        attackRangeObj.gameObject.SetActive(true);
        attackRangeObj.localPosition = new Vector3(0, 10, 2.33f);
        attackRangeObj.localScale = new Vector3(1, 0.00055f, 4.66f);

        yield return new WaitForSeconds(0.4f);  // 찌르기 준비

        skillCollider.IsCollider(true);         // 스킬 콜라이더 활성화
        particleCollider.gameObject.SetActive(true);  // 파티클 콜라이더 활성화

        yield return new WaitForSeconds(0.8f);  // 찌르기

        attackRangeObj.gameObject.SetActive(false);
        particleCollider.gameObject.SetActive(false); // 파티클 콜라이더 비활성화
        skillCollider.IsCollider(false);        // 스킬 콜라이더 비활성화

        yield return new WaitForSeconds(1.2f);  // 가만히 있기

        State = Define.State.Idle;
    }

    // 내려찍기 스킬 코루틴
    private IEnumerator Skill02_WeaponDown()
    {
        // 공격 예상 범위 사이즈 설정
        attackRangeObj.gameObject.SetActive(true);
        attackRangeObj.localPosition = new Vector3(0, 10, 4.5f);
        attackRangeObj.localScale = new Vector3(1, 0.00055f, 9f);

        // 0.9초 동안 플레이어를 바라본 후 공격
        float currentTime = 0f;
        while (true)
        {
            if (currentTime >= 0.9f) break;

            currentTime += Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(m_LockTarget.transform.position - transform.position);

            particleCollider.gameObject.SetActive(true);  // 파티클 콜라이더 활성화
            skillCollider.IsCollider(true);         // 스킬 콜라이더 활성화


            if (State != Define.State.Skill)
                yield break;

            particleCollider.gameObject.SetActive(false); // 파티클 콜라이더 비활성화
            skillCollider.IsCollider(false);        // 스킬 콜라이더 비활성화
            attackRangeObj.gameObject.SetActive(false);

            yield return null;
        }

        yield return new WaitForSeconds(2f);  // 가만히 있기

        State = Define.State.Idle;
    }

    // 무기 콜라이더 비활성화 코루틴
    private IEnumerator WeaponColliderDisable()
    {
        // 0.15초 뒤 비활성화
        yield return new WaitForSeconds(0.15f);

        attackCollider.IsCollider(false);
    }

    // 애니메이션 및 방향 설정
    private void SetAnimation(string animName)
    {
        // 플레이어와 거리값
        Vector3 distance = m_LockTarget.transform.position - transform.position;

        // Nav 도착 좌표 설정
        nav.SetDestination(transform.position);
        transform.rotation = Quaternion.LookRotation(distance);

        // 애니메이션 실행
        m_Anim.CrossFade(animName, 0.15f);
    }

    // 네비게이션 자연스러운 즉각 회전 (떨림 완화)
    private void OnRotation()
    {
        Vector2 forward = new Vector2(transform.position.z, transform.position.x);
        Vector2 steeringTarget = new Vector2(nav.steeringTarget.z, nav.steeringTarget.x);

        //방향을 구한 뒤, 역함수로 각을 구한다.
        Vector2 dir = steeringTarget - forward;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        //방향 적용
        transform.eulerAngles = Vector3.up * angle;
    }

    // 애니메이션 움직임으로 설정
    private void OnAnimationMove()
    {
        Vector3 rootPosition = m_Anim.targetPosition; // 애니메이션의 다음 위치
        rootPosition.y = nav.nextPosition.y;        // Nav Y

        // 현재 위치와 Nav 도착좌표 rootPosition으로 설정
        transform.position = rootPosition;
        nav.SetDestination(rootPosition);
    }

    // 검기 Animation Event
    private void OnTrail() { swingTrail.SetActive(true); }
    private void OffTrail() { swingTrail.SetActive(false); }

    // 보스는 변칙적인 공격이 있기 때문에 사용 x (이대로만 두기)
    protected override void AnimAttack() { }
}
