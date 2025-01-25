using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;//NavMeshAgent ���
using UnityEngine.UI;

public class Monster_Ctrl : Base_Ctrl
{
    public Define_S.MonsterType m_MonsterType; // ���� Ÿ��
    public Vector3 m_SpawnPos;//���� ���� ��ġ
    [Header("HUD")]
    public GameObject HPBack;
    public Image HPBar;
    public Text m_NickTxt;

    protected MonsterStat m_Stat;//���� ����
    NavMeshAgent m_Nav;

    protected float m_Dist;
    protected bool IsOver = false;

    [SerializeField] float m_ScanRange;//�÷��̾� ���� �Ÿ�
    [SerializeField] float m_AttRange;//���� ��Ÿ�
    [SerializeField] public int m_SpawnRange = 16; //���� ����


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
        //HP ����
        HPBar.fillAmount = (float)m_Stat.CurHp / m_Stat.MaxHp;
        HPBack.gameObject.SetActive(true);
        HPBar.gameObject.SetActive(true);

        //�г��� ����
        m_NickTxt.text = m_Stat.m_Name.ToString();


        //Ÿ���� �÷��̾�� ����
        m_Target = GameObject.FindGameObjectWithTag("Player");

        State = Define_S.AllState.Moving;
    }

    protected override void Idle()
    {
        GameObject a_Player = GameObject.FindGameObjectWithTag("Player");
        //����� ����
        if (a_Player == null) return;

        // �÷��̾� �Ÿ� üũ
        m_Dist = TargetDist(a_Player);

        // �÷��̾ ��ĵ ���� �ȿ� ������ �ν�
        if (m_Dist <= m_ScanRange)
        {
            IdleDetective();
        }
        // ��ĵ ���� �ۿ� ���� ��
        else
        {
            m_Target = null;
            State = Define_S.AllState.Idle;
        }
    }

    protected override void Move()
    {
        //�����Ÿ����� ����� ����
        if (IsOver == true) return;

        //�÷��̾ �װų� Ÿ���� null�̸� 
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

        //���� ��ǥ ����
        m_Nav.SetDestination(m_Target.transform.position);

        //Ÿ���� ��Ÿ� ���ʿ� �����ϸ�
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
        //�÷��̾ �׾��ٸ�
        if (m_Target == null || m_Target.GetComponent<Player_Ctrl>().State == Define_S.AllState.Die)
        {
            StartCoroutine(ReturnSpawn());
            return;
        }

        //ȸ���� ����
        Vector3 a_Rot = m_Target.transform.position - this.transform.position;
        a_Rot.y = 0;
        transform.rotation = Quaternion.LookRotation(a_Rot);
    }

    protected override void Hit()
    {
        //���߱� 
        m_Nav.SetDestination(this.transform.position);

        // �ǰ� �ִϸ��̼� �ð� üũ
        if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Hit") &&
            m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
        {
            State = Define_S.AllState.Moving;
        }

    }

    protected override void Die()
    {
        //���߱�
        m_Nav.SetDestination(this.transform.position);

        State = Define_S.AllState.Die;

        //�ݶ��̴��� ������ ���� ����
        if (GetComponent<Collider>() != null)
            StartCoroutine(DelayDestroy());

    }


    #region Animation Events
    //���� �̺�Ʈ
    protected virtual void OnAttEvent()
    {
        //�Ÿ��� 
        m_Dist = TargetDist(m_Target);

        //���� ��Ÿ� �ȿ� ������
        if (m_Dist <= m_AttRange)
        {
            //HP�� Ȱ��ȭ
            HPBar.gameObject.SetActive(true);

            //�÷��̾� ������ �ݿ�(���� ���ȿ� ���� ������ ����)
            MonsterStat a_Stat = GetComponent<MonsterStat>();
            int a_Dmg = Random.Range(a_Stat.Attack - 5, a_Stat.Attack + 5);
            m_Target.GetComponent<Player_Ctrl>().CurHp -= a_Dmg;
        }


    }

    /// [���� �̺�Ʈ ����]
    protected virtual void ExitAttEvent()
    {
        State = Define_S.AllState.Moving;
    }

    //�ǰ� �̺�Ʈ
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
        //�ݶ��̴� ��Ȱ��ȭ
        GetComponent<Collider>().enabled = false;

        yield return new WaitForSeconds(3f);

        //���� ����
        State = Define_S.AllState.Idle;
        Destroy(this.gameObject);

        //�ݶ��̴� Ȱ��ȭ
        GetComponent<Collider>().enabled = true;

        //ü�� �ʱ�ȭ
        m_Stat.CurHp = m_Stat.MaxHp;

    }

    //���� �������� �̵�
    IEnumerator ReturnSpawn()
    {
        IsOver = true;

        //���� ����
        BattleEnd();

        //���� �������� �̵�
        m_Nav.SetDestination(m_SpawnPos);

        //������ ��������� ���߱�
        while (true)
        {
            if (Vector3.Distance(transform.position, m_SpawnPos) <= 0.7f)
            {
                m_Nav.SetDestination(this.transform.position);
                State = Define_S.AllState.Idle; // Idle ���·� ����
                IsOver = false; // IsOver ���� �ʱ�ȭ
                break;
            }
            yield return null;
        }
    }
    #endregion

    //���� ����
    public void BattleEnd()
    {
        m_Target = null;

        //�ٽ� ������������ �̵�
        m_Nav.SetDestination(m_SpawnPos);
        //���� ����� HP�� ��Ȱ��ȭ
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
