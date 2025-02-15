using I18N.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ����
public class Necro_Ctrl : Monster_Ctrl
{
    [SerializeField] Transform m_RangeObj;// ��ų ���� ǥ�� ������Ʈ
    [SerializeField] MonsterAttColl m_AttColl; // �Ϲ� ���� ��� ����
    [SerializeField] MonsterSkillColl m_SkillColl;  // ��ų ��� ����
    [SerializeField] float m_SkillRange = 5f; // ���� ����

    // �ִϸ��̼� �̸�
    string[] Attacks = new string[] { "Attack1", "Attack2" };
    string skill = "Skill"; // �ϳ��� ��ų�� ���

    // Ƚ��
    int m_AttCnt = 0;
    int m_SkillCnt = 3;

    // üũ
    bool IsSkill = false;


    Portal m_ExitPortal; // ��Ż

    public override void Init()
    {
        base.Init();

        //������ ����
        m_AttColl.m_Damage = m_Stat.Attack;
        m_SkillColl.m_Damage = (int)(m_Stat.Attack * 1.2f);

        //������ ��Ż ã��
        m_ExitPortal = GameObject.FindObjectOfType<Portal>();
        if (m_ExitPortal != null)
            m_ExitPortal.gameObject.SetActive(false);

        m_MonsterType = Define_S.MonsterType.Boss;
    }

    #region State
    protected override void IdletoDetective()
    {
        base.IdletoDetective();
    }

    protected override void Move()
    {
        m_Nav.SetDestination(m_Target.transform.position);

        m_Dist = TargetDist(m_Target);

        OnRotate();
        AttCheck();
    }

    // ����/��ų ����(State)�� �� �ִϸ��̼� �����ӿ� ������
    protected override void Attack() { OnAnimMove(); }
    protected override void Skill() { OnAnimMove(); }

    protected override void Die()
    {
        base.Die();

        if (m_ExitPortal == null) return; // ��Ż�� ������ ����

        if (m_ExitPortal.gameObject.activeSelf == false)
            // ��Ż Ȱ��ȭ
            m_ExitPortal.gameObject.SetActive(true);
    }
    #endregion


    #region Animations
    Coroutine a_WeaponCo;
    protected override void OnAttEvent()
    {
        // ���� �ݶ��̴� Ȱ��ȭ
        m_AttColl.IsCollider(true);

        // ���� �ݶ��̴� ��Ȱ��ȭ �ڷ�ƾ ����
        if (a_WeaponCo != null) StopCoroutine(WeaponColl());
        a_WeaponCo = StartCoroutine(WeaponColl());
    }

    protected override void ExitAttEvent()
    {
        // 2~3�� �Ϲ� ���� �� ���� ���� ��ų ����
        if (m_AttCnt++ >= Random.Range(2, m_SkillCnt + 1))
        {
            IsSkill = true;
            m_AttCnt = 0; 
        }

        // ���� �ݶ��̴� ��Ȱ��ȭ
        m_AttColl.IsCollider(false);
        State = Define_S.AllState.Idle;
    }
    #endregion

    // ���� ���� Ȯ��
    void AttCheck()
    {
        // ��ų ������ �����ϴٸ�
        if (IsSkill == true)
        {
            if (m_Dist <= m_SkillRange + 1)
                SetSkill(skill);

            return;
        }
        else
        {
            if (m_Dist <= m_AttRange)
                SetAttack(Attacks[Random.Range(0, 2)]);
        }
    }

    // ���� ����
    void SetAttack(string a_AttName)
    {
        // ���� �ִϸ��̼� ����
        SetAnim(a_AttName);
        State = Define_S.AllState.Attack;
    }

    // ��ų ����
    void SetSkill(string a_SkillName)
    {
        // ��ų �ִϸ��̼� ����
        SetAnim(a_SkillName);

        // ��ų �ڷ�ƾ ����
        if (a_SkillName == "Skill")
        {
            StopCoroutine(Skill_Co());
            StartCoroutine(Skill_Co());
        }

        IsSkill = false;
        State = Define_S.AllState.Skill;
    }

    #region coroutines
    private IEnumerator Skill_Co()
    {
        // ���� ���� ���� ������ �� ��ġ ����
        m_RangeObj.localPosition = new Vector3(0, 0.16f, -0.44f);
        m_RangeObj.localScale = new Vector3(1, 0.00055f, 4.66f);

        yield return new WaitForSeconds(1f);

        m_RangeObj.gameObject.SetActive(true);

        m_SkillColl.IsParticle(true);

        yield return new WaitForSeconds(0.8f);

        m_SkillColl.IsParticle(false);
        m_RangeObj.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.2f);

        State = Define_S.AllState.Idle;
    }
    // ���� �ݶ��̴� ��Ȱ��ȭ �ڷ�ƾ
    IEnumerator WeaponColl()
    {
        // 0.15�� �� ��Ȱ��ȭ
        yield return new WaitForSeconds(0.15f);

        m_AttColl.IsCollider(false);
    }
    #endregion


    // �ִϸ��̼� �� ���� ����
    void SetAnim(string a_AnimName)
    {
        // �÷��̾�� �Ÿ���
        Vector3 a_Dir = m_Target.transform.position - transform.position;

        // Nav ���� ��ǥ ����
        m_Nav.SetDestination(transform.position);
        transform.rotation = Quaternion.LookRotation(a_Dir);

        // �ִϸ��̼� ����
        m_Anim.CrossFade(a_AnimName, 0.1f, -1, 0);
    }


    //�ڿ������� ȸ��
    void OnRotate()
    {
        //2D ���ͷ� x,z�� �˾Ƴ���.
        Vector2 a_Forward = new Vector2(transform.position.z, transform.position.x);
        // 2D ���ͷ� Ÿ��(�÷��̾�)�� x,z�� �˾Ƴ���.
        /// NavMeshAgent�� steeringTarget�� ��θ� ���� �̵��Ҷ� ȸ���� ������ ��Ÿ���� ���� ���ȴ�.
        Vector2 a_Target = new Vector2(m_Nav.steeringTarget.z, m_Nav.steeringTarget.x);

        // Ÿ�ٰ� ���� ��ġ�� ���̰��� ���ϰ�, Atan2�� �̿��Ͽ� ������ ���Ѵ�.
        Vector2 a_Dir = a_Target - a_Forward;
        float a_Angle = Mathf.Atan2(a_Dir.y, a_Dir.x) * Mathf.Rad2Deg;

        // ȸ������ �����Ѵ�.
        transform.eulerAngles = Vector3.up * a_Angle;
    }

    // �ִϸ��̼� ���������� ����
    void OnAnimMove()
    {
        Vector3 a_Pos = m_Anim.targetPosition; // �ִϸ��̼��� ���� ��ġ
        a_Pos.y = m_Nav.nextPosition.y;        // Nav Y

        transform.position = a_Pos;
        m_Nav.SetDestination(a_Pos);
    }


}