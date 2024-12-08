using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;//NavMeshAgent 사용을 위해 추가

public class Monster_Ctrl : Base_Ctrl
{
    [SerializeField]
    float m_ScanRange = 10f;
    [SerializeField]
    float m_AttRange = 2f;
    [SerializeField]
    float m_Speed = 2f;

    private NavMeshAgent m_NavAgent;
    private Vector3 m_SpawnPosition;

    public override void Init()
    {
        m_Anim = GetComponent<Animator>();
        m_NavAgent = GetComponent<NavMeshAgent>();
        m_WObject = Define_S.W_Object.Monster;
        State = Define_S.AllState.Idle;
        m_SpawnPosition = transform.position; // 스폰된 위치 저장
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
                return;
            }

            // 플레이어가 스캔범위를 벗어나면 스폰 위치로 복귀
            if (a_Dist > m_ScanRange)
            {
                m_Target = null;
                m_DPos = m_SpawnPosition;
                transform.rotation = Quaternion.Euler(0, -180, 0); // 스폰된 위치로 돌아가면 회전 설정
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
            float a_Dist = (m_Target.transform.position - transform.position).magnitude;
            Quaternion a_Rot = Quaternion.LookRotation(m_Target.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, a_Rot, 20 * Time.deltaTime);
            if (a_Dist > m_AttRange)
            {
                State = Define_S.AllState.Idle;
                return;
            }
        }
    }
}
