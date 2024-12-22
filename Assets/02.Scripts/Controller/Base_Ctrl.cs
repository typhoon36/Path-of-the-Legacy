using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//플레이어&몬스터의 애니메이션 제어 
public abstract class Base_Ctrl : MonoBehaviour
{
    [SerializeField]
    public Define_S.W_Object m_WObject { get; protected set; } = Define_S.W_Object.Unknown;

    [SerializeField]
    protected GameObject m_Target;                // 마우스로 타겟한 오브젝트 담는 변수

    [SerializeField]
    protected Vector3 m_DPos;                   // 도착 좌표

    [SerializeField]
    protected Define_S.AllState m_State = Define_S.AllState.Idle;//상태

    protected Animator m_Anim;
    protected RaycastHit m_Hit;

    //캐릭터의 상태에 따라 애니메이션 작동
    public virtual Define_S.AllState State
    {
        get { return m_State; }
        set
        {
            if (m_State == value)
                return;
            m_State = value;
            switch (m_State)
            {
                case Define_S.AllState.Idle:
                    if (m_Anim.HasState(0, Animator.StringToHash("Idle")))
                        m_Anim.CrossFade("Idle", 0.4f);
                    break;

                case Define_S.AllState.Moving:
                    if (m_Anim.HasState(0, Animator.StringToHash("Run")))
                        m_Anim.CrossFade("Run", 0.1f);
                    break;

                case Define_S.AllState.Roll:
                    if (m_Anim.HasState(0, Animator.StringToHash("Roll")))
                        m_Anim.CrossFade("Roll", 0.1f, -1, 0);
                    break;

                case Define_S.AllState.Attack:
                    if (m_Anim.HasState(0, Animator.StringToHash("Attack")))
                        m_Anim.CrossFade("Attack", 0.2f);
                    break;

                case Define_S.AllState.Hit:
                    if (m_Anim.HasState(0, Animator.StringToHash("HiT")))
                        m_Anim.CrossFade("HiT", 0.2f);
                    break;

                case Define_S.AllState.Die:
                    if (m_Anim.HasState(0, Animator.StringToHash("Die")))
                        m_Anim.CrossFade("Die", 0.1f);
                    break;
            }
        }
    }

    public abstract void Init();

    void Start()
    {
        Init();
        m_Target = null;
    }

    void Update()
    {
        if (m_WObject == Define_S.W_Object.Monster) return;

        // 마우스 이벤트 처리
        if (Input.GetMouseButtonDown(0))
        {
            SetMouseEvent(Define_S.MouseEvent.LeftDown);
        }
        else if (Input.GetMouseButton(0))
        {
            SetMouseEvent(Define_S.MouseEvent.Left);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            SetMouseEvent(Define_S.MouseEvent.RightDown);
        }
        else if (Input.GetMouseButton(1))
        {
            SetMouseEvent(Define_S.MouseEvent.Right);
        }

        // 키 입력 처리
        OnKeyEvent();

        switch (m_State)
        {
            case Define_S.AllState.Idle:
                Idle();
                break;
            case Define_S.AllState.Moving:
                Move();
                break;
            case Define_S.AllState.Roll:
                Roll();
                break;
            case Define_S.AllState.Attack:
                Attack();
                break;
            case Define_S.AllState.Skill:
                Skill();
                break;
            case Define_S.AllState.Hit:
                Hit();
                break;
            case Define_S.AllState.Die:
                Die();
                break;
        }
    }

    //몬스터의 상태에 따라 애니메이션 작동
    void FixedUpdate()
    {
        if (m_WObject != Define_S.W_Object.Monster) return;

        switch (m_State)
        {
            case Define_S.AllState.Idle:
                Idle();
                break;
            case Define_S.AllState.Moving:
                Move();
                break;
            case Define_S.AllState.Attack:
                Attack();
                break;
            case Define_S.AllState.Hit:
                Hit();
                break;
            case Define_S.AllState.Die:
                Die();
                break;
        }

    }

    protected virtual void AnimAtt()
    {
        string attackStateName = "Attack";
        if (m_Anim.HasState(0, Animator.StringToHash(attackStateName)))
        {
            m_Anim.CrossFade(attackStateName, 0.1f);
        }
    }

    #region State
    protected virtual void Move() { }
    protected virtual void Roll() { }
    protected virtual void Idle() { }
    protected virtual void Attack() { }


    protected virtual void Skill() { }
    protected virtual void Hit() { }

    protected virtual void Die() { }
    #endregion

    protected virtual void SetMouseEvent(Define_S.MouseEvent a_Event) { }

    protected virtual void OnKeyEvent() { }

}
