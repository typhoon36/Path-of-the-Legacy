using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * State 들 공통 정의 
 */
public class Define_S : MonoBehaviour
{
   
    public enum KeySkill
    {
        Unknown,
        A,
        D,
        W,
        R,
    }

    public enum QuestType
    {
        Unknown,
        Talk,
        Monster,
        Item
    }

    public enum ShopType
    {
        Unknown,
        Used,
        Armor,
        Weapon,
        ETC
    }

    public enum ItemGrade
    {
        Common,     // 기본
        Rare,       // 레어
        Epic,       // 에픽
        Legendary,  // 레전드
    }

    public enum ItemType
    {
        Unknown,
        Use,
        Armor,
        Weapon,
        ETC,
    }

    public enum WeaponType
    {
        Unknown,
        Sword,
        Bow,
        Staff,
    }

    public enum UseType
    {
        Unknown,
        Hp,
        Mp,
    }
    public enum MonsterType
    {
        Unknown,
        Normal,
        Boss
    }

    // 상태
    public enum AllState
    {
        Idle,
        Moving,
        Roll,
        Attack,
        Skill,
        Hit,
        Die,
    }

    public enum W_Object//월드 내 오브젝트들 타입
    {
        Unknown,
        Player,
        Monster,
        Item
    }
    public enum Layer
    {
        UIWorldSpace = 6,
        Monster = 7,
        Ground = 8,
        Block = 9,
        Npc = 10,
    }
    public enum Scene
    {
        Unknown,
        Title,
        Game,
        Dungeon
    }

    public enum ArmorType
    {
        Unknown,
        Helmet,       // 모자
        Chest,      // 갑옷
        Pants,      // 바지
        Boots,      // 신발
        MaxCount,
    }

    public enum MouseEvent
    {
        Left,
        Right,
        LeftDown,
        RightDown,
        LeftUp,
        RightUp,
        LeftClick,
        RightClick,
    }

    public const string LoadMsg1 = "미니맵은 던전에서 작동하지않습니다.";
    public const string LoadMsg2 = "상점 열렸을시 Ctrl키를 누르고 클릭시 판매됩니다.";
}
