using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player_Ctrl : Base_Ctrl
{
    //��� ��� ������Ʈ ����
    public Dictionary<int, List<GameObject>> m_Equipment;

    public GameObject m_ClickMark;
    public GameObject m_CurEff;
    public SkillData m_CurSkill;

    //ClickMark Layer
    int m_Mask = (1 << (int)Define_S.Layer.Ground)
        | (1 << (int)Define_S.Layer.Monster)
        | (1 << (int)Define_S.Layer.Npc);


    public float m_Speed = 5; //�÷��̾� �̵��ӵ�

    [SerializeField]
    public int m_Att = 10; // �÷��̾� ���ݷ� �߰�

    Vector3 m_Dir; //�̵� ����

    public GameObject[] m_SkinnedObjs;//���� ������ ������Ʈ(������ ���̴� ������Ʈ)


    [SerializeField]
    private GameObject m_WeaponList;//���� ����Ʈ

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
            CurHp += delta; // �ִ� ü���� ������ �� ���� ü�µ� �Բ� ����
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
    bool IsStopAtt = true;//���� �������� ����
    bool IsRoll = false;//������ ����
    bool IsHit = false; //�ǰ� ����
    #endregion

    #region Float
    float m_RollTime = 0f;//������ �ð�
    float m_AttTime = 0f;//���� ��� �ð�
    #endregion



    #region Init
    void Awake()
    {
        Cam_Ctrl a_CamCtrl = Camera.main.GetComponent<Cam_Ctrl>();

        if (a_CamCtrl != null)
            a_CamCtrl.InitCam(gameObject);
    }

    //������ ���� ����
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
            transform.position = new Vector3(0, 0, 3f); // ���� �Ա� ��ġ
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

        a_Eff.transform.position = transform.position; // �÷��̾� ��ġ�� ����Ʈ ����(���ļ� ������ ����Ʈ�� ���̰Բ�)

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

                //Ÿ���� NPC��� ��ȣ�ۿ�
                if (m_Target.GetComponent<Npc_Ctrl>() != null)
                    m_Target.GetComponent<Npc_Ctrl>().GetInteract();
                return;
            }

        }

        m_DPos.y = 0;//���̰� 0���� ����

        //Ÿ�ٰ� �Ÿ�
        m_Dir = m_DPos - transform.position;

        //0.1f ���Ϸ� ��������� ����
        if (m_Dir.magnitude < 0.1f)
        {
            State = Define_S.AllState.Idle;
            m_ClickMark.SetActive(false);
        }
        else
        {
            //���� �� ���� ������ ���߱�
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

        //�̵�
        transform.position += m_Dir.normalized * a_MoveDist;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_Dir), 20 * Time.deltaTime);

        //�� Ȯ��
        if (BlockCheck() == true)
            ClearRoll();

    }

    //������ �ʱ�ȭ
    void ClearRoll()
    {
        IsRoll = false;
        m_RollTime = 0f;
        m_Speed = 5f;
        State = Define_S.AllState.Idle;
    }

    //������ �̺�Ʈ
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
    public bool OnAttack = false;//���� ������ ����
    void OnAtt()
    {
        if (OnAttack &&
            m_Anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") &&
            m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.3f)
        {
            OnAttack = true;
        }

        // ����
        if (OnAttack == false)
        {
            State = Define_S.AllState.Attack;
            OnAttack = true;

            // ȸ��
            m_Dir = m_DPos - transform.position;
            transform.rotation = Quaternion.LookRotation(GetMouseRay() - transform.position);
        }
    }

    // ���� ����
    void StopAtt()
    {
        OnAttack = false;
        IsStopAtt = true;
        m_AttTime = 0f;
    }
    // ���� �ִϸ��̼� �̺�Ʈ
    void ExitAttack()
    {
        if (OnAttack == true)
        {
            State = Define_S.AllState.Attack;
            OnAttack = false;

            // ȸ��
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

        // ������ �ٶ󺸱�
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

        // Effect ��Ȱ��ȭ ����
        m_CurEff.GetComponent<EffectData>().EffectDisableDelay();
    }

    //��ų ������ �̺�Ʈ
    void EventSkill()
    {
        EffectClose();
        ClearRoll();
        State = Define_S.AllState.Idle;
    }

    // ��ų ���
    private void OnSkill(SkillData a_Skill)
    {
        // Null Check
        if (a_Skill == null)
        {
            Debug.Log("��ϵ� ��ų�� �����ϴ�!");
            return;
        }

        // ��Ÿ�� Ȯ��
        if (a_Skill.isCoolDown == true)
        {
            Debug.Log("��Ÿ�� ���Դϴ�.");
            return;
        }

        // ���� Ȯ��
        if (a_Skill.skillConsumMp > m_CurMp)
        {
            Debug.Log("������ �����մϴ�.");
            return;
        }

        // �Ϲ� ���� ����
        StopAtt();

        // ���콺 �������� ȸ��
        m_DPos = GetMouseRay();
        m_Dir = m_DPos - transform.position;
        transform.rotation = Quaternion.LookRotation(m_Dir);

        m_CurSkill = a_Skill;

        // ��ų ����Ʈ ã��
        foreach (EffectData a_Effect in m_Effects)
        {
            if (m_CurSkill.skillId == a_Effect.id)
            {
                m_CurEff = a_Effect.gameObject;
                break;
            }
        }

        // �ִϸ��̼� ���� �̸� ����
        string skillAnimName = "Skill" + m_CurSkill.skillId;

        // ��ų ����
        State = Define_S.AllState.Skill;
        m_Anim.CrossFade(skillAnimName, 0.1f);

        m_CurSkill.isCoolDown = true;
        m_CurSkill.skillCoolDown = (int)Time.time; // ��ٿ� ���� �ð� ����
        CurMp -= m_CurSkill.skillConsumMp; // CurMp ������Ƽ�� ����Ͽ� MP ����

        // MP �� ������Ʈ
        UI_Mgr.Inst.UpdateMpBar(CurMp, MaxMp);

        // ��ų ����Ʈ Ȱ��ȭ
        if (m_CurEff != null)
        {
            m_CurEff.SetActive(true);
            // ��ų ����Ʈ ��ġ ����
            m_CurEff.transform.position = transform.position + (transform.forward * 2f);
        }
        else
        {
            Debug.LogWarning("��ų ����Ʈ�� ã�� �� �����ϴ�.");
        }
    }

    // ��ų ��������
    private SkillData GetSkill(Define_S.KeySkill keySkill)
    {
        // Scene UI�� ��ų�ٿ� ��ų�� �����ϴ��� Ȯ��
        if (UI_Mgr.Inst.SkillBarList.TryGetValue(keySkill, out SkillData skill) == false)
            return null;

        return skill;
    }
    #endregion


    #endregion

    #region ���콺 Ŭ��

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

        //�ڽ� Ŭ���� ���� x
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

                        //Ŭ�� ��ġ�� Ÿ���� �ִٸ� ����
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
                    //�������� �ƴ϶��
                    if (IsStopAtt == true)
                    {
                        //���߰� �ִٸ� �̵�
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

    #region Ű�Է�
    protected override void OnKeyEvent()
    {
        if (State == Define_S.AllState.Die || EventSystem.current.IsPointerOverGameObject()) return;

        //������ ������ 
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

            //������ǥ
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
        //��ȣŰ�� ���� ������ ���(�Һ� �������� �ƴ� Ű�� ���� HP, MP ȸ��)
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
    float a_ItemMaxRadius = 5f; // ������ �ݱ� �ִ� �ݰ�
    void GetPickUp()
    {
        Collider[] coll = Physics.OverlapSphere(transform.position, a_ItemMaxRadius, 1 << 11);

        // F Ű�� ������ �ݱ�
        if (Input.GetKeyDown(KeyCode.F))
        {
            for (int i = 0; i < coll.Length; i++)
            {
                ItemPickUp a_Item = coll[i].GetComponent<ItemPickUp>();

                if (a_Item != null)
                {
                    //�κ��� �ֱ�
                    if (InvenPopup_UI.Inst.SaveItem(a_Item.m_Item, a_Item.m_ItemCount) == true)
                        Destroy(coll[i].gameObject);

                    return;
                }

            }
        }
    }

    // ��ų ��� 
    void GetSkill()
    {
        if (State == Define_S.AllState.Skill || IsHit == true) return;

        // ���Ⱑ ������ ��ų ��� �Ұ�
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

    #region üũ
    Vector3 GetMouseRay()
    {
        // ���콺 Ray ���
        Ray a_Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(a_Ray, out m_Hit, 150f, m_Mask);

        // ���콺 ����Ʈ ��ǥ ��ȯ
        Vector3 a_HitPoint = m_Hit.point;
        a_HitPoint.y = 0;
        return a_HitPoint;
    }

    bool BlockCheck()
    {
        // ���� Block üũ�Ͽ� ���߱� 
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


    //��� ����
    public void SetPart()
    {
        // ĳ���� ���� ��������
        GameObject a_Obj = GameObject.FindWithTag("Player");

        if (a_Obj == null)
        {
            Debug.Log("�÷��̾ ã���������ϴ�.");
            return;
        }

        // Starter_ �� Plate_ ���λ縦 ������ �� �ùٸ� ������ �̸����� �����ϴ� ��ųʸ�
        Dictionary<string, string> itemNameMap = new Dictionary<string, string>
        {
            { "Starter_Boots", "���� �Ź�" },
            { "Starter_Chest", "���� ����" },
            { "Starter_Pants", "���� ����" },
            { "Plate_Boots", "ö ����" },
            { "Plate_Chest", "ö�� ����" },
            { "Plate_Pants", "ö ����" },
        };

        foreach (Transform a_Child in a_Obj.GetComponentsInChildren<Transform>())
        {
            // ��� ���� ��������
            if (a_Child.CompareTag("Equipment"))
            {
                // ��� �̸� ��������(����)
                string a_ItName = a_Child.name.Replace("Starter_", "").Replace("Plate_", "");
                if (itemNameMap.TryGetValue(a_ItName, out string a_MapName))
                {
                    // ��� ������ ã��(����)
                    /// system.StringComparison.OrdinalIgnoreCase : ��ҹ��� ���� ���� �����ֱ�
                    ArmorItemData a_Armor = Data_Mgr.m_AromrData.Find(ID => ID.ItemName.Equals(a_MapName, System.StringComparison.OrdinalIgnoreCase));

                    if (a_Armor != null)
                    {
                        if (a_Armor.Equipment == null)
                            a_Armor.Equipment = new List<GameObject>();

                        a_Armor.Equipment.Add(a_Child.gameObject);

                        // �÷��̾� �ȿ��� ��� ���� ����
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
