using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//���� ���� 
public class Necro_Ctrl : Monster_Ctrl
{
    [SerializeField] float m_ArrowRange = 5f; //���Ÿ� ���� ����

    [SerializeField] GameObject m_ScytheTrail; //����(�보)Ʈ����

    [SerializeField] Transform m_RangeObj; //��ų ����(������ ���� �÷��̾ ������ ���� �� �ֵ���)

    [SerializeField] EffParticles m_ParticleColl;//�ǰ� ����Ʈ

    [SerializeField] MonsterAttColl m_AttColl; //���� �ݶ��̴�
    [SerializeField] MonsterAttColl m_SkillColl;//��ų �ݶ��̴�


    // ��ų, ���� �ִϸ��̼� �̸�(���� ���� ��ų,���� �ִϸ��̼�)
    string[] m_Skills = new string[] { "Skill1", "Skill2" };
    string[] m_MeleeAtt = new string[] { "Attack1" };
    string[] m_RangeAtt = new string[] { "Attack2" };

    Portal m_ExitPortal; //������ ��Ż

    int m_AttCnt = 0;
    int m_SkillCnt = 3;

    bool IsRAtt = false;//���Ÿ� ����������
    bool IsSkill = false;//��ų������



    public override void Init()
    {
        base.Init();  //�θ��� Monster_Ctrl�� Init() ȣ��

        //��ƼŬ �ǰ� ����
        m_ParticleColl.SetInfo(() => { m_Target.GetComponent<Player_Ctrl>().OnHit(m_Stat, (int)(m_Stat.Attack * 0.8f)); });

        //������ ���� ����
        m_SkillColl.m_Damage = (int)(m_Stat.Attack * 1.5f);
        m_AttColl.m_Damage = m_Stat.Attack;

        //������ ��Ż ����
        m_ExitPortal = GameObject.FindObjectOfType<Portal>();

        if (m_ExitPortal != null) m_ExitPortal.gameObject.SetActive(false);

        //Ÿ�� ����
        m_MonsterType = Define_S.MonsterType.Boss;

    }

    #region State
    protected override void IdletoDetective()
    {
        //���� �������� 40% Ȯ���� true, 60% Ȯ���� false�� �ؼ� ����
        IsRAtt = Random.Range(0, 10) < 4 ? true : false;

        base.IdletoDetective(); //�θ��� Monster_Ctrl�� IdletoDetective() ȣ��
    }

    protected override void Move()
    {
        //���� ����
        m_Nav.SetDestination(m_Target.transform.position);

        //Monster_Ctrl�� TargetDist()�� �̿��� Ÿ�ٰ��� �Ÿ� ����
        m_Dist = TargetDist(m_Target);

        //�����ߴ��� Ȯ��
        AttackCheck();
    }

    protected override void Attack() { OnAnimMove(); }

    //��ų ���
    protected override void Skill() { OnAnimMove(); }

    protected override void Die()
    {
        base.Die();

        if (m_ExitPortal != null) return;

        //������ ��Ż Ȱ��ȭ
        if (m_ExitPortal == null) m_ExitPortal.gameObject.SetActive(true);
    }
    #endregion


    /* ���� �̺�Ʈ */
    Coroutine m_DisableCo;
    protected override void OnAttEvent()
    {
        m_AttColl.IsCollider(true);

        if (m_DisableCo != null) StopCoroutine(m_DisableCo);

        m_DisableCo = StartCoroutine(DisableColl());

    }

    protected override void ExitAttEvent()
    {
        //���� ����
        if (m_AttCnt++ >= Random.Range(2, m_SkillCnt + 1))
        {
            IsSkill = true; m_AttCnt = 0;
        }

        //�����ߴٸ� �ݸ��� ��Ȱ��ȭ
        m_AttColl.IsCollider(false);
        //Idle���� 
        State = Define_S.AllState.Idle;
    }

    //���� Ȯ��
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

    //���� ����
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

    //�ִϸ��̼�(���⼳��) 
    void SetAnim(string a_AnimName)
    {
        Vector3 a_Dist = m_Target.transform.position - transform.position;

        m_Nav.SetDestination(transform.position);
        transform.rotation = Quaternion.LookRotation(a_Dist);

        //�ִϸ��̼� ����
        m_Anim.CrossFade(a_AnimName, 0.1f, -1);
    }

    void OnAnimMove()
    {
        Vector3 a_RtPos = m_Anim.targetPosition; //targetPosition�� �ִϸ��̼ǿ��� ������ ��ġ
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
    IEnumerator Skill1() //Ⱦ���� ��ų
    {
        //���� ���� ��ġ ����
        m_RangeObj.localPosition = new Vector3(0, 0, 4.5f);
        m_RangeObj.localScale = new Vector3(1, 1, 1);

        //1���� ����
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
        //���� ���� ��ġ ����
        m_RangeObj.localPosition = new Vector3(0, 0, 4.5f);
        m_RangeObj.localScale = new Vector3(1, 1, 1);

        //1���� ����
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


    /*Ʈ���� ���� */
    void OnTrail() { m_ScytheTrail.SetActive(true); }

    void OffTrail() { m_ScytheTrail.SetActive(false); }

}
