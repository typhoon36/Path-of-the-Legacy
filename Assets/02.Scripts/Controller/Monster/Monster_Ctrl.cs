using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;//NavMeshAgent ����� ���� �߰�

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
        m_SpawnPosition = transform.position; // ������ ��ġ ����
    }

    protected override void Idle()
    {
        GameObject a_Player = GameObject.FindGameObjectWithTag("Player");
        if (a_Player == null) return;

        float a_Dist = (a_Player.transform.position - transform.position).magnitude;

        transform.rotation = Quaternion.Euler(0, -180, 0); // ������ ��ġ�� ���ư��� ȸ�� ����

        // �÷��̾ ��ĵ���� �ȿ� ������ �̵����·� ��ȯ
        if (a_Dist < m_ScanRange)
        {
            m_Target = a_Player;
            State = Define_S.AllState.Moving;
        }
        else if (m_DPos != m_SpawnPosition)
        {
            m_Target = null;
            m_DPos = m_SpawnPosition; // ������ ��ҷ� ����
            State = Define_S.AllState.Moving;
        }
    }

    protected override void Move()
    {
        if (m_Target != null)
        {
            m_DPos = m_Target.transform.position;
            float a_Dist = (m_Target.transform.position - transform.position).magnitude;

            // �÷��̾ ���ݹ��� �ȿ� ������ ���ݻ��·� ��ȯ
            if (a_Dist <= m_AttRange)
            {
                m_NavAgent.SetDestination(transform.position);
                State = Define_S.AllState.Attack;
                return;
            }

            // �÷��̾ ��ĵ������ ����� ���� ��ġ�� ����
            if (a_Dist > m_ScanRange)
            {
                m_Target = null;
                m_DPos = m_SpawnPosition;
                transform.rotation = Quaternion.Euler(0, -180, 0); // ������ ��ġ�� ���ư��� ȸ�� ����
            }
        }

        // ��ã�� �̵�
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
