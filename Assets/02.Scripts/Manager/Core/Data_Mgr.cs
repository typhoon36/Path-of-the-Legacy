using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Data_Mgr
{
    //구글 스프레드시트에서 Csv로 데이터를 가져오기 위한 URL
    const string URL =
    "https://docs.google.com/spreadsheets/d/1JrR2gxJniIMkQcb9BAhsMUXlrcaJXhBNavmh5Zs4IpY/export?format=csv&gid=";

    public bool IsData = false;

    //데이터 딕셔너리
    public StartData Start { get; set; }
    public Dictionary<int, LevelData> Level { get; set; }
    public Dictionary<int, SkillData> Skill { get; set; }
    public Dictionary<int, ItemData> Item { get; set; }
    public Dictionary<int, List<int>> DropItem { get; set; }
    public Dictionary<int, GameObject> Monster { get; set; }
    public Dictionary<int, List<int>> Shop { get; set; }
    public Dictionary<int, QuestData> Quest { get; set; }
    public Dictionary<int, TalkData> Talk { get; set; }
    public Dictionary<int, List<SkinnedData>> Skinned { get; set; }

    // Deep Copy 아이템
    public ItemData CallItem(int itemId)
    {
        if (Item.ContainsKey(itemId) == false)
        {
            Debug.Log("CallItem Failed : " + itemId);
            return null;
        }

        return Item[itemId].ItemClone();
    }

    public void Init() { Item = new Dictionary<int, ItemData>(); }


    //게임 시작시 데이터 요청
    public IEnumerator DataRequest(string a_DataNumber)
    {
        //네트워크 요청(Url뒤에 데이터 번호를 붙여서 요청)
        UnityWebRequest www = UnityWebRequest.Get(URL + a_DataNumber);

        //요청을 보내고 응답을 기다림
        yield return www.SendWebRequest();

        // 요청이 성공했을 경우
        string a_Data = www.downloadHandler.text;


        switch (a_DataNumber)
        {
            case Define.StartNumber:
                StartRequest(a_Data);
                break;
            case Define.LevelNumber:
                LevelRequest(a_Data);
                break;
            case Define.SkillNumber:
                SkillRequest(a_Data);
                break;
            case Define.UseItemNumber:
                UseItemRequest(a_Data);
                break;
            case Define.WeaponItemNumber:
                WeaponItemRequest(a_Data);
                break;
            case Define.ArmorItemNumber:
                ArmorItemRequest(a_Data);
                break;
            case Define.DropItemNumber:
                DropItemRequest(a_Data);
                break;
            case Define.MonsterNumber:
                MonsterRequest(a_Data);
                break;
            case Define.ShopNumber:
                ShopRequest(a_Data);
                break;
            case Define.TalkNumber:
                TalkRequest(a_Data);
                break;
            case Define.QuestNumber:
                QuestRequest(a_Data);
                break;
        }
    }

    #region Parsing Data

    void StartRequest(string a_Data)
    {
        Start = new StartData();

        string[] a_Lines = a_Data.Split("\n");
        string[] a_Row = a_Lines[1].Replace("\r", "").Split(',');

        Start = new StartData()
        {
            Id = int.Parse(a_Row[0]),
            TotalExp = int.Parse(a_Row[1]),
            Exp = int.Parse(a_Row[2]),
            Level = int.Parse(a_Row[3]),
            MaxHp = int.Parse(a_Row[4]),
            MaxMp = int.Parse(a_Row[5]),
            Str = int.Parse(a_Row[6]),
            MoveSpeed = int.Parse(a_Row[7]),
            Luk = int.Parse(a_Row[8]),
            Gold = int.Parse(a_Row[9]),
        };
    }

    void LevelRequest(string a_Data)
    {
        Level = new Dictionary<int, LevelData>();

        string[] a_Lines = a_Data.Split("\n");
        for (int y = 1; y < a_Lines.Length; y++)
        {
            string[] a_Row = a_Lines[y].Replace("\r", "").Split(',');
            if (a_Row.Length == 0)
                continue;
            if (string.IsNullOrEmpty(a_Row[0]))
                continue;

            LevelData a_LevelData = new LevelData()
            {
                level = int.Parse(a_Row[0]),
                TotalExp = int.Parse(a_Row[1]),
                StatPoint = int.Parse(a_Row[2]),
                MaxHp = int.Parse(a_Row[3]),
                MaxMp = int.Parse(a_Row[4]),
            };

            Level.Add(a_LevelData.level, a_LevelData);
        }
    }

    void SkillRequest(string a_Data)
    {
        Skill = new Dictionary<int, SkillData>();

        string[] a_Lines = a_Data.Split("\n");

        for (int y = 1; y < a_Lines.Length; y++)
        {
            string[] a_Row = a_Lines[y].Replace("\r", "").Split(',');

            if (a_Row.Length == 0)
                continue;
            if (string.IsNullOrEmpty(a_Row[0]))
                continue;

            // 스킬 정보 1~6
            SkillData a_SkillData = new SkillData()
            {
                SkillId = int.Parse(a_Row[0]),
                SkillName = a_Row[1],
                MinLevel = int.Parse(a_Row[2]),
                SkillCoolDown = int.Parse(a_Row[3]),
                SkillConsumMp = int.Parse(a_Row[4]),
                Discription = a_Row[5],
            };

            // Sprite 6
            a_SkillData.SkillSprite = Managers.Resource.Load<Sprite>("Art/UI/Skill/" + a_Row[6]);

            // 공격력 7
            List<int> powerList = new List<int>();
            foreach (string attackNumber in a_Row[7].Split("|"))
                powerList.Add(int.Parse(attackNumber));

            a_SkillData.PowerList = powerList;

            Skill.Add(a_SkillData.SkillId, a_SkillData);
        }
    }

    void UseItemRequest(string a_Data)
    {
        string[] a_Lines = a_Data.Split("\n");
        for (int y = 1; y < a_Lines.Length; y++)
        {
            string[] a_Row = a_Lines[y].Replace("\r", "").Split(',');

            if (a_Row.Length == 0) continue;
            
            if (string.IsNullOrEmpty(a_Row[0])) continue;

            UseItemData a_UseItem = new UseItemData()
            {
                Id = int.Parse(a_Row[0]),
                ItemName = a_Row[1],
                UseType = (Define.UseType)int.Parse(a_Row[2]),
                ItemGrade = (Define.ItemGrade)int.Parse(a_Row[3]),
                UseValue = int.Parse(a_Row[4]),
                ItemPrice = int.Parse(a_Row[5]),
                ItemDesc = a_Row[6],
                ItemMaxCount = 99,
                ItemIcon = Managers.Resource.Load<Sprite>("Art/UI/Item/Use/" + a_Row[7]),
                ItemObject = Managers.Resource.Load<GameObject>("Prefabs/Object/Use/" + a_Row[8]),
                ItemType = Define.ItemType.Use,
            };

            Item.Add(a_UseItem.Id, a_UseItem);
        }
    }

    void WeaponItemRequest(string a_Data)
    {
        string[] a_Lines = a_Data.Split("\n");
        for (int y = 1; y < a_Lines.Length; y++)
        {
            string[] a_Row = a_Lines[y].Replace("\r", "").Split(',');
            if (a_Row.Length == 0) continue;
            
            if (string.IsNullOrEmpty(a_Row[0])) continue;

            WeaponItemData a_WeaponItem = new WeaponItemData()
            {
                Id = int.Parse(a_Row[0]),
                ItemName = a_Row[1],
                WeaponType = (Define.WeaponType)int.Parse(a_Row[2]),
                ItemGrade = (Define.ItemGrade)int.Parse(a_Row[3]),
                MinLevel = int.Parse(a_Row[4]),
                Attack = int.Parse(a_Row[5]),
                UpgradeValue = int.Parse(a_Row[6]),
                ItemPrice = int.Parse(a_Row[7]),
                ItemDesc = a_Row[8],
                ItemMaxCount = 1,
                ItemIcon = Managers.Resource.Load<Sprite>("Art/UI/Item/Weapon/" + a_Row[9]),
                ItemObject = Managers.Resource.Load<GameObject>("Prefabs/Object/Weapon/" + a_Row[10]),
                ItemType = Define.ItemType.Weapon,
            };

            Item.Add(a_WeaponItem.Id, a_WeaponItem);
        }
    }

    void ArmorItemRequest(string a_Data)
    {
        string[] a_Lines = a_Data.Split("\n");
        for (int y = 1; y < a_Lines.Length; y++)
        {
            string[] a_Row = a_Lines[y].Replace("\r", "").Split(',');
            if (a_Row.Length == 0) continue;
            if (string.IsNullOrEmpty(a_Row[0])) continue;

            ArmorItemData a_Armor;
            int id = int.Parse(a_Row[0]);

            // 이미 만들어져 있는지 확인
            if (Item.ContainsKey(id) == true) a_Armor = Item[id] as ArmorItemData;
            else a_Armor = new ArmorItemData();

            a_Armor.Id = int.Parse(a_Row[0]);
            a_Armor.ItemName = a_Row[1];
            a_Armor.ArmorType = (Define.ArmorType)int.Parse(a_Row[2]);
            a_Armor.ItemGrade = (Define.ItemGrade)int.Parse(a_Row[3]);
            a_Armor.MinLevel = int.Parse(a_Row[4]);
            a_Armor.UpgradeValue = int.Parse(a_Row[5]);
            a_Armor.ItemPrice = int.Parse(a_Row[6]);
            a_Armor.Defnece = int.Parse(a_Row[7]);
            a_Armor.Hp = int.Parse(a_Row[8]);
            a_Armor.Mp = int.Parse(a_Row[9]);
            a_Armor.MoveSpeed = int.Parse(a_Row[10]);
            a_Armor.ItemDesc = a_Row[11];
            a_Armor.ItemMaxCount = 1;
            a_Armor.ItemIcon = Managers.Resource.Load<Sprite>("Art/UI/Item/Armor/" + a_Row[12]);
            a_Armor.ItemObject = Managers.Resource.Load<GameObject>("Prefabs/Object/Armor/" + a_Row[13]);
            a_Armor.ItemType = Define.ItemType.Armor;

            if (Item.ContainsKey(id) == false)
                Item.Add(a_Armor.Id, a_Armor);
        }
    }

    void DropItemRequest(string a_Data)
    {
        DropItem = new Dictionary<int, List<int>>();

        string[] a_Lines = a_Data.Split("\n");

        for (int y = 1; y < a_Lines.Length; y++)
        {
            string[] a_Row = a_Lines[y].Replace("\r", "").Split(',');

            if (a_Row.Length == 0) continue;

            if (string.IsNullOrEmpty(a_Row[0])) continue;

            List<int> ItemDatas = new List<int>();
            foreach (string itemdata in a_Row[1].Split("|"))
                ItemDatas.Add(int.Parse(itemdata));

            DropItem.Add(int.Parse(a_Row[0]), ItemDatas);
        }
    }

    void MonsterRequest(string a_Data)
    {
        Monster = new Dictionary<int, GameObject>();

        string[] a_Lines = a_Data.Split("\n");
        for (int y = 1; y < a_Lines.Length; y++)
        {
            string[] a_Row = a_Lines[y].Replace("\r", "").Split(',');
            if (a_Row.Length == 0) continue;
            if (string.IsNullOrEmpty(a_Row[0])) continue;

            GameObject a_MonsterObj = Managers.Resource.Load<GameObject>("Prefabs/Monster/" + a_Row[9]);
            MonsterStat a_MonsterStat = a_MonsterObj.GetOrAddComponent<MonsterStat>();

            a_MonsterStat.Id = int.Parse(a_Row[0]);
            a_MonsterStat.Name = a_Row[1];
            a_MonsterObj.GetComponent<Monster_Ctrl>().m_MonsterType = (Define.MonsterType)int.Parse(a_Row[2]);
            a_MonsterStat.MaxHp = int.Parse(a_Row[3]);
            a_MonsterStat.Attack = int.Parse(a_Row[4]);
            a_MonsterStat.MoveSpeed = int.Parse(a_Row[5]);
            a_MonsterStat.DropExp = int.Parse(a_Row[6]);
            a_MonsterStat.DropGold = int.Parse(a_Row[7]);
            a_MonsterStat.DropItemId = int.Parse(a_Row[8]);

            Monster.Add(a_MonsterStat.Id, a_MonsterObj);
        }
    }

    void ShopRequest(string a_Data)
    {
        Shop = new Dictionary<int, List<int>>();

        string[] lines = a_Data.Split("\n");
        for (int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0)
                continue;
            if (string.IsNullOrEmpty(row[0]))
                continue;

            List<int> shopDatas = new List<int>();
            foreach (string itemdata in row[1].Split("|"))
                shopDatas.Add(int.Parse(itemdata));

            Shop.Add(int.Parse(row[0]), shopDatas);
        }
    }

    void QuestRequest(string a_Data)
    {
        Quest = new Dictionary<int, QuestData>();

        string[] a_Lines = a_Data.Split("\n");
        for (int y = 1; y < a_Lines.Length; y++)
        {
            string[] row = a_Lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0)
                continue;
            if (string.IsNullOrEmpty(row[0]))
                continue;

            QuestData a_QuestData = new QuestData()
            {
                Id = int.Parse(row[0]),
                TitleName = row[1],
                QuestType = (Define.QuestType)int.Parse(row[2]),
                MinLevel = int.Parse(row[3]),
                TargetId = int.Parse(row[4]),
                TargetCount = int.Parse(row[5]),
                RewardGold = int.Parse(row[6]),
                RewardExp = int.Parse(row[7]),
                Description = row[10],
                TargetDescription = row[11]
            };

            // 아이템 보상
            a_QuestData.RewardItems = new List<RewardItem>();
            foreach (string itemId in row[8].Split("|"))
                a_QuestData.RewardItems.Add(new RewardItem() { ItemId = int.Parse(itemId) });

            int i = 0;
            foreach (string itemCount in row[9].Split("|"))
            {
                a_QuestData.RewardItems[i].ItemCount = int.Parse(itemCount);
                i++;
            }

            string[] targetPos = row[12].Split("|");
            a_QuestData.TargetPos = new Vector3(float.Parse(targetPos[0]), float.Parse(targetPos[1]), float.Parse(targetPos[2]));

            Quest.Add(a_QuestData.Id, a_QuestData);
        }
    }

    void TalkRequest(string a_Data)
    {
        Talk = new Dictionary<int, TalkData>();

        string[] a_Lines = a_Data.Split("\n");
        for (int y = 1; y < a_Lines.Length; y++)
        {
            string[] a_Row = a_Lines[y].Replace("\r", "").Split(',');
            if (a_Row.Length == 0)
                continue;
            if (string.IsNullOrEmpty(a_Row[0]))
                continue;

            TalkData a_TalkData = new TalkData()
            {
                id = int.Parse(a_Row[0]),
                basicsTalk = a_Row[1],
                acceptTalk = a_Row[3],
                refusalTalk = a_Row[4],
                procTalk = a_Row[5],
                clearTalk = a_Row[6]
            };

            a_TalkData.questStartTalk = new List<string>();
            foreach (string startTalk in a_Row[2].Split("|"))
                a_TalkData.questStartTalk.Add(startTalk);

            Talk.Add(a_TalkData.id, a_TalkData);
        }
    }

    #endregion
}
