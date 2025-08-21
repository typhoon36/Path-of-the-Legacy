using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class Monster_Ctrl : Base_Ctrl
{
    public Define.MonsterType m_MonsterType;            // 몬스터 타입
    public Vector3 m_SpawnPos;               // 스폰 위치
    public GameObject m_HpBarUI;                // 체력바 UI

    protected MonsterStat m_Stat;                  // 몬스터 스탯
    protected NavMeshAgent m_Nav;

    protected float m_Dist;               // 타겟과의 사이 거리
    protected bool IsOverSpawn = false;    // 스폰거리에서 벗어났는지 체크

    [SerializeField] protected float m_ScanRange;         // 플레이어 감지 거리
    [SerializeField] protected float m_AttRange;       // 공격 사거리
    [SerializeField] protected float m_SpawnRange = 16;   // 스폰 사거리 Max 거리

    public override void Init()
    {
        m_MonsterType = Define.MonsterType.Normal;
        WorldObjectType = Define.WorldObject.Monster;

        m_Stat = GetComponent<MonsterStat>();
        m_Anim = GetComponent<Animator>();
        m_Nav = GetComponent<NavMeshAgent>();

        // 체력바 생성
        m_HpBarUI = Managers.UI.MakeWorldSpaceUI<UI_HpBar>(transform).gameObject;

        // 스폰 위치 설정
        m_SpawnPos = transform.position;
    }

    // Idle 상태에서 타겟 감지 시
    protected virtual void IdleTargetDetection()
    {
        m_HpBarUI.SetActive(true);                    // 체력바 활성화
        m_LockTarget = Managers.Game.GetPlayer();    // 타겟 설정

        State = Define.State.Moving;
    }

    protected override void UpdateIdle()
    {
        // 플레이어 사망 시 작동 X
        if (Managers.Game.GetPlayer().GetComponent<Player_Ctrl>().State == Define.State.Die)
            return;

        // 플레이어와 거리 체크
        m_Dist = TargetDistance(Managers.Game.GetPlayer());
        if (m_Dist <= m_ScanRange) IdleTargetDetection();
    }

    protected override void UpdateMoving()
    {
        // 스폰거리 초과 체크
        if (IsOverSpawn == true) return;

        // 플레이어가 죽었거나, 타겟이 Null이면
        if (Managers.Game.GetPlayer().GetComponent<Player_Ctrl>().State == Define.State.Die ||
            m_LockTarget.IsNull() == true)
        {
            StartCoroutine(SpawnMoving());  // 스폰 지점으로 이동
            return;
        }

        // 스폰 지점에서 일정 거리 벗어나면 스폰지점으로 이동
        float a_SpawnDist = (m_SpawnPos - transform.position).magnitude;
        if (a_SpawnDist >= m_SpawnRange)
        {
            StartCoroutine(SpawnMoving());  // 스폰 지점으로 이동
            return;
        }

        m_Dist = TargetDistance(m_LockTarget);         // 타겟 거리값
        Managers.Game.m_PlayScene.OnMonsterBar(m_Stat);   // Scene UI 몬스터 정보 활성화

        // 타겟과의 거리가 일정 범위 벗어나면
        if (m_Dist > m_ScanRange)
        {
            StartCoroutine(SpawnMoving());  // 스폰 지점으로 이동
            return;
        }

        // nav 도착좌표 설정
        m_Nav.SetDestination(m_LockTarget.transform.position);

        // 타겟이 공격사거리안에 들어오면
        if (m_Dist <= m_AttRange)
        {
            // 멈추고 공격 시작
            m_Nav.SetDestination(transform.position);
            State = Define.State.Attack;
        }
    }

    protected override void UpdateAttack()
    {
        // 플레이어가 죽었다면
        if (Managers.Game.GetPlayer().GetComponent<Player_Ctrl>().State == Define.State.Die)
        {
            State = Define.State.Moving;
            return;
        }

        // 회전값 설정
        Vector3 dir = Managers.Game.GetPlayer().transform.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    protected override void UpdateHit()
    {
        // 멈추기
        m_Nav.SetDestination(transform.position);

        // 피격 애니메이션 시간 체크
        if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Hit") &&
            m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
        {
            State = Define.State.Moving;
        }
    }

    protected override void UpdateDie()
    {
        // 멈추기
        m_Nav.SetDestination(transform.position);

        // 콜라이더가 Null이 아니라면 삭제 진행
        if (GetComponent<CapsuleCollider>().IsNull() == false)
            StartCoroutine(DelayDestroy());
    }

    // 공격할 때 (Animation Event)
    protected virtual void OnAttackEvent()
    {
        // 타겟 거리값
        m_Dist = TargetDistance(Managers.Game.GetPlayer());

        // 공격 사거리에 있으면
        if (m_Dist <= m_AttRange)
        {
            // Scene UI 몬스터 정보 활성화
            Managers.Game.m_PlayScene.OnMonsterBar(m_Stat);

            // 플레이어 데미지 반영
            Managers.Game.OnAttacked(m_Stat);
        }
    }

    // 공격이 끝날때 (Animation Event)
    protected virtual void ExitAttack()
    {
        State = Define.State.Moving;
    }

    // 타겟 거리값
    protected float TargetDistance(GameObject _target)
    {
        if (_target.IsNull() == true) return 0;
        return (_target.transform.position - transform.position).magnitude;
    }

    // 스폰 지점 이동 코루틴
    IEnumerator SpawnMoving()
    {
        IsOverSpawn = true;

        BattleClose();  // 전투 종료

        m_Nav.SetDestination(m_SpawnPos);   // 스폰 위치로

        // 스폰과 가까워지면 멈추기
        while (true)
        {
            float a_SpawnDist = (m_SpawnPos - transform.position).magnitude;
            if (a_SpawnDist <= 0.7f)
                break;

            yield return null;
        }

        State = Define.State.Idle;

        IsOverSpawn = false;
    }

    // 삭제 딜레이 코루틴
    IEnumerator DelayDestroy()
    {
        // 콜라이더 비활성화 ( 플레이어 감지 때문 )
        GetComponent<CapsuleCollider>().enabled = false;

        // Scene UI 몬스터 정보 삭제
        Managers.Game.m_PlayScene.CloseMonsterBar();

        yield return new WaitForSeconds(3f);

        // 몬스터 삭제 ( Pool )
        State = Define.State.Idle;
        Managers.Game.Despawn(this.gameObject);

        // 콜라이더 활성화
        GetComponent<CapsuleCollider>().enabled = true;

        // 체력 복구
        m_Stat.Hp = m_Stat.MaxHp;
    }

    // 전투 종료 
    public void BattleClose()
    {
        m_LockTarget = null;
        Managers.Game.m_PlayScene.CloseMonsterBar();

        m_Nav.SetDestination(transform.position);
        m_HpBarUI.SetActive(false);
    }
}
