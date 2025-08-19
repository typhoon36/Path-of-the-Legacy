using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.UI;



public class Player_Ctrl : Base_Ctrl
{
    // 모든 장비 오브젝트 저장
    public Dictionary<int, List<GameObject>> m_CharEquipment;

    public GameObject m_ClickMoveObj;    // 클릭 이동 파티클 Prefab
    public GameObject m_CurEffect;      // 현재 이펙트
    public SkillData m_CurSkill;       // 현재 스킬

    [SerializeField] GameObject m_RootBone;

    [SerializeField] GameObject m_WeaponObjs;          // 무기 Prefab List

    [SerializeField] List<EffectData> m_Effects;           // 이펙트 관리 변수

    [SerializeField] Text m_NickText;            // 플레이어 닉네임


    // Click LayerMask
    int m_Mask = (1 << (int)Define.Layer.Ground) |
        (1 << (int)Define.Layer.Monster) |
        (1 << (int)Define.Layer.Npc);

    bool IsStopAtt = true;     // 공격 가능 여부
    bool IsRoll = false;    // 구르기 여부
    bool IsDown = false;    // 넘어진 상태 여부

    float m_CurRollTime = 0f;   // 현재 구르는 시간
    float m_AttCloseTime = 0;   // 공격 취소 시간

    Vector3 m_Dir;


    public override void Init()
    {
        m_Anim = GetComponent<Animator>();

        m_CharEquipment = new Dictionary<int, List<GameObject>>();
        m_CurEffect = null;

        WorldObjectType = Define.WorldObject.Player;
        State = Define.State.Idle;

        m_NickText.text = Managers.Game.Name; // 플레이어 닉네임 설정

        // 입력 매니저에서 관리
        Managers.Input.KeyAction -= OnKeyEvent;
        Managers.Input.KeyAction += OnKeyEvent;
        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;

        //파츠 세팅
        SetPart();
    }

    // 피격 받기 (넘어지는 공격에 호출됨)
    Coroutine HitCo;
    public void OnHitDown(MonsterStat a_Attacker, int a_AddDamge = 0)
    {
        if (IsRoll == true)
            return;

        if (HitCo.IsNull() == false) StopCoroutine(HitCo);
        HitCo = StartCoroutine(HitDown(a_Attacker, a_AddDamge));
    }

    // 레벨업 시 이펙트 발동
    Coroutine LevelUpCo;
    public void LevelUpEffect()
    {
        if (LevelUpCo.IsNull() == false) StopCoroutine(LevelUpCo);
        LevelUpCo = StartCoroutine(LevelUpCoroutine());
    }

    #region State 패턴

    protected override void UpdateIdle() { if (IsStopAtt == false) StopAttack();}

    float a_ScanRange = 1.5f;
    protected override void UpdateMoving()
    {
        // 이동한 곳에 타겟이 있으면 멈추기
        if (m_LockTarget.IsNull() == false)
        {
            float a_Dist = (m_LockTarget.transform.position - transform.position).magnitude;
            if (a_Dist <= a_ScanRange)
            {
                State = Define.State.Idle;

                // 타겟이 NPC라면 상호작용
                if (m_LockTarget.GetComponent<Npc_Ctrl>().IsNull() == false)
                    m_LockTarget.GetComponent<Npc_Ctrl>().GetInteract();

                return;
            }
        }

 
        m_DestPos.y = 0;

        // 타겟과의 거리
        m_Dir = m_DestPos - transform.position;

        // 0.1만큼 가깝다면 멈추기
        if (m_Dir.magnitude < 0.1f)
            State = Define.State.Idle;
        else
        {
            // 가는 중에 벽이 있으면 멈추기
            if (BlockCheck() == true)
            {
                if (Input.GetMouseButton(0) == false)
                    State = Define.State.Idle;

                return;
            }

            // 회전
            float a_MoveDist = Mathf.Clamp(Managers.Game.MoveSpeed * Time.deltaTime, 0, m_Dir.magnitude);

            transform.position += m_Dir.normalized * a_MoveDist;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_Dir), 20f * Time.deltaTime);
        }
    }

    float diveTime = 0.8f;
    protected override void UpdateDiveRoll()
    {
        // 구르기 타이머
        m_CurRollTime += Time.deltaTime;
        if (m_CurRollTime >= diveTime)
        {
            ClearDiveRoll();
            return;
        }

        // 공격 중지
        StopAttack();

        // 도착 위치 받기
        m_DestPos = GetMousePoint();
        float moveDist = Mathf.Clamp(Managers.Game.MoveSpeed * Time.deltaTime, 0, m_Dir.magnitude);

        // 이동
        transform.position += m_Dir.normalized * moveDist;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_Dir), 20f * Time.deltaTime);

        // 벽 확인
        if (BlockCheck() == true)
        {
            IsRoll = false;
            Managers.Game.MoveSpeed = 5;
            return;
        }
    }

    protected override void UpdateAttack()
    {
        m_AttCloseTime += Time.deltaTime;

        // 공격이 시간이 끝나면 종료 || 공격 했는데 가만히 있다면
        if ((m_Anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") == true &&
            m_AttCloseTime > 0.94f && _onComboAttack == false) ||
            (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") == true &&
             m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.2f))
        {
            StopAttack();
            State = Define.State.Idle;
            return;
        }
    }

    #endregion

    #region 마우스 입력

    // 마우스 클릭
    void OnMouseEvent(Define.MouseEvent evt)
    {
        switch (State)
        {
            case Define.State.Moving:
                GetMouseEvent(evt);
                break;
            case Define.State.Idle:
                GetMouseEvent(evt);
                break;
            case Define.State.Attack:
                GetMouseEvent(evt);
                break;
        }
    }

    float minDistance = 0.3f;
    void GetMouseEvent(Define.MouseEvent evt)
    {
        // 메인 카메라에서 마우스가 가르키는 위치의 ray를 저장
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool raycastHit = Physics.Raycast(ray, out m_Hit, 150f, m_Mask);

        // 자신 캐릭터 클릭 시 진행 X
        float distance = (m_Hit.point - transform.position).magnitude;
        if (distance <= minDistance)
            return;

        switch (evt)
        {
            // 마우스를 클릭했을 때 [ 클릭한 위치로 이동 ]
            case Define.MouseEvent.RightDown:
                {
                    m_DestPos = m_Hit.point;   // 해당 좌표 저장
                    if (raycastHit && IsStopAtt)
                    {
                        State = Define.State.Moving;

                        // 클릭 장소에 파란 원 활성화시키기
                        m_ClickMoveObj.SetActive(false);
                        m_ClickMoveObj.SetActive(true);
                        m_ClickMoveObj.transform.position = m_DestPos;

                        // 클릭 위치에 타겟이 있다면 저장
                        if (m_Hit.collider.gameObject.layer == (int)Define.Layer.Npc)
                            m_LockTarget = m_Hit.collider.gameObject;
                        else
                            m_LockTarget = null;
                    }
                }
                break;
            // 마우스를 클릭 중일 때
            case Define.MouseEvent.RightPress:
                {
                    // 공격 상태가 아니라면
                    if (IsStopAtt == true)
                    {
                        // 멈추고 있으면 움직이기
                        if (State == Define.State.Idle)
                            State = Define.State.Moving;

                        if (m_LockTarget.IsNull() == false)
                            m_DestPos = m_LockTarget.transform.position;
                        else if (raycastHit)
                            m_DestPos = m_Hit.point;
                    }
                }
                break;
            // 왼쪽 클릭 시 공격
            case Define.MouseEvent.LeftDown:
                {
                    // 무기가 있다면 공격 가능
                    if (Managers.Game.CurrentWeapon.IsNull() == false)
                    {
                        IsStopAtt = false;
                        m_DestPos = m_Hit.point;
                        m_DestPos.y = 0;
                        OnAttack();
                    }
                }
                break;
            // 왼쪽 누르는 중이면 다음 공격 진행
            case Define.MouseEvent.LeftPress:
                {
                    if (IsStopAtt == false)
                    {
                        m_DestPos = m_Hit.point;
                        m_DestPos.y = 0;
                        OnAttack();
                    }
                }
                break;
        }
    }

    bool IsAttack = false;         // 공격 여부 체크
    bool _onComboAttack = false;    // 콤보 공격 여부 체크
    void OnAttack()
    {
        // 콤보 체크 (공격 중에 다음 공격을 할 것인지?)
        if (IsAttack &&
            m_Anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") &&
            m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.3f)
        {
            _onComboAttack = true;
        }

        // 공격!
        if (IsAttack == false)
        {
            State = Define.State.Attack;
            IsAttack = true;

            // 회전
            m_Dir = m_DestPos - transform.position;
            transform.rotation = Quaternion.LookRotation(GetMousePoint() - transform.position);
        }
    }

    #endregion

    #region 키입력

    // 키보드 클릭
    void OnKeyEvent()
    {
        if (State == Define.State.Die)
            return;

        // 구르지 않을 때 가능
        if (IsRoll == false)
        {
            GetDiveRoll();  // 구르기
            GetSkill();     // 스킬
        }

        GetUseItem();       // 아이템 사용
        GetPickUp();        // 아이템 줍기
    }

    // F Key로 아이템 줍기
    [SerializeField]
    float itemMaxRadius = 5f;
    void GetPickUp()
    {
        // 주변 아이템 탐색
        Collider[] colliders = Physics.OverlapSphere(transform.position, itemMaxRadius, 1 << 12); // 12 : Item

        // F 키를 누르면 줍기
        if (Input.GetKeyDown(KeyCode.F))
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                ItemPickUp _item = colliders[i].GetComponent<ItemPickUp>();
                if (_item.IsNull() == false)
                {
                    // 인벤에 넣기
                    if (Managers.Game.m_PlayScene.Inventory.AcquireItem(_item.m_Item, _item.m_ItemCount) == true)
                        Destroy(colliders[i].gameObject);

                    return;
                }
            }
        }
    }

    // Space Bar로 구르기
    void GetDiveRoll()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (BlockCheck() == true)
                return;

            // 마나 체크
            if (Managers.Game.Mp < 10)
            {
                Managers.UI.MakeSubItem<UI_Guide>().SetInfo("마나가 부족합니다.", Color.blue);
                return;
            }

            IsDown = false;
            IsRoll = true;

            // 넘어진 상태라면 취소시키기
            StopCoroutine(HitDown(null));

            // 도착 좌표
            m_DestPos = GetMousePoint();
            m_Dir = m_DestPos - transform.position;

            Managers.Game.Mp -= 10;
            Managers.Game.MoveSpeed = 8;

            State = Define.State.DiveRoll;

            // 이펙트 취소
            EffectClose();
        }
    }

    // 번호키를 눌러 소비 아이템 사용
    void GetUseItem()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) Managers.Game.m_PlayScene.UsingItem(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) Managers.Game.m_PlayScene.UsingItem(2);
    }

    // 스킬 사용 (Q, W, E, A, S, D, R)
    void GetSkill()
    {
        // 스킬 사용 중이면 x
        if (State == Define.State.Skill || IsDown == true)
            return;

        // 무기가 없으면 스킬 사용 불가
        if (Managers.Game.CurrentWeapon.IsNull() == true)
            return;

        // 스킬 진행
        if (Input.GetKeyDown(KeyCode.Q)) OnSkill(GetSkill(Define.KeySkill.Q));
        else if (Input.GetKeyDown(KeyCode.W)) OnSkill(GetSkill(Define.KeySkill.W));
        else if (Input.GetKeyDown(KeyCode.E)) OnSkill(GetSkill(Define.KeySkill.E));
        else if (Input.GetKeyDown(KeyCode.A)) OnSkill(GetSkill(Define.KeySkill.A));
        else if (Input.GetKeyDown(KeyCode.S)) OnSkill(GetSkill(Define.KeySkill.S));
        else if (Input.GetKeyDown(KeyCode.D)) OnSkill(GetSkill(Define.KeySkill.D));
        else if (Input.GetKeyDown(KeyCode.R)) OnSkill(GetSkill(Define.KeySkill.R));
    }

    // 스킬 진행
    void OnSkill(SkillData skill)
    {
        // Null Check
        if (skill.IsNull() == true)
        {
            Debug.Log("등록된 스킬이 없습니다!");
            return;
        }

        // 쿨타임 확인
        if (skill.IsCoolDown == true)
        {
            Debug.Log("쿨타임 중입니다.");
            return;
        }

        // 마나 확인
        if (skill.SkillConsumMp > Managers.Game.Mp)
        {
            Managers.UI.MakeSubItem<UI_Guide>().SetInfo("마나가 부족합니다.", Color.blue);
            return;
        }

        // 일반 공격 중지
        StopAttack();

        // 마우스 방향으로 회전
        m_DestPos = GetMousePoint();
        m_Dir = m_DestPos - transform.position;
        transform.rotation = Quaternion.LookRotation(m_Dir);

        m_CurSkill = skill;

        // 스킬 이펙트 찾기
        foreach (EffectData effect in m_Effects)
        {
            if (m_CurSkill.SkillId == effect.Id)
            {
                m_CurEffect = effect.gameObject;
                break;
            }
        }

        // 스킬 진행
        State = Define.State.Skill;
        m_Anim.CrossFade("Skill" + m_CurSkill.SkillId, 0.1f, -1, 0);

        m_CurSkill.IsCoolDown = true;
        Managers.Game.Mp -= m_CurSkill.SkillConsumMp;

        // 스킬 이펙트 활성화
        m_CurEffect.SetActive(true);
    }

    #endregion

    // 플레이어 넘어지기
    IEnumerator HitDown(MonsterStat attacker, int addDamge = 0)
    {
        // 플레이어 데미지 반영
        Managers.Game.OnAttacked(attacker, addDamge);

        // 사망 시 중지
        if (State == Define.State.Die)
            yield break;

        State = Define.State.Down;
        IsDown = true;

        // 공격자 바라보기
        Vector3 dir = attacker.transform.position - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_Dir), 1);

        yield return new WaitForSeconds(2f);

        // 구르는게 아니면 Idle 변경
        if (IsRoll == false)
            State = Define.State.Idle;

        IsDown = false;
    }

    // 레벨업 이펙트 효과
    IEnumerator LevelUpCoroutine()
    {
        // 레벨업 이펙트 Prefab 생성
        GameObject a_Effect = Managers.Resource.Instantiate("Effect/LevelUpEffect", this.transform);
        a_Effect.transform.localPosition = Vector3.zero;

        yield return new WaitForSeconds(4f);

        Managers.Resource.Destroy(a_Effect);
    }

    // 마우스 Ray 위치 반환
    Vector3 GetMousePoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out m_Hit, 150f, m_Mask);


        Vector3 a_HitPos = m_Hit.point;
        a_HitPos.y = 0;
        return a_HitPos;
    }

    // 해당 키 스킬 반환 
    SkillData GetSkill(Define.KeySkill keySkill)
    {
        // Scene UI의 스킬바에 스킬이 존재하는지 확인
        if (Managers.Game.SkillBarList.TryGetValue(keySkill, out SkillData skill) == false)
            return null;

        return skill;
    }

    // 전방 벽 체크
    bool BlockCheck()
    {
        // 전방 Block 체크하여 멈추기 (1.0f 거리에서 멈추기)
        if (Physics.Raycast(transform.position + (Vector3.up * 0.5f), m_Dir, 1.0f, 1 << 10)) // 10 : Block
            return true;

        return false;
    }

    #region Anim Event
    // [ Anim Event ]
    // 공격이 끝나면 발동.
    void ExitAttack()
    {
        // 다음 공격 확인
        if (_onComboAttack == true)
        {
            // 공격 진행
            State = Define.State.Attack;
            _onComboAttack = false;

            // 회전
            m_Dir = m_DestPos - transform.position;
            transform.rotation = Quaternion.LookRotation(GetMousePoint() - transform.position);
        }
    }


    // [ Anim Event ]
    // 구르기가 끝나면 발동.
    void EventDiveRoll()
    {
        ClearDiveRoll();
    }

    // [ Anim Event ]
    // 스킬 끝날 때 발동
    void EventEndSkill()
    {
        EffectClose();
        ClearDiveRoll();
        State = Define.State.Idle;
    }
    #endregion

    // 공격 중지
    void StopAttack()
    {
        IsAttack = false;
        _onComboAttack = false;
        IsStopAtt = true;
        m_AttCloseTime = 0;
        m_AttNumber = 1;
    }

    // 구르기 초기화
    void ClearDiveRoll()
    {
        IsRoll = false;
        m_CurRollTime = 0f;
        Managers.Game.MoveSpeed = 5;
        State = Define.State.Idle;
    }

    // 스킬 이펙트 비활성화
    void EffectClose()
    {
        if (m_CurEffect.IsFakeNull() == true) return;

        // Effect 비활성화 진행
        m_CurEffect.GetComponent<EffectData>().EffectDisableDelay();
    }

    #region 커스텀 파츠 세팅
    // 캐릭터 파츠 세팅
    void SetPart()
    {
        // 캐릭터 파츠 가져오기
        GameObject goChild = Utility.FindChild(gameObject, "Modular_Character");
        foreach (Transform child in goChild.GetComponentsInChildren<Transform>())
        {
            // 캐릭터의 커스텀 파츠 저장
            if (child.CompareTag("Custom"))
            {
                string result = Regex.Replace(child.name, "Base", "");
                Define.DefaultPart partType = (Define.DefaultPart)System.Enum.Parse(typeof(Define.DefaultPart), result);

                SetSkinned(partType, child);
                continue;
            }

            // 장비 파츠 가져오기
            if (child.CompareTag("Equipment"))
            {
                // 기본 옷이라면 커스텀했던 옷 입혀주기
                if (child.name.Contains("Defualt") == true)
                {
                    string defualtResult = Regex.Replace(child.name, "Defualt", "");
                    defualtResult = Regex.Replace(defualtResult, @"\d", "");
                    Define.DefaultPart partType = (Define.DefaultPart)System.Enum.Parse(typeof(Define.DefaultPart), defualtResult);

                    SetSkinned(partType, child);
                }

                string result = Regex.Replace(child.name, @"\D", "");
                int id = int.Parse(result);

                // 아이템 안에 장비 파츠 저장
                ArmorItemData armor = Managers.Data.Item[id] as ArmorItemData;
                if (armor.CharEquipment.IsNull() == true)
                    armor.CharEquipment = new List<GameObject>();

                armor.CharEquipment.Add(child.gameObject);

                // 플레이어 안에서 장비 파츠 저장
                List<GameObject> equipList;
                if (m_CharEquipment.TryGetValue(id, out equipList) == false)
                {
                    equipList = new List<GameObject>();
                    m_CharEquipment.Add(id, equipList);
                }

                equipList.Add(child.gameObject);

                child.gameObject.SetActive(false);
            }
        }

        // 장착할 무기 객체 아이템 안에 저장
        foreach (Transform child in m_WeaponObjs.transform)
        {
            string result = Regex.Replace(child.name, @"\D", "");
            int id = int.Parse(result);

            // Data 저장
            WeaponItemData weapon = Managers.Data.Item[id] as WeaponItemData;
            weapon.charEquipment = child.gameObject;

            child.gameObject.SetActive(false);
        }
    }

    // SkinnedMeshReaderer 변경
    void SetSkinned(Define.DefaultPart a_PartType, Transform a_Obj)
    {
        // SkinnedMeshRenderer 컴포넌트 받기
        SkinnedMeshRenderer a_ObjSkinned = a_Obj.GetComponent<SkinnedMeshRenderer>();

        SkinnedData a_SkinnedInfo = Managers.Game.DefaultPart[a_PartType];

        // 파츠를 가지고 있는 Model FBX를 찾아 파츠 이름 검색하여 Mesh 받기
        GameObject a_MeshObj = Managers.Resource.Load<GameObject>("Models/ModularCharacters");

        // SkinnedMeshRenderer의 SharedMesh 설정
        a_ObjSkinned.sharedMesh = Utility.FindChild<SkinnedMeshRenderer>(a_MeshObj, a_SkinnedInfo.SharedMeshName, true).sharedMesh;
        a_ObjSkinned.localBounds = a_SkinnedInfo.Bounds;
        a_ObjSkinned.rootBone = Utility.FindChild<Transform>(m_RootBone, a_SkinnedInfo.RootBoneName, true);

        // SkinnedMeshRenderer의 Bones 설정
        Transform[] a_NewBones = new Transform[a_SkinnedInfo.Bones.Count];

        for (int i = 0; i < a_SkinnedInfo.Bones.Count; i++)
            a_NewBones[i] = Utility.FindChild<Transform>(m_RootBone, a_SkinnedInfo.Bones[i], true);
        
        a_ObjSkinned.bones = a_NewBones;
    }
    #endregion
}