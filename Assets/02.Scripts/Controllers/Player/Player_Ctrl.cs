using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;



public class Player_Ctrl : Base_Ctrl
{
    // 모든 장비 오브젝트 저장
    public Dictionary<int, List<GameObject>> charEquipment;

    public GameObject   clickMoveEffect;    // 클릭 이동 파티클 Prefab
    public GameObject   currentEffect;      // 현재 이펙트
    public SkillData    currentSkill;       // 현재 스킬

    // Click LayerMask
    private int         _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster) | (1 << (int)Define.Layer.Npc);

    private bool        _stopAttack = true;     // 공격 가능 여부
    private bool        _isDiveRoll = false;    // 구르기 여부
    private bool        _isDown     = false;    // 넘어진 상태 여부
    
    private float       currentDiveTime = 0f;   // 현재 구르는 시간
    private float       _attackCloseTime = 0;   // 공격 취소 시간

    private Vector3     dir;

    [SerializeField]
    private GameObject  rootBone;               // SkinnedMeshRenderer 대표 뼈대

    [SerializeField]
    private GameObject  waeponObjList;          // 무기 Prefab List

    [SerializeField]
    private List<EffectData> effects;           // 이펙트 관리 변수

    [SerializeField] Text m_NameText; // 플레이어 이름 표시



    public override void Init()
    {
        m_Anim = GetComponent<Animator>();

        charEquipment = new Dictionary<int, List<GameObject>>();
        currentEffect = null;

        WorldObjectType = Define.WorldObject.Player;
        State = Define.State.Idle;

        m_NameText.text ="<" + Managers.Game.Name + ">"; // 플레이어 이름 설정


        // 입력 매니저에서 관리
        //Managers.Input.KeyAction -= OnKeyEvent;
        //Managers.Input.KeyAction += OnKeyEvent;
        //Managers.Input.MouseAction -= OnMouseEvent;
        //Managers.Input.MouseAction += OnMouseEvent;

        SetPart();
    }
    

    // 캐릭터 파츠 세팅
    private void SetPart()
    {
        // 캐릭터 파츠 가져오기
        GameObject goChild = Utility.FindChild(gameObject, "Modular_Characters");
        foreach(Transform child in goChild.GetComponentsInChildren<Transform>())
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
                if (charEquipment.TryGetValue(id, out equipList) == false)
                {
                    equipList = new List<GameObject>();
                    charEquipment.Add(id, equipList);
                }

                equipList.Add(child.gameObject);

                child.gameObject.SetActive(false);
            }
        }
        
        // 장착할 무기 객체 아이템 안에 저장
        foreach(Transform child in waeponObjList.transform)
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
    private void SetSkinned(Define.DefaultPart partType, Transform go)
    {
        // SkinnedMeshRenderer 컴포넌트 받기
        SkinnedMeshRenderer objSkinned = go.GetComponent<SkinnedMeshRenderer>();

        SkinnedData skinnedInfo = Managers.Game.DefaultPart[partType];

        // 파츠를 가지고 있는 Model FBX를 찾아 파츠 이름 검색하여 Mesh 받기
        GameObject meshObj = Managers.Resource.Load<GameObject>("Art/PolygonFantasyHeroCharacters/Models/ModularCharacters");

        // SkinnedMeshRenderer 세팅
        objSkinned.sharedMesh = Utility.FindChild<SkinnedMeshRenderer>(meshObj, skinnedInfo.SharedMeshName, true).sharedMesh;
        objSkinned.localBounds = skinnedInfo.Bounds;
        objSkinned.rootBone = Utility.FindChild<Transform>(rootBone, skinnedInfo.RootBoneName, true);

        Transform[] newBones = new Transform[skinnedInfo.Bones.Count];
        for(int i=0; i<skinnedInfo.Bones.Count; i++)
        {
            newBones[i] = Utility.FindChild<Transform>(rootBone, skinnedInfo.Bones[i], true);
        }
        
        objSkinned.bones = newBones;
    }


}