using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;//NavMeshAgent 사용
using UnityEngine.UI;

public class Monster_Ctrl : Base_Ctrl
{
    public Define_S.MonsterType m_MonsterType; // 몬스터 타입
    public Vector3 m_SpawnPos;//몬스터 스폰 위치
    [Header("HUD")]
    public GameObject HPBack;
    public Image HPBar;
    public Text m_NickTxt;

    protected MonsterStat m_Stat;//몬스터 스텟
    NavMeshAgent m_Nav;

    protected float m_Dist;
    protected bool IsOver = false;

    [SerializeField] float m_ScanRange;//플레이어 감지 거리
    [SerializeField] float m_AttRange;//공격 사거리
    [SerializeField] public int m_SpawnRange = 16; //스폰 범위


    public override void Init()
    {
        m_MonsterType = Define_S.MonsterType.Normal;
        m_WObject = Define_S.W_Object.Monster;

        m_Stat = GetComponent<MonsterStat>();
        m_Anim = GetComponent<Animator>();
        m_Nav = GetComponent<NavMeshAgent>();

        State = Define_S.AllState.Idle;
        m_SpawnPos = this.transform.position;

        HPBar.fillAmount = 1;
        HPBack.gameObject.SetActive(false);
        HPBar.gameObject.SetActive(false);

        m_NickTxt.text = m_Stat.m_Name.ToString();

    }

    protected virtual void IdleDetective()
    {
        //HP 설정
        HPBar.fillAmount = (float)m_Stat.CurHp / m_Stat.MaxHp;
        HPBack.gameObject.SetActive(true);
        HPBar.gameObject.SetActive(true);

        //닉네임 설정
        m_NickTxt.text = m_Stat.m_Name.ToString();


        //타겟을 플레이어로 설정
        m_Target = GameObject.FindGameObjectWithTag("Player");

        State = Define_S.AllState.Moving;
    }

    protected override void Idle()
    {
        GameObject a_Player = GameObject.FindGameObjectWithTag("Player");
        //사망시 리턴
        if (a_Player == null) return;

        // 플레이어 거리 체크
        m_Dist = TargetDist(a_Player);

        // 플레이어가 스캔 범위 안에 들어오면 인식
        if (m_Dist <= m_ScanRange)
        {
            IdleDetective();
        }
        // 스캔 범위 밖에 있을 때
        else
        {
            m_Target = null;
            State = Define_S.AllState.Idle;
        }
    }

    protected override void Move()
    {
        //스폰거리에서 벗어나면 리턴
        if (IsOver == true) return;

        //플레이어가 죽거나 타겟이 null이면 
        if (m_Target == null || m_Target.GetComponent<Player_Ctrl>().State == Define_S.AllState.Die)
        {
            StartCoroutine(ReturnSpawn());
            return;
        }

        float a_SpawnDist = (m_SpawnPos - this.transform.position).magnitude;

        if (a_SpawnDist >= m_SpawnRange)
        {
            StartCoroutine(ReturnSpawn());
            return;
        }

        m_Dist = TargetDist(m_Target);
        HPBack.gameObject.SetActive(true);
        HPBar.gameObject.SetActive(true);
        m_NickTxt.text = m_Stat.m_Name.ToString();

        if (m_Dist > m_ScanRange)
        {
            StartCoroutine(ReturnSpawn());
            return;
        }

        //도착 좌표 설정
        m_Nav.SetDestination(m_Target.transform.position);

        //타겟이 사거리 안쪽에 존재하면
        if (m_Dist <= m_AttRange)
        {
            m_Nav.SetDestination(this.transform.position);
            State = Define_S.AllState.Attack;
        }
        else if(m_Dist > m_AttRange)
        {
           
            State = Define_S.AllState.Moving;
        }
    }

    protected override void Attack()
    {
        //플레이어가 죽었다면
        if (m_Target == null || m_Target.GetComponent<Player_Ctrl>().State == Define_S.AllState.Die)
        {
            StartCoroutine(ReturnSpawn());
            return;
        }

        //회전값 설정
        Vector3 a_Rot = m_Target.transform.position - this.transform.position;
        a_Rot.y = 0;
        transform.rotation = Quaternion.LookRotation(a_Rot);
    }

    protected override void Hit()
    {
        //멈추기 
        m_Nav.SetDestination(this.transform.position);

        // 피격 애니메이션 시간 체크
        if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Hit") &&
            m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
        {
            State = Define_S.AllState.Moving;
        }

    }

    protected override void Die()
    {
        //멈추기
        m_Nav.SetDestination(this.transform.position);

        State = Define_S.AllState.Die;

        //콜라이더가 있으면 삭제 진행
        if (GetComponent<Collider>() != null)
            StartCoroutine(DelayDestroy());

    }


    #region Animation Events
    //공격 이벤트
    protected virtual void OnAttEvent()
    {
        //거리값 
        m_Dist = TargetDist(m_Target);

        //공격 사거리 안에 있으면
        if (m_Dist <= m_AttRange)
        {
            //HP바 활성화
            HPBar.gameObject.SetActive(true);

            //플레이어 데미지 반영(몬스터 스탯에 따라 데미지 설정)
            MonsterStat a_Stat = GetComponent<MonsterStat>();
            int a_Dmg = Random.Range(a_Stat.Attack - 5, a_Stat.Attack + 5);
            m_Target.GetComponent<Player_Ctrl>().CurHp -= a_Dmg;
        }


    }

    /// [공격 이벤트 종료]
    protected virtual void ExitAttEvent()
    {
        State = Define_S.AllState.Moving;
    }

    //피격 이벤트
    void OnHitEvent()
    {
        if (m_Target == null)
            State = Define_S.AllState.Idle;


        if (m_Target != null)
        {
            int a_Dmg = Random.Range(5, 10);
            Player_Ctrl a_Player = m_Target.GetComponent<Player_Ctrl>();
            a_Player.CurHp -= a_Dmg;
            UI_Mgr.Inst.UpdateHPBar(a_Player.CurHp, a_Player.MaxHp);

            if (a_Player.CurHp > 0)
            {
                float a_Dist = (m_Target.transform.position - transform.position).magnitude;

                if (a_Dist <= m_AttRange)
                {
                    State = Define_S.AllState.Attack;
                }
                else
                {
                    State = Define_S.AllState.Moving;
                }
            }
            else
            {
                State = Define_S.AllState.Idle;
            }
        }
    }
    #endregion

    #region Courutines
    IEnumerator DelayDestroy()
    {
        //콜라이더 비활성화
        GetComponent<Collider>().enabled = false;

        yield return new WaitForSeconds(3f);

        //몬스터 삭제
        State = Define_S.AllState.Idle;
        Destroy(this.gameObject);

        //콜라이더 활성화
        GetComponent<Collider>().enabled = true;

        //체력 초기화
        m_Stat.CurHp = m_Stat.MaxHp;

    }

    //스폰 지점으로 이동
    IEnumerator ReturnSpawn()
    {
        IsOver = true;

        //전투 종료
        BattleEnd();

        //스폰 지점으로 이동
        m_Nav.SetDestination(m_SpawnPos);

        //스폰에 가까워지면 멈추기
        while (true)
        {
            if (Vector3.Distance(transform.position, m_SpawnPos) <= 0.7f)
            {
                m_Nav.SetDestination(this.transform.position);
                State = Define_S.AllState.Idle; // Idle 상태로 변경
                IsOver = false; // IsOver 상태 초기화
                break;
            }
            yield return null;
        }
    }
    #endregion

    //전투 종료
    public void BattleEnd()
    {
        m_Target = null;

        //다시 스폰지점으로 이동
        m_Nav.SetDestination(m_SpawnPos);
        //전투 종료시 HP바 비활성화
        HPBack.SetActive(false);
        HPBar.gameObject.SetActive(false);
    }

    protected float TargetDist(GameObject a_Target)
    {
        if (a_Target == null) return 0;
        return (a_Target.transform.position - this.transform.position).magnitude;
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("PlayerWeapon"))
        {
            m_Stat.OnAttacked(10);
        }
        else if (coll.CompareTag("PlayerSkill"))
        {
            m_State = Define_S.AllState.Hit;
            m_Stat.OnAttacked(20);
        }
    }

}
