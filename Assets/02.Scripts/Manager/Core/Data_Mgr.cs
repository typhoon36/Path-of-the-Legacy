using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;
using I18N.Common;

#region Data Structure
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
}
[System.Serializable]
public class StartData
{
    public int Id;
    public int Exp = 100;
    public int Level = 1;
    public int MaxHp;
    public int CurHp;
    public int MaxMp;
    public int CurMp;
    public int ATK = 10;
    public int STR = 2;
    public int Speed = 5;
    public int Int = 2;
    public int Luk = 2;
    public int Gold;
    public int SkillPoint = 5;
    public int StatPoint = 5;
    public Vector3 m_Pos;
}
//아이템 데이터 구조체
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
    public Sprite ItemIcon;
}
[System.Serializable]
public class UseItemData
{
    public Define_S.UseType useType = Define_S.UseType.Unknown;
    public int UseValue = 0;
    public int ItemCnt = 0;
}

[System.Serializable]
public class ArmorItemData
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
}

[System.Serializable]
public class EquipItemData
{
    public int MinLevel;
}


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
[System.Serializable]
public class QuestData
{
    public int Id;
    public string TitleName;
    public Define_S.QuestType QuestType;
    public int MinLevel;
    public int TargetCnt;
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
public class RewardItemData
{
    public int ItemId;
    public int ItemCount;
}
[System.Serializable]
public class TalkData
{
    public int id;
    public string basicsTalk;
    public List<string> questStartTalk;
    public string acceptTalk;
    public string refusalTalk;
    public string procTalk;
    public string clearTalk;
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
#endregion

public class Data_Mgr
{
    public static AllData m_AllData = new AllData();
    public static StartData m_StartData = new StartData();
    public static List<ItemData> m_ItemData = new List<ItemData>();
    public static List<SkillData> m_SkillData = new List<SkillData>();
    public static List<QuestData> m_QuestData = new List<QuestData>();
    public static List<TalkData> m_TalkData = new List<TalkData>();
    public static List<LevelData> m_LevelData = new List<LevelData>();
    public static List<ArmorItemData> m_AromrData = new List<ArmorItemData>();
    public static List<EquipItemData> m_EquipData = new List<EquipItemData>();
    public static List<UseItemData> m_UseItemData = new List<UseItemData>();

    public static event Action OnLevelUp;

    public static void LoadData()
    {
        string filePath = Application.dataPath + "/Resources/Data/GameData.json";
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            m_AllData = JsonUtility.FromJson<AllData>(jsonData);

            m_StartData = m_AllData.m_StartData;
            m_ItemData = m_AllData.m_ItemData ?? new List<ItemData>();
            m_SkillData = m_AllData.m_SkillData ?? new List<SkillData>();
            m_QuestData = m_AllData.m_QuestData ?? new List<QuestData>();
            m_TalkData = m_AllData.m_TalkData ?? new List<TalkData>();
            m_LevelData = m_AllData.m_LevelData ?? new List<LevelData>();
            m_AromrData = m_AllData.m_AromrData ?? new List<ArmorItemData>();
            m_EquipData = m_AllData.m_EquipData ?? new List<EquipItemData>();
            m_UseItemData = m_AllData.m_UseItemData ?? new List<UseItemData>();

            // 기본 스킬 데이터 초기화
            if (m_SkillData.Count == 0)
            {
                InitializeDefaultSkillData();
            }
        }
    }

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
        m_AllData.m_UseItemData = m_UseItemData;

        string jsonData = JsonUtility.ToJson(m_AllData, true);
        File.WriteAllText(Application.dataPath + "/Resources/Data/GameData.json", jsonData);
    }

    public static void LevelUp()
    {
        m_StartData.Level++;
        OnLevelUp?.Invoke();
    }

    private static void InitializeDefaultSkillData()
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
            minLevel = 2,
            skillCoolDown = 7,
            skillConsumMp = 35,
            isCoolDown = false,
            isLock = true,
            discription = "Enermy gave Damage",
            skillSprite = null, 
            powerList = new List<int> { 15, 25, 35 }
        });
    }
}
