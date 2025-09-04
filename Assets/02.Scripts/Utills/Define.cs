using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 모든 enum,정의 관련
public class Define : MonoBehaviour
{
    public enum DefaultPart // 캐릭터 기본 부위, 커스텀
    {
        Hair,           // 헤어
        Eyebrows,       // 눈썹
        FacialHair,     // 수염
        Ears,       // 귀
        Nose,       // 코
    }

    public enum Popup
    {
        Unknown,
        Inventory,
        Equipment,
        SkillUI,
        Talk,
        Quest,
        Menu,
        Shop,
        Upgrade,
        Max,
    }

    public enum MonsterType
    {
        Normal,
        Named,
        Boss,
    }
    
    public enum KeySkill
    {
        Unknown,
        Q,
        W,
        E,
        R,
        A,
        S,
        D,
    }

    public enum QuestType
    {
        Unknown,
        Talk,
        Monster,
        Item,
    }

    public enum ShopType
    {
        Unknown,
        Used,
        Armor,
        Weapon,
        Accessory,
    }

    public enum WeaponType
    {
        Unknown,
        Sword,
    }

    public enum ArmorType
    {
        Unknown,
        Helm,       // 모자
        Chest,      // 갑옷
        Pants,      // 바지
        Boots,      // 신발
        Gloves,     // 장갑
        Necklace,   // 목걸이
        Ring,       // 반지
        MaxCount,
    }

    public enum UseType
    {
        Unknown,
        Hp,
        Mp,
    }

    public enum itemGrade
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

    // 캐릭터 상태
    public enum State
    {
        Moving,
        DiveRoll,
        Idle,
        Hit,
        Down,
        Die,
        Attack,
        Skill,
    }

    public enum WorldObject
    {
        Unknown,
        Player,
        Monster,
        Item,
    }

    public enum Layer
    {
        UIWorldSpace = 6,
        Monster = 8,
        Ground = 9,
        Block = 10,
        Npc = 11,
    }

    public enum Scene
    {
        Unknown,
        Title,
        PlayerCustom,
        Game,
        Dungeon,
        Boss,
    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }

    public enum UIEvent
    {
        Enter,
        Exit,
        Click,
        BeginDrag,
        Drag,
        EndDrag,
        Drop,
    }

    public enum MouseEvent
    {
        LeftPress,
        RightPress,
        LeftDown,
        RightDown,
        LeftUp,
        RightUp,
        LeftClick,
        RightClick,
    }

    public enum CameraMode
    {
        QuarterView,    // 디아블로 게임 같은 시점
    }

    public const string StartNumber = "0";
    public const string LevelNumber = "286381888";
    public const string SkillNumber = "1312613729";
    public const string UseItemNumber = "184335821";
    public const string WeaponItemNumber = "1545559787";
    public const string ArmorItemNumber = "1117444053";
    public const string DropItemNumber = "1910751509";
    public const string MonsterNumber = "215645758";
    public const string ShopNumber = "1781348496";
    public const string TalkNumber = "490479389";
    public const string QuestNumber = "2111092513";

    public const string SkillOpenMessage = "스킬 활성화";
    public const string ShopSaleMessage = "구매 하시겠습니까?";
    public const string DungeonMessage = "던전에 입장하시겠습니까?";

    public const string LoadMessage1 = "장비는 최대 10강까지 강화할 수 있습니다.";
    public const string LoadMessage2 = "5레벨이 되면 강력한 스킬을 사용할 수 있습니다.";
    public const string LoadMessage3 = "미니맵은 던전과 보스전에서 사용불가능합니다.";

    public const string NameRegex = @"^[0-9a-zA-Z가-힣]{2,9}$";
}
