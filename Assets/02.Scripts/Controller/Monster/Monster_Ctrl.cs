using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;//NavMeshAgent ����� ���� �߰�
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
    public int m_Att = 10; // ���ݷ� �߰�

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
        m_SpawnPosition = transform.position; // ������ ��ġ ����
        HPBar.fillAmount = 1;
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
                AnimAtt();
                return;
            }

            // �÷��̾ ��ĵ������ ����� ���� ��ġ�� ����
            if (a_Dist > m_ScanRange && a_Dist > m_AttRange)
            {
                m_Target = null;
                m_DPos = m_SpawnPosition;
                transform.rotation = Quaternion.Euler(0, -180, 0); // ������ ��ġ�� ���ư��� ȸ�� ����
                State = Define_S.AllState.Moving;
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
            if (player != null && player.State == Define_S.AllState.Attack) // �÷��̾ ���� ������ ����
            {
                int damage = player.m_Att; // �÷��̾��� ���ݷ� ��������

                MonsterStat monsterStat = GetComponent<MonsterStat>();

                //������ ���� �� ����ó��
                if (monsterStat != null)
                {
                    monsterStat.OnAttacked(damage); // ���Ϳ��� ������ ����
                    if (monsterStat.Hp <= 0)
                        MonGen_Mgr.Inst.OnMonsterDeath(gameObject); // ���Ͱ� �׾��� �� ȣ��
                    
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
