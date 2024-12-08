using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Ctrl : Base_Ctrl
{
    public GameObject m_ClickEff;

    float m_Speed = 5f;     // 이동 속도

    float m_MaxHp = 100;
    float m_CurHp = 100;
    float m_MaxMp = 100;
    float m_CurMp = 100;

    bool IsAttack = true;     // 공격 가능 여부
    bool IsRoll = false;    // 구르기 여부
    bool IsHit = false;    // 넘어진 상태 여부

    float m_RollTime = 0f;   // 현재 구르는 시간
    float m_AttCancle = 0;   // 공격 취소 시간

    Vector3 m_Dir;

    int m_Mask = (1 << (int)Define_S.Layer.Ground)
       | (1 << (int)Define_S.Layer.Monster)
       | (1 << (int)Define_S.Layer.Npc);

    [SerializeField]
    private GameObject waeponObjList;          // 무기 Prefab List

    void Awake()
    {
        Cam_Ctrl a_CamCtrl = Camera.main.GetComponent<Cam_Ctrl>();

        if (a_CamCtrl != null)
            a_CamCtrl.InitCam(gameObject);

        m_Anim = GetComponent<Animator>();
    }

    public override void Init()
    {
        m_CurHp = m_MaxHp;
        m_CurMp = m_MaxMp;

        m_Anim = GetComponent<Animator>();

        m_WObject = Define_S.W_Object.Player;
        State = Define_S.AllState.Idle;
    }

    private Coroutine Co_HitDown;
    private Coroutine co_LevelUp;

    #region State
    protected override void Idle()
    {
        if (IsAttack == false)
            StopAtt();
    }

    //클릭 이동
    float a_TargetRange = 1.5f;
    protected override void Move()
    {
        if (m_Target != null)
        {
            float a_Dist = (m_Target.transform.position - transform.position).magnitude;
            if (a_Dist <= a_TargetRange)
            {
                State = Define_S.AllState.Idle;

                if (m_Target.GetComponent<Npc_Ctrl>() != null)
                {
                    m_Target.GetComponent<Npc_Ctrl>().Interact();
                }

                return;
            }
        }

        m_DPos.y = 0;
        m_Dir = m_DPos - transform.position;

        if (m_Dir.magnitude < 0.1f)
        {
            State = Define_S.AllState.Idle;
            m_ClickEff.gameObject.SetActive(false);
        }
        else
        {
            if (Define_S.AllState.Roll == State) return;//구르기 상태일때는 이동 불가
            if (Define_S.AllState.Attack == State) return;//공격 상태일때는 이동 불가

            if (BlockCheck() == true)
            {
                if (Input.GetMouseButton(0) == false)
                    State = Define_S.AllState.Idle;

                return;
            }

            float a_Dist = Mathf.Clamp(m_Speed * Time.deltaTime, 0, m_Dir.magnitude);
            transform.position += m_Dir.normalized * Time.deltaTime * m_Speed;
            Quaternion targetRotation = Quaternion.LookRotation(m_Dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 20 * Time.deltaTime);
        }
    }

    //구르기 -- 구르기 후 2초 구르기 불가
    private float m_DTime = 0.8f;
    protected override void Roll()
    {
        m_RollTime += Time.deltaTime;
        if (m_RollTime >= m_DTime)
        {
            ClearRoll();
            return;
        }

        StopAtt();

        m_DPos = GetMouseRay();
        float a_Dist = Mathf.Clamp(m_Speed * Time.deltaTime, 0, m_Dir.magnitude);

        transform.position += m_Dir.normalized * a_Dist;
        Quaternion targetRotation = Quaternion.LookRotation(m_Dir);
        targetRotation.x = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 20f * Time.deltaTime);

        if (BlockCheck() == true)
        {
            IsRoll = false;
            m_Speed = 5;
            return;
        }
    }

    private void EventRoll()
    {
        ClearRoll();
    }

    protected override void Attack()
    {
        m_AttCancle += Time.deltaTime;

        if ((m_Anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") == true &&
            m_AttCancle > 0.94f) ||
            (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") == true &&
             m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.2f))
        {
            StopAtt();
            State = Define_S.AllState.Idle;
            return;
        }

        AnimAtt(); // 공격 애니메이션 실행
    }
    #endregion

    #region Click
    protected override void SetMouseEvent(Define_S.MouseEvent a_Event)
    {
        switch (State)
        {
            case Define_S.AllState.Idle:
                GetMouseEvent(a_Event);
                break;
            case Define_S.AllState.Moving:
                GetMouseEvent(a_Event);
                break;
            case Define_S.AllState.Attack:
                GetMouseEvent(a_Event);
                break;
        }
    }

    float m_MinDist = 0.3f;
    void GetMouseEvent(Define_S.MouseEvent a_Event)
    {
        Ray a_Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool a_Hit = Physics.Raycast(a_Ray, out m_Hit, 150f, m_Mask);

        Debug.DrawRay(a_Ray.origin, a_Ray.direction * 100, Color.red);

        if (!a_Hit || m_Hit.collider == null) return;

        float a_Dist = (m_Hit.point - transform.position).magnitude;
        if (a_Dist <= m_MinDist) return;

        switch (a_Event)
        {
            case Define_S.MouseEvent.RightDown:
                {
                    m_DPos = m_Hit.point;
                    if (a_Hit && IsAttack)
                    {
                        State = Define_S.AllState.Moving;

                        m_ClickEff.gameObject.SetActive(false);
                        m_ClickEff.gameObject.SetActive(true);
                        m_ClickEff.transform.position = m_DPos;

                        if (m_Hit.collider.gameObject.layer == (int)Define_S.Layer.Npc)
                            m_Target = m_Hit.collider.gameObject;
                        else
                            m_Target = null;
                    }
                }
                break;

            case Define_S.MouseEvent.Right:
                {
                    if (IsAttack == false)
                    {
                        if (State == Define_S.AllState.Idle)
                            State = Define_S.AllState.Moving;

                        if (m_Target != null)
                            m_DPos = m_Target.transform.position;
                        else if (a_Hit)
                            m_DPos = m_Hit.point;
                    }
                }
                break;

            case Define_S.MouseEvent.LeftDown:
                {
                    IsAttack = true;
                    m_DPos = m_Hit.point;
                    m_DPos.y = 0;
                    State = Define_S.AllState.Attack;
                    OnAtt();
                }
                break;

            case Define_S.MouseEvent.Left:
                {
                    if (IsAttack == true)
                    {
                        m_DPos = m_Hit.point;
                        m_DPos.y = 0;
                        OnAtt();
                    }
                }
                break;
        }
    }

    bool OnAttack = false;
    void OnAtt()
    {
        if (OnAttack &&
            m_Anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") &&
            m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.3f)
        {
            OnAttack = true;
        }

        if (OnAttack == false)
        {
            State = Define_S.AllState.Attack;
            OnAttack = true;

            m_Dir = m_DPos - transform.position;
            transform.rotation = Quaternion.LookRotation(GetMouseRay() - transform.position);

            AnimAtt();
        }
    }

    //공격 애니메이션 이벤트
    private void ExitAttack()
    {
        // 공격 진행
        State = Define_S.AllState.Attack;

        // 회전
        m_Dir = m_DPos - transform.position;
        transform.rotation = Quaternion.LookRotation(GetMouseRay() - transform.position);

        AnimAtt();
    }
    #endregion

    #region KeyInput
    protected override void OnKeyEvent()
    {
        if (State == Define_S.AllState.Die)
            return;

        if (IsRoll == false)
        {
            GetDiveRoll();
            //GetSkill();
        }
    }

    private void GetDiveRoll()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (BlockCheck() == true)
                return;

            IsHit = false;
            IsRoll = true;
            IsAttack = false;

            m_DPos = GetMouseRay();
            m_Dir = m_DPos - transform.position;

            m_Speed = 5;

            State = Define_S.AllState.Roll;
        }
    }
    #endregion

    void StopAtt()
    {
        IsAttack = false;
        OnAttack = false; // OnAttack 플래그를 false로 설정
        m_AttCancle = 0;
    }

    void ClearRoll()
    {
        m_RollTime = 0f;
        m_Speed = 5;
        IsRoll = false;
        IsAttack = true; // IsAttack 플래그를 true로 설정
        State = Define_S.AllState.Idle;
    }

    #region Check
    Vector3 GetMouseRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out m_Hit, 150f, m_Mask);

        Vector3 hitPoint = m_Hit.point;
        hitPoint.y = 0;
        return hitPoint;
    }

    private bool BlockCheck()
    {
        if (Physics.Raycast(transform.position + (Vector3.up * 0.5f), m_Dir, 1.0f, 1 << 10))
            return true;

        return false;
    }
    #endregion
}
