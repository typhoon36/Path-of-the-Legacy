using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Networking;


public class DataManager
{
    const string URL =
    "https://docs.google.com/spreadsheets/d/1JrR2gxJniIMkQcb9BAhsMUXlrcaJXhBNavmh5Zs4IpY/edit?usp=sharing";

    public bool IsData = false;

    public StartData Start { get;  set; }
    public Dictionary<int, LevelData> Level { get;  set; }
    public Dictionary<int, SkillData> Skill { get;  set; }
    public Dictionary<int, ItemData> Item { get;  set; }
    public Dictionary<int, List<int>> DropItem { get;  set; }
    public Dictionary<int, GameObject> Monster { get;  set; }
    public Dictionary<int, List<int>> Shop { get;  set; }
    public Dictionary<int, QuestData> Quest { get;  set; }
    public Dictionary<int, TalkData> Talk { get;  set; }
    public Dictionary<int, List<SkinnedData>> Skinned { get;  set; }
    // public Dictionary<int, TextData> Texts { get;  set; }

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

    public void Init()
    {
        Item = new Dictionary<int, ItemData>();
    }

    // 게임 시작 시 호출 (GameScene)
    public IEnumerator DataRequest(string dataNumber)
    {
        UnityWebRequest www = UnityWebRequest.Get(URL+dataNumber);

        yield return www.SendWebRequest();

        string data = www.downloadHandler.text;

        switch(dataNumber)
        {
            case Define.StartNumber:
                StartRequest(data);
                break;
            case Define.LevelNumber:
                LevelRequest(data);
                break;
            case Define.SkillNumber:
                SkillRequest(data);
                break;
            case Define.UseItemNumber:
                UseItemRequest(data);
                break;
            case Define.WeaponItemNumber:
                WeaponItemRequest(data);
                break;
            case Define.ArmorItemNumber:
                ArmorItemRequest(data);
                break;
            case Define.DropItemNumber:
                DropItemRequest(data);
                break;
            case Define.MonsterNumber:
                MonsterRequest(data);
                break;
            case Define.ShopNumber:
                ShopRequest(data);
                break;
            case Define.TalkNumber:
                TalkRequest(data);
                break;
            case Define.QuestNumber:
                QuestRequest(data);
                break;
        }
    }

#region 데이터 파싱

    void StartRequest(string data)
    {
        Start = new StartData();

        string[] lines = data.Split("\n");
        string[] row = lines[1].Replace("\r", "").Split(',');

        Start = new StartData()
        {
            Id = int.Parse(row[0]),
            TotalExp = int.Parse(row[1]),
            Exp = int.Parse(row[2]),
            Level = int.Parse(row[3]),
            MaxHp = int.Parse(row[4]),
            MaxMp = int.Parse(row[5]),
            Str = int.Parse(row[6]),
            MoveSpeed = int.Parse(row[7]),
            Luk = int.Parse(row[8]),
            Gold = int.Parse(row[9]),
        };
    }

    void LevelRequest(string data)
    {
        Level = new Dictionary<int, LevelData>();

        string[] lines = data.Split("\n");
        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

            LevelData levelData = new LevelData()
            {
                level = int.Parse(row[0]),
                TotalExp = int.Parse(row[1]),
                StatPoint = int.Parse(row[2]),
                MaxHp = int.Parse(row[3]),
                MaxMp = int.Parse(row[4]),
            };

            Level.Add(levelData.level, levelData);
        }
    }

    void SkillRequest(string data)
    {
        Skill = new Dictionary<int, SkillData>();

        string[] lines = data.Split("\n");

        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            
            if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

            // 스킬 정보 1~6
            SkillData skillData = new SkillData()
            {
                SkillId = int.Parse(row[0]),
                SkillName = row[1],
                MinLevel = int.Parse(row[2]),
                SkillCoolDown = int.Parse(row[3]),
                SkillConsumMp = int.Parse(row[4]),
                Discription = row[5],
            };

            // Sprite 6
            skillData.SkillSprite = Managers.Resource.Load<Sprite>("Art/UI/Skill/"+row[6]);

            // 공격력 7
            List<int> powerList = new List<int>();
            foreach(string attackNumber in row[7].Split("|"))
                powerList.Add(int.Parse(attackNumber));

            skillData.PowerList = powerList;

            Skill.Add(skillData.SkillId, skillData);
        }
    }

    void UseItemRequest(string data)
    {
        string[] lines = data.Split("\n");
        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

            UseItemData useItem = new UseItemData()
            {
                id = int.Parse(row[0]),
                itemName = row[1],
                UseType = (Define.UseType)int.Parse(row[2]),
                itemGrade = (Define.ItemGrade)int.Parse(row[3]),
                UseValue = int.Parse(row[4]),
                itemPrice = int.Parse(row[5]),
                itemDesc = row[6],
                itemMaxCount = 99,
                itemIcon = Managers.Resource.Load<Sprite>("Art/UI/Item/Use/"+row[7]),
                itemObject = Managers.Resource.Load<GameObject>("Prefabs/Object/Use/"+row[8]),
                itemType = Define.ItemType.Use,
            };

            Item.Add(useItem.id, useItem);
        }
    }

    void WeaponItemRequest(string data)
    {
        string[] lines = data.Split("\n");
        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

            WeaponItemData weaponItem = new WeaponItemData()
            {
                id = int.Parse(row[0]),
                itemName = row[1],
                WeaponType = (Define.WeaponType)int.Parse(row[2]),
                itemGrade = (Define.ItemGrade)int.Parse(row[3]),
                MinLevel = int.Parse(row[4]),
                Attack = int.Parse(row[5]),
                UpgradeValue = int.Parse(row[6]),
                itemPrice = int.Parse(row[7]),
                itemDesc = row[8],
                itemMaxCount = 1,
                itemIcon = Managers.Resource.Load<Sprite>("Art/UI/Item/Weapon/"+row[9]),
                itemObject = Managers.Resource.Load<GameObject>("Prefabs/Object/Weapon/"+row[10]),
                itemType = Define.ItemType.Weapon,
            };

            Item.Add(weaponItem.id, weaponItem);
        }
    }

    void ArmorItemRequest(string data)
    {
        string[] lines = data.Split("\n");
        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

            ArmorItemData armor;
            int id = int.Parse(row[0]);

            // 이미 만들어져 있는지 확인
            if (Item.ContainsKey(id) == true)
                armor = Item[id] as ArmorItemData;
            else
                armor = new ArmorItemData();

            armor.id = int.Parse(row[0]);
            armor.itemName = row[1];
            armor.armorType = (Define.ArmorType)int.Parse(row[2]);
            armor.itemGrade = (Define.ItemGrade)int.Parse(row[3]);
            armor.MinLevel = int.Parse(row[4]);
            armor.UpgradeValue = int.Parse(row[5]);
            armor.itemPrice = int.Parse(row[6]);
            armor.Defnece = int.Parse(row[7]);
            armor.Hp = int.Parse(row[8]);
            armor.Mp = int.Parse(row[9]);
            armor.MoveSpeed = int.Parse(row[10]);
            armor.itemDesc = row[11];
            armor.itemMaxCount = 1;
            armor.itemIcon = Managers.Resource.Load<Sprite>("Art/UI/Item/Armor/"+row[12]);
            armor.itemObject = Managers.Resource.Load<GameObject>("Prefabs/Object/Armor/"+row[13]);
            armor.itemType = Define.ItemType.Armor;

            if (Item.ContainsKey(id) == false)
                Item.Add(armor.id, armor);
        }
    }

    void DropItemRequest(string data)
    {
        DropItem = new Dictionary<int, List<int>>();

        string[] lines = data.Split("\n");

        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            
            if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

            List<int> itemDatas = new List<int>();
            foreach(string itemdata in row[1].Split("|"))
                itemDatas.Add(int.Parse(itemdata));

            DropItem.Add(int.Parse(row[0]), itemDatas);
        }
    }

    void MonsterRequest(string data)
    {
        Monster = new Dictionary<int, GameObject>();

        string[] lines = data.Split("\n");
        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

            GameObject monsterObject = Managers.Resource.Load<GameObject>("Prefabs/Monster/"+row[9]);
            MonsterStat monsterStat = monsterObject.GetOrAddComponent<MonsterStat>();

            monsterStat.Id = int.Parse(row[0]);
            monsterStat.Name = row[1];
            monsterObject.GetComponent<Monster_Ctrl>().monsterType = (Define.MonsterType)int.Parse(row[2]);
            monsterStat.MaxHp = int.Parse(row[3]);
            monsterStat.Attack = int.Parse(row[4]);
            monsterStat.MoveSpeed = int.Parse(row[5]);
            monsterStat.DropExp = int.Parse(row[6]);
            monsterStat.DropGold = int.Parse(row[7]);
            monsterStat.DropItemId = int.Parse(row[8]);

            Monster.Add(monsterStat.Id, monsterObject);
        }
    }

    void ShopRequest(string data)
    {
        Shop = new Dictionary<int, List<int>>();

        string[] lines = data.Split("\n");
        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

            List<int> shopDatas = new List<int>();
            foreach(string itemdata in row[1].Split("|"))
                shopDatas.Add(int.Parse(itemdata));

            Shop.Add(int.Parse(row[0]), shopDatas);
        }
    }

    void QuestRequest(string data)
    {
        Quest = new Dictionary<int, QuestData>();

        string[] lines = data.Split("\n");
        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

            QuestData questData = new QuestData()
            {
                id = int.Parse(row[0]),
                titleName = row[1],
                questType = (Define.QuestType)int.Parse(row[2]),
                minLevel = int.Parse(row[3]),
                targetId = int.Parse(row[4]),
                targetCount = int.Parse(row[5]),
                rewardGold = int.Parse(row[6]),
                rewardExp = int.Parse(row[7]),
                description = row[10],
                targetDescription = row[11]
            };

            // 아이템 보상
            questData.rewardItems = new List<RewardItem>();
            foreach(string itemId in row[8].Split("|"))
                questData.rewardItems.Add(new RewardItem(){ ItemId = int.Parse(itemId) });

            int i=0;
            foreach(string itemCount in row[9].Split("|"))
            {
                questData.rewardItems[i].itemCount = int.Parse(itemCount);
                i++;
            }

            string[] targetPos = row[12].Split("|");
            questData.targetPos = new Vector3(float.Parse(targetPos[0]), float.Parse(targetPos[1]), float.Parse(targetPos[2]));

            Quest.Add(questData.id, questData);
        }
    }

    void TalkRequest(string data)
    {
        Talk = new Dictionary<int, TalkData>();

        string[] lines = data.Split("\n");
        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

            TalkData talkData = new TalkData()
            {
                id = int.Parse(row[0]),
                basicsTalk = row[1],
                acceptTalk = row[3],
                refusalTalk = row[4],
                procTalk = row[5],
                clearTalk = row[6]
            };

            talkData.questStartTalk = new List<string>();
            foreach(string startTalk in row[2].Split("|"))
                talkData.questStartTalk.Add(startTalk);

            Talk.Add(talkData.id, talkData);
        }
    }

#endregion
}
