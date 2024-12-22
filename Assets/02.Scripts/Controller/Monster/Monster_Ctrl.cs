using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;//NavMeshAgent 사용을 위해 추가
using UnityEngine.UI;

public class Monster_Ctrl : Base_Ctrl
{
    [SerializeField]
    float m_ScanRange = 10f;
    [SerializeField]
    float m_AttRange = 2f;
    [SerializeField]
    float m_Speed = 2f;
    [SerializeField]
    public int m_Att = 10; // 공격력 추가

    private NavMeshAgent m_NavAgent;
    private Vector3 m_SpawnPosition;

    public Image HPBar;

    public Define_S.MonsterType m_MonsterType = Define_S.MonsterType.Normal;

    public override void Init()
    {
        m_Anim = GetComponent<Animator>();
        m_NavAgent = GetComponent<NavMeshAgent>();
        m_WObject = Define_S.W_Object.Monster;
        State = Define_S.AllState.Idle;
        m_SpawnPosition = transform.position; // 스폰된 위치 저장
        HPBar.fillAmount = 1;
    }

    protected override void Idle()
    {
        GameObject a_Player = GameObject.FindGameObjectWithTag("Player");
        if (a_Player == null) return;

        float a_Dist = (a_Player.transform.position - transform.position).magnitude;

        transform.rotation = Quaternion.Euler(0, -180, 0); // 스폰된 위치로 돌아가면 회전 설정

        // 플레이어가 스캔범위 안에 들어오면 이동상태로 전환
        if (a_Dist < m_ScanRange)
        {
            m_Target = a_Player;
            State = Define_S.AllState.Moving;
        }
        else if (m_DPos != m_SpawnPosition)
        {
            m_Target = null;
            m_DPos = m_SpawnPosition; // 스폰된 장소로 복귀
            State = Define_S.AllState.Moving;
        }
    }

    protected override void Move()
    {
        if (m_Target != null)
        {
            m_DPos = m_Target.transform.position;
            float a_Dist = (m_Target.transform.position - transform.position).magnitude;

            // 플레이어가 공격범위 안에 들어오면 공격상태로 전환
            if (a_Dist <= m_AttRange)
            {
                m_NavAgent.SetDestination(transform.position);
                State = Define_S.AllState.Attack;
                AnimAtt();
                return;
            }

            // 플레이어가 스캔범위를 벗어나면 스폰 위치로 복귀
            if (a_Dist > m_ScanRange && a_Dist > m_AttRange)
            {
                m_Target = null;
                m_DPos = m_SpawnPosition;
                transform.rotation = Quaternion.Euler(0, -180, 0); // 스폰된 위치로 돌아가면 회전 설정
                State = Define_S.AllState.Moving;
            }
        }

        // 길찾기 이동
        Vector3 a_Dir = m_DPos - transform.position;
        if (a_Dir.magnitude < 0.1f)
        {
            State = Define_S.AllState.Idle;
            return;
        }
        else
        {
            m_NavAgent.SetDestination(m_DPos);
            m_NavAgent.speed = m_Speed;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(a_Dir), 20 * Time.deltaTime);
        }
    }

    protected override void Attack()
    {
        if (m_Target != null)
        {
            Vector3 a_Dist = m_Target.transform.position - transform.position;
            Quaternion a_Rot = Quaternion.LookRotation(a_Dist);
            transform.rotation = Quaternion.Slerp(transform.rotation, a_Rot, 20 * Time.deltaTime);
        }
    }

    void OnHitEvent()
    {
        if (m_Target != null)
        {
            int a_Damage = Random.Range(5, 10);
            Player_Ctrl a_Player = m_Target.GetComponent<Player_Ctrl>();
            a_Player.CurHp -= a_Damage;
            UI_Mgr.Inst.UpdateHPBar(a_Player.CurHp, a_Player.m_MaxHp);

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
        else
        {
            State = Define_S.AllState.Idle;
        }
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "PlayerWeapon")
        {
            Player_Ctrl player = coll.GetComponentInParent<Player_Ctrl>();
            if (player != null && player.State == Define_S.AllState.Attack) // 플레이어가 공격 상태일 때만
            {
                int damage = player.m_Att; // 플레이어의 공격력 가져오기

                MonsterStat monsterStat = GetComponent<MonsterStat>();

                //데미지 적용 및 죽음처리
                if (monsterStat != null)
                {
                    monsterStat.OnAttacked(damage); // 몬스터에게 데미지 적용
                    if (monsterStat.Hp <= 0)
                        MonGen_Mgr.Inst.OnMonsterDeath(gameObject); // 몬스터가 죽었을 때 호출
                    
                }
            }
        }
    }

    public void UpdateHPBar()
    {
        if (HPBar != null)
        {
            HPBar.fillAmount = (float)GetComponent<MonsterStat>().Hp / GetComponent<MonsterStat>().MaxHp;
        }
    }
}
