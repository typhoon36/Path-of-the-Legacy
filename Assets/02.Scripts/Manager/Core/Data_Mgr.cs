using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#region Data Structures
[System.Serializable]
public class AllData
{
    public StartData m_StartData;
    public List<ItemData> m_ItemData;
    public List<SkillData> m_SkillData;
    public List<QuestData> m_QuestData;
    public List<TalkData> m_TalkData;
    public List<LevelData> m_LevelData;
    public List<ArmorItemData> m_AromrData;
    public List<EquipItemData> m_EquipData;
    public List<UseItemData> m_UseItemData;
    public List<WeaponItemData> m_WeaponData;
    public List<int> AcceptedQuest;
    public Dictionary<int, List<int>> DropItem;
}

[System.Serializable]
public class StartData
{
    public int Id;
    public int Exp = 100;
    public int Level = 1;
    public int MaxHp = 100;
    public int CurHp = 100;
    public int MaxMp = 100;
    public int CurMp = 100;
    public int ATK = 10;
    public int STR = 2;
    public int Speed = 5;
    public int Int = 2;
    public int Luk = 2;
    public int Gold;
    public int SkillPoint = 5;
    public int StatPoint = 0;
    public Vector3 m_Pos;
}

[System.Serializable]
public class LevelData
{
    public int Level;
    public int TotalExp;
    public int StatPoint;
    public int MaxHp;
    public int MaxMp;
}

#region Items

// 아이템 데이터 구조체
[System.Serializable]
public class ItemData
{
    public int Id;
    public string ItemName;
    public Define_S.ItemType ItemType = Define_S.ItemType.Unknown;
    public Define_S.ItemGrade ItemGrade = Define_S.ItemGrade.Common;
    public int ItemPrice;
    public int ItemMaxCount = 99;
    public GameObject ItemObj;
    public string ItemDesc;
    public string ItemIconPath;
}

#region Item(Use, Armor, Equip) Data
//아이템 사용 데이터 구조체
[System.Serializable]
public class UseItemData : ItemData
{
    public Define_S.UseType useType = Define_S.UseType.Unknown;
    public int UseValue = 0;
    public int ItemCnt = 0;
}

//방어구 아이템 데이터 구조체
[System.Serializable]
public class ArmorItemData : ItemData
{
    public Define_S.ArmorType m_ArmorType = Define_S.ArmorType.Unknown;
    public int defnece = 0;
    public int hp = 0;
    public int mp = 0;
    public int moveSpeed = 0;
    public int addDefnece = 0;
    public int addHp = 0;
    public int addMp = 0;
    public int addMoveSpeed = 0;
    public List<GameObject> Equipment;
}

//무기 아이템 데이터 구조체
[System.Serializable]
public class WeaponItemData : ItemData
{
    public Define_S.WeaponType m_WeaponType = Define_S.WeaponType.Unknown;
    public int attack = 0;
    public int addAttack = 0;
    public GameObject charEquipment;
}

//장비 아이템 데이터 구조체
[System.Serializable]
public class EquipItemData : ItemData
{
    public int MinLevel;
}
#endregion

//보상 아이템 데이터 구조체
[System.Serializable]
public class RewardItemData
{
    public int ItemId;
    public int ItemCount;
}
#endregion


//스킬 데이터 구조체
[System.Serializable]
public class SkillData
{
    public int skillId;
    public string skillName;
    public int minLevel;
    public int skillCoolDown;
    public int skillConsumMp;
    public bool isCoolDown = false;
    public bool isLock = true;
    public string discription;
    public Sprite skillSprite;
    public List<int> powerList;
}

#region Dialogue
[System.Serializable]
public class QuestData
{
    public int Id;
    public string TitleName;
    public Define_S.QuestType QuestType;
    public int MinLevel;
    public int TargetCnt;
    public int TargetId;
    public int CurTargetCnt;
    public int RewardGold;
    public int RewardExp;
    public List<RewardItemData> RewardItems;
    public string Desc;
    public string TargetDesc;
    public Vector3 TargetPos;
    public bool IsAccept = false;
    public bool IsClear = false;

    public void QuestClear()
    {
        IsClear = true;
        Data_Mgr.m_StartData.Gold += RewardGold;
        Data_Mgr.m_StartData.Exp += RewardExp;
    }
}
[System.Serializable]
public class TalkData
{
    public int Id;
    public string BasicsTalk;
    public List<string> QuestStartTalk;
    public string AcceptTalk;
    public string RefusalTalk;
    public string ProcTalk;
    public string ClearTalk;
}
#endregion

#endregion

public class Data_Mgr
{
    public static AllData m_AllData = new AllData();
    public static StartData m_StartData = new StartData();
    #region Items
    public static List<ItemData> m_ItemData = new List<ItemData>();
    public static List<ArmorItemData> m_AromrData = new List<ArmorItemData>();
    public static List<EquipItemData> m_EquipData = new List<EquipItemData>();
    public static List<UseItemData> m_UseItemData = new List<UseItemData>();
    public static List<WeaponItemData> m_WeaponData = new List<WeaponItemData>();

    public static Dictionary<int, List<int>> DropItem { get; private set; } = new Dictionary<int, List<int>>();
    public static Dictionary<int, GameObject> ItemObjects { get; private set; } = new Dictionary<int, GameObject>();
    #endregion

    public static List<LevelData> m_LevelData = new List<LevelData>();
    public static List<SkillData> m_SkillData = new List<SkillData>();
    #region Dialogue
    public static List<QuestData> m_QuestData = new List<QuestData>();
    public static List<TalkData> m_TalkData = new List<TalkData>();
    public static List<int> m_AcceptedQuest = new List<int>();
    #endregion


    public static void SaveData()
    {
        m_AllData.m_StartData = m_StartData;
        m_AllData.m_ItemData = m_ItemData;
        m_AllData.m_SkillData = m_SkillData;
        m_AllData.m_QuestData = m_QuestData;
        m_AllData.m_TalkData = m_TalkData;
        m_AllData.m_LevelData = m_LevelData;
        m_AllData.m_AromrData = m_AromrData;
        m_AllData.m_EquipData = m_EquipData;
        m_AllData.m_WeaponData = m_WeaponData;
        m_AllData.m_UseItemData = m_UseItemData;
        m_AllData.AcceptedQuest = m_AcceptedQuest;
        m_AllData.DropItem = DropItem;

        string a_JsonData = JsonUtility.ToJson(m_AllData, true);
        File.WriteAllText(Application.dataPath + "/Resources/Data/GameData.json", a_JsonData);
    }

    public static void LoadData()
    {
        string filePath = Application.dataPath + "/Resources/Data/GameData.json";
        if (File.Exists(filePath))
        {
            string a_JsonData = File.ReadAllText(filePath);
            m_AllData = JsonUtility.FromJson<AllData>(a_JsonData);

            m_StartData = m_AllData.m_StartData;
            m_ItemData = m_AllData.m_ItemData ?? new List<ItemData>();
            m_SkillData = m_AllData.m_SkillData ?? new List<SkillData>();
            m_QuestData = m_AllData.m_QuestData ?? new List<QuestData>();
            m_TalkData = m_AllData.m_TalkData ?? new List<TalkData>();
            m_LevelData = m_AllData.m_LevelData ?? new List<LevelData>();
            m_AromrData = m_AllData.m_AromrData ?? new List<ArmorItemData>();
            m_WeaponData = m_AllData.m_WeaponData ?? new List<WeaponItemData>();
            m_EquipData = m_AllData.m_EquipData ?? new List<EquipItemData>();
            m_UseItemData = m_AllData.m_UseItemData ?? new List<UseItemData>();
            m_AcceptedQuest = m_AllData.AcceptedQuest ?? new List<int>();
            DropItem = m_AllData.DropItem ?? new Dictionary<int, List<int>>();

            // 수락된 퀘스트 상태 업데이트
            foreach (var a_Quest in m_QuestData)
            {
                if (m_AcceptedQuest.Contains(a_Quest.Id))
                    a_Quest.IsAccept = true;
            }

            // 기본 스킬 데이터 초기화
            if (m_SkillData.Count == 0)
            {
                InitSkillData();
            }

            // 기본 퀘스트 데이터 초기화
            if (m_QuestData.Count == 0)
            {
                InitQuestData();
            }

            // 기본 대화 데이터 초기화
            if (m_TalkData.Count == 0)
            {
                InitTalkData();
            }

            // 기본 아이템 데이터 초기화
            InitItemData();
            // 기본 드랍 아이템 데이터 초기화
            InitDropData();
        }
        else
        {
            Debug.LogError("GameData.json 파일을 찾을 수 없습니다.");
        }
    }

    public static ItemData CallItem(int a_Id)
    {
        return m_ItemData.Find(i => i.Id == a_Id);
    }

    #region Dialogue
    public static void AcceptQuest(int a_Id)
    {
        if (!m_AcceptedQuest.Contains(a_Id))
            m_AcceptedQuest.Add(a_Id);
    }

    public static void CompleteQuest(int a_Id)
    {
        if (m_AcceptedQuest.Contains(a_Id))
            m_AcceptedQuest.Remove(a_Id);
    }
    #endregion


    static void InitSkillData()
    {
        m_SkillData.Add(new SkillData
        {
            skillId = 1,
            skillName = "Sheild",
            minLevel = 1,
            skillCoolDown = 25,
            skillConsumMp = 10,
            isCoolDown = false,
            isLock = false,
            discription = "Warrior`s auror gave Damage Reduce.",
            skillSprite = null,
            powerList = new List<int> { 10, 20, 30 }
        });

        m_SkillData.Add(new SkillData
        {
            skillId = 2,
            skillName = "Spin Slice",
            minLevel = 1,
            skillCoolDown = 7,
            skillConsumMp = 10,
            isCoolDown = false,
            isLock = true,
            discription = "Enermy gave Damage",
            skillSprite = null,
            powerList = new List<int> { 15, 25, 35 }
        });
    }

    static void InitQuestData()
    {
        m_QuestData.Add(new QuestData
        {
            Id = 1,
            TitleName = "트롤 처치",
            QuestType = Define_S.QuestType.Monster,
            MinLevel = 1,
            TargetCnt = 10,
            TargetId = 3, 
            CurTargetCnt = 0,
            RewardGold = 100,
            RewardExp = 100,
            RewardItems = new List<RewardItemData> { new RewardItemData { ItemId = 1, ItemCount = 1 } },
            Desc = "트롤 저지",
            TargetDesc = "10마리 사냥.",
            TargetPos = Vector3.zero,
            IsAccept = false,
            IsClear = false
        });
        m_QuestData.Add(new QuestData
        {
            Id = 2,
            TitleName = "홉고블린 소탕",
            QuestType = Define_S.QuestType.Monster,
            MinLevel = 1,
            TargetId = 1, 
            TargetCnt = 10,
            CurTargetCnt = 0,
            RewardGold = 100,
            RewardExp = 100,
            RewardItems = new List<RewardItemData> { new RewardItemData { ItemId = 2, ItemCount = 1 } },
            Desc = "홉고블린 저지",
            TargetDesc = "10마리 사냥.",
            TargetPos = Vector3.zero,
            IsAccept = false,
            IsClear = false
        });
    }

    static void InitTalkData()
    {
        m_TalkData.Add(new TalkData
        {
            Id = 1,
            BasicsTalk = "안녕하신가",
            QuestStartTalk = new List<string> { "자네에게 부탁이 있네." },
            AcceptTalk = "부탁하네",
            RefusalTalk = "아쉽군",
            ProcTalk = "행운을 비네",
            ClearTalk = "고맙네."
        });
        m_TalkData.Add(new TalkData
        {
            Id = 2,
            BasicsTalk = "안녕하신가",
            QuestStartTalk = new List<string> { "자네에게 다른 부탁이 있네." },
            AcceptTalk = "부탁하네",
            RefusalTalk = "아쉽군",
            ProcTalk = "행운을 비네",
            ClearTalk = "벌써 온건가?고맙군."
        });
    }

    /*<Items*/
    public static void InitItemData()
    {
        // 포션 종류
        AddItemData(1, "회복의 포션", Define_S.ItemType.Use, Define_S.ItemGrade.Common, 100, 99, null, "서서히 회복되는 포션(지속시간 3분)", "Items/Potions/grass_potion");
        AddItemData(2, "이동의 포션", Define_S.ItemType.Use, Define_S.ItemGrade.Common, 150, 99, null, "바람처럼 빠르게 움직일수있게한다.", "Items/Potions/wind_potion");

        // 갑옷 종류
        AddItemData(3, "가죽 갑옷", Define_S.ItemType.Armor, Define_S.ItemGrade.Common, 200, 1, null, "방어력 10 증가", "Items/Armor/01_Leather_chest");
        AddItemData(4, "철제 갑옷", Define_S.ItemType.Armor, Define_S.ItemGrade.Rare, 300, 1, null, "방어력 20 증가", "Items/Armor/01_plate_chest");
        AddItemData(5, "가죽 바지", Define_S.ItemType.Armor, Define_S.ItemGrade.Common, 200, 1, null, "방어력 10 증가", "Items/Armor/06_leather_pants");
        AddItemData(6, "철 바지", Define_S.ItemType.Armor, Define_S.ItemGrade.Rare, 300, 1, null, "방어력 20 증가", "Items/Armor/06_plate_pants");

        // 신발 종류
        AddItemData(7, "가죽 신발", Define_S.ItemType.Armor, Define_S.ItemGrade.Common, 200, 1, null, "이동속도 10 증가, 방어력 5증가", "Items/Armor/05_leather_boots");
        AddItemData(8, "철 부츠", Define_S.ItemType.Armor, Define_S.ItemGrade.Rare, 300, 1, null, "이동속도 20 감소,방어력 10 증가", "Items/Armor/05_plate_boots");

        // 무기 종류
        AddItemData(9, "연습용 칼", Define_S.ItemType.Weapon, Define_S.ItemGrade.Common, 10, 1, null, "공격력 10 증가", "Items/Weapons/Sword_1");
        AddItemData(10, "기사의 검", Define_S.ItemType.Weapon, Define_S.ItemGrade.Rare, 100, 1, LoadPrefab("Items/Roots/Warriors_Sword"), "공격력 20 증가", "Items/Weapons/Sword_2");
        AddItemData(11, "무딘 도끼", Define_S.ItemType.Weapon, Define_S.ItemGrade.Common, 150, 1, LoadPrefab("Items/Roots/Axe_1"), "공격력 25 증가", "Items/Weapons/Ax_1");
        AddItemData(12, "황금 도끼", Define_S.ItemType.Weapon, Define_S.ItemGrade.Rare, 250, 1, LoadPrefab("Items/Roots/Axe_2"), "공격력 30 증가", "Items/Weapons/Ax_2");
        AddItemData(13, "전투 도끼", Define_S.ItemType.Weapon, Define_S.ItemGrade.Rare, 350, 1, null, "공격력 45 증가", "Items/Weapons/Ax_3");
        AddItemData(14, "낡은 해머", Define_S.ItemType.Weapon, Define_S.ItemGrade.Epic, 550, 1, LoadPrefab("Items/Roots/Hammer"), "공격력 55 증가", "Items/Weapons/Hammer");
        AddItemData(15, "낡은 방패", Define_S.ItemType.Weapon, Define_S.ItemGrade.Common, 150, 1, LoadPrefab("Items/Roots/Sheild"), "방어력 10 증가", "Items/Weapons/Shield");
    }

    static void AddItemData(int Id, string a_Name, Define_S.ItemType a_Type,
      Define_S.ItemGrade a_Grade, int a_Price, int a_MaxCount,
      GameObject a_Obj, string a_Desc, string a_IconPath)
    {
        m_ItemData.Add(new ItemData
        {
            Id = Id,
            ItemName = a_Name,
            ItemType = a_Type,
            ItemGrade = a_Grade,
            ItemPrice = a_Price,
            ItemMaxCount = a_MaxCount,
            ItemObj = a_Obj,
            ItemDesc = a_Desc,
            ItemIconPath = a_IconPath
        });
    }

    /*DropItems*/
    static void InitDropData()
    {
        // 몬스터 1에 대한 드랍 아이템 설정(홉고블린)
        AddDropData(1, 11, LoadPrefab("Items/Roots/Axe_1"), "Items/Roots/Axe_1");
        AddDropData(1, 12, LoadPrefab("Items/Roots/Axe_2"), "Items/Roots/Axe_2");
        AddDropData(1, 14, LoadPrefab("Items/Roots/Hammer"), "Items/Roots/Hammer");

        // 몬스터 2에 대한 드랍 아이템 설정(해골병사)
        AddDropData(2, 10, LoadPrefab("Items/Roots/Warriors_Sword"), "Items/Roots/Warriors_Sword");
        AddDropData(2, 15, LoadPrefab("Items/Roots/Sheild"), "Items/Roots/Sheild");

        // 몬스터 3에 대한 드랍 아이템 설정(오우거)
        AddDropData(3, 10, LoadPrefab("Items/Roots/Warriors_Sword"), "Items/Roots/Warriors_Sword");
        AddDropData(3, 11, LoadPrefab("Items/Roots/Axe_1"), "Items/Roots/Axe_1");
        AddDropData(3, 12, LoadPrefab("Items/Roots/Axe_2"), "Items/Roots/Axe_2");


    }

    static void AddDropData(int MonsterId, int ItemId, GameObject a_Obj, string a_Path)
    {
        if (!DropItem.ContainsKey(MonsterId))
        {
            DropItem[MonsterId] = new List<int>();
        }
        DropItem[MonsterId].Add(ItemId);

        // 아이템 오브젝트를 저장하는 딕셔너리 추가
        if (!ItemObjects.ContainsKey(ItemId))
        {
            ItemObjects[ItemId] = a_Obj;
        }
    }

    //Prefab
    static GameObject LoadPrefab(string path)
    {
        return Resources.Load<GameObject>(path);
    }



}
