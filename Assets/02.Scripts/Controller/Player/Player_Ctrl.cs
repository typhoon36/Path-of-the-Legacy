using I18N.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Ctrl : Base_Ctrl
{
    //모든 장비 오브젝트 저장
    public Dictionary<int, List<GameObject>> m_Equipment;

    public GameObject m_ClickMark;
    public GameObject m_CurEff;
    public SkillData m_CurSkill;

    //ClickMark Layer
    int m_Mask = (1 << (int)Define_S.Layer.Ground) | (1 << (int)Define_S.Layer.Monster) | (1 << (int)Define_S.Layer.Npc);

    #region 임시 변수
    float m_Speed = 5f;

    public float m_MaxHp = 100;
    private float m_CurHp;
    //public 대신 get,set 사용 -- 나중에 Data로 되어 제거할 예정
    public float CurHp
    {
        get { return m_CurHp; }
        set
        {
            m_CurHp = Mathf.Clamp(value, 0, m_MaxHp);
            UI_Mgr.Inst.UpdateHPBar(m_CurHp, m_MaxHp);

            if (m_CurHp <= 0)
            {
                OnDie();
            }
        }
    }
    public float m_MaxMp = 100;
    public float m_CurMp;
    #endregion


    #region Bool
    bool IsStopAtt = true;//공격 가능인지 여부
    bool IsRoll = false;//구르기 상태
    [HideInInspector] public bool IsHit = false; //피격 상태
    #endregion

    #region Float
    float m_RollTime = 0f;//구르기 시간
    float m_AttTime = 0f;//공격 취소 시간
    #endregion

    [SerializeField]
    public int m_Att = 10; // 플레이어 공격력 추가

    Vector3 m_Dir; //이동 방향

    public GameObject[] m_SkinnedObjs;


    [SerializeField]
    private GameObject m_WeaponList;

    [SerializeField]
    private List<EffectData> m_Effects;

    #region JoyStick & Buttons
    #endregion

    #region Init
    void Awake()
    {
        Cam_Ctrl a_CamCtrl = Camera.main.GetComponent<Cam_Ctrl>();

        if (a_CamCtrl != null)
            a_CamCtrl.InitCam(gameObject);
    }

    public override void Init()
    {
        m_Anim = GetComponent<Animator>();

        CurHp = m_MaxHp;
        m_CurMp = m_MaxMp;

        UI_Mgr.Inst.m_HPBar.fillAmount = 1;

        m_Equipment = new Dictionary<int, List<GameObject>>();
        m_CurEff = null;

        m_WObject = Define_S.W_Object.Player;
        State = Define_S.AllState.Idle;


        //SetPart();
    }
    #endregion

    #region Levelup
    Coroutine Co_LevelUp;
    public void LevelUpEffect()
    {
        if (Co_LevelUp == null) StopCoroutine(Co_LevelUp);
        Co_LevelUp = StartCoroutine(LevelUpCoroutine());
    }
    IEnumerator LevelUpCoroutine()
    {
        // 레벨업 이펙트 Prefab 생성
        GameObject effect = Instantiate(Resources.Load("Effect/LevelupBuff")) as GameObject;
        effect.transform.localPosition = Vector3.zero;

        yield return new WaitForSeconds(4f);

        Destroy(effect);//이펙트 삭제
    }
    #endregion

    #region State Pattern
    protected override void Idle()
    {
        if (IsStopAtt == false)
            StopAtt();
    }


    #region Moving
    float m_ScanRange = 1.5f;
    protected override void Move()
    {
        if (m_Target != null)
        {
            float a_Dist = (m_Target.transform.position - transform.position).magnitude;

            if (a_Dist <= m_ScanRange)
            {
                State = Define_S.AllState.Idle;

                //타겟이 NPC라면 상호작용
                if (m_Target.GetComponent<Npc_Ctrl>() != null)
                    m_Target.GetComponent<Npc_Ctrl>().GetInteract();
                return;
            }

        }

        m_DPos.y = 0;//높이값 0으로 고정

        //타겟과 거리
        m_Dir = m_DPos - transform.position;

        //0.1f 이하로 가까워지면 멈춤
        if (m_Dir.magnitude < 0.1f)
        {
            State = Define_S.AllState.Idle;
            m_ClickMark.SetActive(false);
        }
        else
        {
            //가는 중 벽이 있으면 멈추기
            if (BlockCheck() == true)
            {
                if (Input.GetMouseButton(0) == false)
                    State = Define_S.AllState.Idle;
                return;
            }

            float a_MoveDist = Mathf.Clamp(m_Speed * Time.deltaTime, 0, m_Dir.magnitude);

            transform.position += m_Dir.normalized * a_MoveDist;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_Dir), 20 * Time.deltaTime);
        }
    }
    #endregion

    #region Roll
    float m_DTime = 0.8f;
    protected override void Roll()
    {
        m_RollTime += Time.deltaTime;

        if (m_RollTime > m_DTime)
        {
            ClearRoll();
            return;
        }

        StopAtt();

        float a_MoveDist = Mathf.Clamp(m_Speed * Time.deltaTime, 0, m_Dir.magnitude);

        //이동
        transform.position += m_Dir.normalized * a_MoveDist;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_Dir), 20 * Time.deltaTime);

        //벽 확인
        if (BlockCheck() == true)
            ClearRoll();

    }

    //구르기 초기화
    void ClearRoll()
    {
        IsRoll = false;
        m_RollTime = 0f;
        m_Speed = 5f;
        State = Define_S.AllState.Idle;
    }

    //구르기 이벤트
    void EventRoll()
    {
        ClearRoll();
    }


    #endregion

    #region Attack

    protected override void Attack()
    {
        m_AttTime += Time.deltaTime;

        if ((m_Anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") == true &&
         m_AttTime > 0.94f) || (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") == true &&
          m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.2f))
        {
            StopAtt();
            State = Define_S.AllState.Idle;
            return;
        }
    }
    bool OnAttack = false;//공격 중인지 여부
    void OnAtt()
    {
        if (OnAttack &&
            m_Anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") &&
            m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.3f)
        {
            OnAttack = true;
        }

        // 공격
        if (OnAttack == false)
        {
            State = Define_S.AllState.Attack;
            OnAttack = true;

            // 회전
            m_Dir = m_DPos - transform.position;
            transform.rotation = Quaternion.LookRotation(GetMouseRay() - transform.position);
        }
    }

    // 공격 중지
    void StopAtt()
    {
        OnAttack = false;
        IsStopAtt = true;
        m_AttTime = 0f;
    }
    // 공격 애니메이션 이벤트
    void ExitAttack()
    {
        if (OnAttack == true)
        {
            State = Define_S.AllState.Attack;
            OnAttack = false;

            // 회전
            m_Dir = m_DPos - transform.position;
            transform.rotation = Quaternion.LookRotation(GetMouseRay() - transform.position);
        }
    }
    #endregion

    #region Hit
    private Coroutine Co_HitDown;
    public void OnHit(MonsterStat attacker, int a_Dmg = 0)
    {
        if (IsRoll == true) return;

        if (Co_HitDown != null)
            StopCoroutine(Co_HitDown);

        Co_HitDown = StartCoroutine(HitDown(attacker, a_Dmg));
    }

    IEnumerator HitDown(MonsterStat attacker, int a_Dmg)
    {
        CurHp -= a_Dmg;

        if (CurHp <= 0)
        {
            CurHp = 0;
            OnDie();
            yield break;
        }

        if (State == Define_S.AllState.Die) yield break;

        State = Define_S.AllState.Hit;
        IsHit = true;

        // 공격자 바라보기
        Vector3 a_Dir = attacker.transform.position - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(a_Dir), 1);

        yield return new WaitForSeconds(2f);

        if (IsRoll == false)
            State = Define_S.AllState.Idle;

        IsHit = false;
    }
    #endregion


    #region Skill
    void EffectClose()
    {
        // Null Check
        if (m_CurEff == null)
            return;

        // Effect 비활성화 진행
        m_CurEff.GetComponent<EffectData>().EffectDisableDelay();
    }

    //스킬 끝날때 이벤트
    void EventSkill()
    {
        EffectClose();
        ClearRoll();
        State = Define_S.AllState.Idle;
    }

    // 스킬 사용
    private void OnSkill(SkillData a_Skill)
    {
        // Null Check
        if (a_Skill == null)
        {
            Debug.Log("등록된 스킬이 없습니다!");
            return;
        }

        // 쿨타임 확인
        if (a_Skill.isCoolDown == true)
        {
            Debug.Log("쿨타임 중입니다.");
            return;
        }

        // 마나 확인
        if (a_Skill.skillConsumMp > m_CurMp)
        {
            Debug.Log("마나가 부족합니다.");
            return;
        }

        // 일반 공격 중지
        StopAtt();

        // 마우스 방향으로 회전
        m_DPos = GetMouseRay();
        m_Dir = m_DPos - transform.position;
        transform.rotation = Quaternion.LookRotation(m_Dir);

        m_CurSkill = a_Skill;

        // 스킬 이펙트 찾기
        foreach (EffectData a_Effect in m_Effects)
        {
            if (m_CurSkill.skillId == a_Effect.id)
            {
                m_CurEff = a_Effect.gameObject;
                break;
            }
        }

        // 애니메이션 상태 이름 설정
        string skillAnimName = "Skill" + m_CurSkill.skillId;

        // 스킬 진행
        State = Define_S.AllState.Skill;
        m_Anim.CrossFade(skillAnimName, 0.1f);

        m_CurSkill.isCoolDown = true;
        m_CurSkill.skillCoolDown = (int)Time.time; // 쿨다운 시작 시간 설정
        m_CurMp -= m_CurSkill.skillConsumMp;

        UI_Mgr.Inst.UpdateMpBar(m_CurMp, m_MaxMp);

        // 스킬 이펙트 활성화
        if (m_CurEff != null)
        {
            m_CurEff.SetActive(true);
        }
        else
        {
            Debug.LogWarning("스킬 이펙트를 찾을 수 없습니다.");
        }
    }

    // 스킬 가져오기
    private SkillData GetSkill(Define_S.KeySkill keySkill)
    {
        // Scene UI의 스킬바에 스킬이 존재하는지 확인
        if (UI_Mgr.Inst.SkillBarList.TryGetValue(keySkill, out SkillData skill) == false)
            return null;

        return skill;
    }
    #endregion


    #endregion

    #region 마우스 클릭

    protected override void SetMouseEvent(Define_S.MouseEvent a_Event)
    {
        if (UI_Mgr.Inst.IsPressed)
        {
            UI_Mgr.Inst.ResetButtonPress();
            return;
        }

        GetMouseEvent(a_Event);
    }

    float m_MinDist = 0.3f;
    void GetMouseEvent(Define_S.MouseEvent evt)
    {
        if (UI_Mgr.Inst.IsPressed)
        {
            UI_Mgr.Inst.ResetButtonPress();
            return;
        }

        Ray a_Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool a_Hit = Physics.Raycast(a_Ray, out m_Hit, 150f, m_Mask);

        //자신 클릭시 진행 x
        float a_Dist = (m_Hit.point - transform.position).magnitude;
        if (a_Dist < m_MinDist) return;

        switch (evt)
        {
            case Define_S.MouseEvent.RightDown:
                {
                    m_DPos = m_Hit.point;
                    if (a_Hit && IsStopAtt)
                    {
                        State = Define_S.AllState.Moving;

                        m_ClickMark.SetActive(false);
                        m_ClickMark.SetActive(true);
                        m_ClickMark.transform.position = m_DPos;

                        //클릭 위치에 타겟이 있다면 지정
                        if (m_Hit.collider.gameObject.layer == (int)Define_S.Layer.Npc)
                        {
                            m_Target = m_Hit.collider.gameObject;
                        }
                        else
                            m_Target = null;
                    }

                }
                break;

            case Define_S.MouseEvent.Right:
                {
                    //공격중이 아니라면
                    if (IsStopAtt == true)
                    {
                        //멈추고 있다면 이동
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

                    IsStopAtt = false;
                    m_DPos = m_Hit.point;
                    m_DPos.y = 0;
                    OnAtt();
                }
                break;

            case Define_S.MouseEvent.Left:
                {
                    if (IsStopAtt == false)
                    {
                        m_DPos = m_Hit.point;
                        m_DPos.y = 0;
                        OnAtt();
                    }

                }
                break;
        }
    }
    #endregion

    #region 키입력
    protected override void OnKeyEvent()
    {
        if (State == Define_S.AllState.Die) return;

        //구르지 않을때 
        if (IsRoll == false)
        {
            GetRoll();
            GetSkill();
        }

        GetUseItem();
        GetPickUp();

    }

    void GetRoll()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (BlockCheck() == true) return;

            IsHit = false;
            IsRoll = true;

            if (Co_HitDown != null)
                StopCoroutine(Co_HitDown);

            //도착좌표
            m_DPos = GetMouseRay();
            m_Dir = m_DPos - transform.position;

            m_CurMp -= 5;
            m_Speed = 8f;

            UI_Mgr.Inst.UpdateMpBar(m_CurMp, m_MaxMp);

            State = Define_S.AllState.Roll;

            EffectClose();
        }
    }

    void GetUseItem()
    {
        //번호키를 눌러 아이템 사용(소비 아이템이 아닌 키를 눌러 HP, MP 회복)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UI_Mgr.Inst.UseItem(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UI_Mgr.Inst.UseItem(1);
        }
    }

    [SerializeField]
    float a_ItemMaxRadius = 5f; // 아이템 줍기 최대 반경
    void GetPickUp()
    {
        // 주변 아이템 탐색
        Collider[] coll = Physics.OverlapSphere(transform.position, a_ItemMaxRadius, 1 << 12); //Item = 12 Layer

        // F 키를 누르면 줍기
        if (Input.GetKeyDown(KeyCode.F))
        {
            for (int i = 0; i < coll.Length; i++)
            {
                ItemPickUp a_Item = coll[i].GetComponent<ItemPickUp>();
                if (a_Item != null)
                {
                    // 인벤토리에 넣기
                    Debug.Log("아이템 줍기 : " + a_Item.item.ItemName);
                    InvenPopup_UI.Inst.AddItem();

                    Destroy(coll[i].gameObject);//필드 아이템 삭제

                    return;
                }
            }
        }

    }

    // 스킬 사용 
    void GetSkill()
    {
        if (State == Define_S.AllState.Skill || IsHit == true) return;

        // 무기가 없으면 스킬 사용 불가
        if (m_WeaponList == null) return;


        else if (Input.GetKeyDown(KeyCode.A))
        {
            OnSkill(GetSkill(Define_S.KeySkill.A));
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            OnSkill(GetSkill(Define_S.KeySkill.D));
        }
    }

    #endregion

    #region 체크
    Vector3 GetMouseRay()
    {
        // 마우스 Ray 쏘기
        Ray a_Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(a_Ray, out m_Hit, 150f, m_Mask);

        // 마우스 포인트 좌표 반환
        Vector3 a_HitPoint = m_Hit.point;
        a_HitPoint.y = 0;
        return a_HitPoint;
    }

    bool BlockCheck()
    {
        // 전방 Block 체크하여 멈추기 
        if (Physics.Raycast(transform.position + (Vector3.up * 0.5f), m_Dir, 1.0f, 1 << 10)) // 10 : Block
            return true;

        return false;
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("MonsterWeapon"))
        {
            Monster_Ctrl monster = coll.GetComponent<Monster_Ctrl>();

            if (monster != null)
            {
                int a_Dmg = monster.m_Att;
                MonsterStat a_MonStat = monster.GetComponent<MonsterStat>();
                if (a_MonStat != null)
                {
                    OnHit(a_MonStat, a_Dmg);
                }
            }
        }
    }

    void OnDie()
    {
        State = Define_S.AllState.Die;

        UI_Mgr.Inst.UpdateHPBar(CurHp, m_MaxHp);

        UI_Mgr.Inst.DieOn();

        Destroy(gameObject, 2f);
    }


    //public void SetPart()
    //{
    //    // 모든 장비 오브젝트 저장
    //    m_Equipment.Clear();

    //    foreach (var itemData in Data_Mgr.m_ItemData)
    //    {
    //        if (itemData.ItemType == Define_S.ItemType.Armor)
    //        {
    //            GameObject a_Obj = Instantiate(itemData.ItemObj);
    //            a_Obj.SetActive(false);

    //            if (m_Equipment.ContainsKey((int)Define_S.ItemType.Armor) == false)
    //                m_Equipment.Add((int)Define_S.ItemType.Armor, new List<GameObject>());
    //        }
    //    }
    //}
    #endregion

}
