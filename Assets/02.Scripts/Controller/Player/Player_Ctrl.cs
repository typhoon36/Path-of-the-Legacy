using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player_Ctrl : Base_Ctrl
{
    //모든 장비 오브젝트 저장
    public Dictionary<int, List<GameObject>> m_Equipment;

    public GameObject m_ClickMark;
    public GameObject m_CurEff;
    public SkillData m_CurSkill;

    //ClickMark Layer
    int m_Mask = (1 << (int)Define_S.Layer.Ground)
        | (1 << (int)Define_S.Layer.Monster)
        | (1 << (int)Define_S.Layer.Npc);


    public float m_Speed = 5; //플레이어 이동속도

    [SerializeField]
    public int m_Att = 10; // 플레이어 공격력 추가

    Vector3 m_Dir; //이동 방향

    public GameObject[] m_SkinnedObjs;//장착 아이템 오브젝트(장착시 보이는 오브젝트)


    [SerializeField]
    private GameObject m_WeaponList;//무기 리스트

    [SerializeField]
    public List<EffectData> m_Effects;

    #region HP & MP
    float m_MaxHp = 100f;
    public float MaxHp
    {
        get { return m_MaxHp; }
        set
        {
            float delta = value - m_MaxHp;
            m_MaxHp = Mathf.Clamp(value, 0, m_MaxHp + delta);
            CurHp += delta; // 최대 체력이 증가할 때 현재 체력도 함께 증가
            Data_Mgr.m_StartData.MaxHp = (int)m_MaxHp;
            UI_Mgr.Inst.UpdateHPBar(m_CurHp, m_MaxHp);
        }
    }

    private float m_CurHp;

    public float CurHp
    {
        get { return m_CurHp; }
        set
        {
            m_CurHp = Mathf.Clamp(value, 0, m_MaxHp);
            Data_Mgr.m_StartData.CurHp = (int)m_CurHp;
            UI_Mgr.Inst.UpdateHPBar(m_CurHp, m_MaxHp);

            if (m_CurHp <= 0)
                OnDie();

        }
    }

    float m_MaxMp = 100f;
    public float MaxMp
    {
        get { return m_MaxMp; }
        set
        {
            m_MaxMp = Mathf.Clamp(value, 0, m_MaxMp);
            Data_Mgr.m_StartData.MaxMp = (int)m_MaxMp;
            UI_Mgr.Inst.UpdateMpBar(m_CurMp, m_MaxMp);
        }
    }

    float m_CurMp;
    public float CurMp
    {
        get { return m_CurMp; }
        set
        {
            m_CurMp = Mathf.Clamp(value, 0, m_MaxMp);
            Data_Mgr.m_StartData.CurMp = (int)m_CurMp;
            UI_Mgr.Inst.UpdateMpBar(m_CurMp, m_MaxMp);
        }
    }
    #endregion

    #region Bool
    bool IsStopAtt = true;//공격 가능인지 여부
    bool IsRoll = false;//구르기 상태
    bool IsHit = false; //피격 상태
    #endregion

    #region Float
    float m_RollTime = 0f;//구르기 시간
    float m_AttTime = 0f;//공격 취소 시간
    #endregion



    #region Init
    void Awake()
    {
        Cam_Ctrl a_CamCtrl = Camera.main.GetComponent<Cam_Ctrl>();

        if (a_CamCtrl != null)
            a_CamCtrl.InitCam(gameObject);
    }

    //나가기 전에 저장
    void OnApplicationQuit()
    {
        Data_Mgr.m_StartData.CurHp = (int)CurHp;
        Data_Mgr.m_StartData.MaxHp = (int)MaxHp;
        Data_Mgr.m_StartData.CurMp = (int)CurMp;
        Data_Mgr.m_StartData.MaxMp = (int)MaxMp;
        Data_Mgr.m_StartData.m_Pos = transform.position;

        Data_Mgr.SaveData();
    }


    public override void Init()
    {
        m_Anim = GetComponent<Animator>();

        CurHp = m_MaxHp;
        CurMp = m_MaxMp;
        m_Speed = Data_Mgr.m_StartData.Speed;

        m_Equipment = new Dictionary<int, List<GameObject>>();
        m_CurEff = null;

        m_WObject = Define_S.W_Object.Player;
        State = Define_S.AllState.Idle;

        SetPart();

        Data_Mgr.LoadData();

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "DungeonScene")
        {
            transform.position = new Vector3(0, 0, 3f); // 던전 입구 위치
        }
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "GameScene")
        {
            transform.position = Data_Mgr.m_StartData.m_Pos;
        }
    }

    #endregion

    #region Levelup 
    Coroutine Co_LevelUp;
    public void LevelUpEffect()
    {
        if (Co_LevelUp != null) StopCoroutine(Co_LevelUp);

        Co_LevelUp = StartCoroutine(LevelUpCoroutine());
    }
    IEnumerator LevelUpCoroutine()
    {
        GameObject a_Eff = Instantiate(Resources.Load("SubItem/Effect/LevelupBuff")) as GameObject;

        a_Eff.transform.position = transform.position; // 플레이어 위치에 이펙트 생성(겹쳐서 생성해 이펙트가 보이게끔)

        yield return new WaitForSeconds(4f);

        Destroy(a_Eff);
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
    public bool OnAttack = false;//공격 중인지 여부
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
    Coroutine Co_HitDown;
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
        CurMp -= m_CurSkill.skillConsumMp; // CurMp 프로퍼티를 사용하여 MP 감소

        // MP 바 업데이트
        UI_Mgr.Inst.UpdateMpBar(CurMp, MaxMp);

        // 스킬 이펙트 활성화
        if (m_CurEff != null)
        {
            m_CurEff.SetActive(true);
            // 스킬 이펙트 위치 설정
            m_CurEff.transform.position = transform.position + (transform.forward * 2f);
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
        if (UI_Mgr.Inst.IsPressed || EventSystem.current.IsPointerOverGameObject())
        {
            UI_Mgr.Inst.ResetButtonPress();
            return;
        }

        GetMouseEvent(a_Event);
    }

    float m_MinDist = 0.3f;
    void GetMouseEvent(Define_S.MouseEvent evt)
    {
        if (UI_Mgr.Inst.IsPressed || EventSystem.current.IsPointerOverGameObject())
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
        if (State == Define_S.AllState.Die || EventSystem.current.IsPointerOverGameObject()) return;

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

            Data_Mgr.m_StartData.CurMp -= 5;
            m_Speed = 8f;

            UI_Mgr.Inst.UpdateMpBar(Data_Mgr.m_StartData.CurMp, Data_Mgr.m_StartData.MaxMp);

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
        Collider[] coll = Physics.OverlapSphere(transform.position, a_ItemMaxRadius, 1 << 11);

        // F 키를 누르면 줍기
        if (Input.GetKeyDown(KeyCode.F))
        {
            for (int i = 0; i < coll.Length; i++)
            {
                ItemPickUp a_Item = coll[i].GetComponent<ItemPickUp>();

                if (a_Item != null)
                {
                    //인벤에 넣기
                    if (InvenPopup_UI.Inst.SaveItem(a_Item.m_Item, a_Item.m_ItemCount) == true)
                        Destroy(coll[i].gameObject);

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
        if (Physics.Raycast(transform.position + (Vector3.up * 0.5f), m_Dir, 1.0f, 1 << 9)) // 9 : Block
            return true;

        return false;
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("MonsterWeapon"))
        {
            Monster_Ctrl a_Monster = coll.GetComponent<Monster_Ctrl>();

            if (a_Monster != null)
            {
                MonsterStat a_MonStat = a_Monster.GetComponent<MonsterStat>();
                int a_Dmg = a_MonStat.Attack;
                if (a_MonStat != null)
                    OnHit(a_MonStat, a_Dmg);

            }
        }
        else if (coll.CompareTag("MonsterSkill"))
        {
            Monster_Ctrl a_Monster = coll.GetComponent<Monster_Ctrl>();
            if (a_Monster != null)
            {
                MonsterStat a_MonStat = a_Monster.GetComponent<MonsterStat>();
                int a_Dmg = a_MonStat.Attack * 2;
                if (a_MonStat != null)
                    OnHit(a_MonStat, a_Dmg);
            }
        }
    }

    void OnDie()
    {
        State = Define_S.AllState.Die;

        UI_Mgr.Inst.UpdateHPBar(CurHp, MaxHp);

        UI_Mgr.Inst.DieOn();

        Destroy(gameObject, 4f);
    }
    #endregion


    //장비 설정
    public void SetPart()
    {
        // 캐릭터 파츠 가져오기
        GameObject a_Obj = GameObject.FindWithTag("Player");

        if (a_Obj == null)
        {
            Debug.Log("플레이어를 찾을수없습니다.");
            return;
        }

        // Starter_ 및 Plate_ 접두사를 제거한 후 올바른 데이터 이름으로 매핑하는 딕셔너리
        Dictionary<string, string> itemNameMap = new Dictionary<string, string>
        {
            { "Starter_Boots", "가죽 신발" },
            { "Starter_Chest", "가죽 갑옷" },
            { "Starter_Pants", "가죽 바지" },
            { "Plate_Boots", "철 부츠" },
            { "Plate_Chest", "철제 갑옷" },
            { "Plate_Pants", "철 바지" },
        };

        foreach (Transform a_Child in a_Obj.GetComponentsInChildren<Transform>())
        {
            // 장비 파츠 가져오기
            if (a_Child.CompareTag("Equipment"))
            {
                // 장비 이름 가져오기(매핑)
                string a_ItName = a_Child.name.Replace("Starter_", "").Replace("Plate_", "");
                if (itemNameMap.TryGetValue(a_ItName, out string a_MapName))
                {
                    // 장비 데이터 찾기(매핑)
                    /// system.StringComparison.OrdinalIgnoreCase : 대소문자 구분 없이 비교해주기
                    ArmorItemData a_Armor = Data_Mgr.m_AromrData.Find(ID => ID.ItemName.Equals(a_MapName, System.StringComparison.OrdinalIgnoreCase));

                    if (a_Armor != null)
                    {
                        if (a_Armor.Equipment == null)
                            a_Armor.Equipment = new List<GameObject>();

                        a_Armor.Equipment.Add(a_Child.gameObject);

                        // 플레이어 안에서 장비 파츠 저장
                        List<GameObject> a_EquipList;
                        if (m_Equipment.TryGetValue(a_Armor.Id, out a_EquipList) == false)
                        {
                            a_EquipList = new List<GameObject>();
                            m_Equipment.Add(a_Armor.Id, a_EquipList);
                        }

                        a_EquipList.Add(a_Child.gameObject);

                        a_Child.gameObject.SetActive(false);
                    }
                }
            }

        }


    }


}
