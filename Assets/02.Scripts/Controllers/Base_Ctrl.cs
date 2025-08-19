using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class Base_Ctrl : MonoBehaviour
{
    [SerializeField] public Define.WorldObject WorldObjectType { get; protected set; } = Define.WorldObject.Unknown;

    [SerializeField] protected GameObject m_LockTarget;                // 마우스로 타겟한 오브젝트 담는 변수

    [SerializeField] protected Vector3 m_DestPos;                   // 도착 좌표

    [SerializeField] protected Define.State m_State = Define.State.Idle; // 상태 변수

    protected int m_AttNumber = 1;           // 일반 공격 콤보 체크

    protected Animator m_Anim;
    protected RaycastHit m_Hit;

    // 캐릭터 상태에 따라 애니메이션 작동
    public virtual Define.State State
    {
        get { return m_State; }
        set
        {
            m_State = value;

            switch (m_State)
            {
                case Define.State.Moving:
                    m_Anim.CrossFade("Run", 0.1f);
                    break;
                case Define.State.Idle:
                    m_Anim.CrossFade("Idle", 0.4f);
                    break;
                case Define.State.DiveRoll:
                    m_Anim.CrossFade("Roll", 0.1f, -1, 0);
                    break;
                case Define.State.Attack:
                    AnimAttack();
                    break;
                case Define.State.Hit:
                    m_Anim.CrossFade("Hit", 0.1f, -1, 0);
                    break;
                case Define.State.Down:
                    m_Anim.CrossFade("Down", 0.1f, -1, 0);
                    break;
                case Define.State.Die:
                    m_Anim.CrossFade("Die", 0.1f, -1, 0);
                    break;
            }
        }
    }

    void Start()
    {
        Init();
        m_LockTarget = null;
    }

    // Playe, NPC 전용 ( 키 입력이 필요한 경우 )
    void Update()
    {
        if (WorldObjectType == Define.WorldObject.Monster)
            return;

        switch (State)
        {
            case Define.State.Moving:    // 움직임
                UpdateMoving();
                break;
            case Define.State.DiveRoll:  // 구르기
                UpdateDiveRoll();
                break;
            case Define.State.Idle:      // 가만히 있기
                UpdateIdle();
                break;
            case Define.State.Attack:     // 일반 공격
                UpdateAttack();
                break;
            case Define.State.Skill:     // 스킬
                UpdateSkill();
                break;
            case Define.State.Hit:       // 피격
                UpdateHit();
                break;
            case Define.State.Die:       // 죽음
                UpdateDie();
                break;
        }
    }

    // Monster 전용
    void FixedUpdate()
    {
        if (WorldObjectType != Define.WorldObject.Monster)
            return;

        switch (State)
        {
            case Define.State.Moving:    // 움직임
                UpdateMoving();
                break;
            case Define.State.Idle:      // 가만히 있기
                UpdateIdle();
                break;
            case Define.State.Attack:     // 일반 공격
                UpdateAttack();
                break;
            case Define.State.Skill:     // 스킬
                UpdateSkill();
                break;
            case Define.State.Hit:       // 피격
                UpdateHit();
                break;
            case Define.State.Die:       // 죽음
                UpdateDie();
                break;
        }
    }

    public abstract void Init();

    // 기본 공격 애니메이션
    protected virtual void AnimAttack()
    {
        m_Anim.CrossFade("Attack" + m_AttNumber, 0.1f, -1, 0);

        if (m_AttNumber == 1)
            m_AttNumber = 2;
        else if (m_AttNumber == 2)
            m_AttNumber = 1;
    }

    protected virtual void UpdateMoving() { }
    protected virtual void UpdateDiveRoll() { }
    protected virtual void UpdateIdle() { }
    protected virtual void UpdateAttack() { }
    protected virtual void UpdateSkill() { }
    protected virtual void UpdateHit() { }
    protected virtual void UpdateDie() { }
}
