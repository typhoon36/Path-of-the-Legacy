using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * State �� ���� ���� 
 */
public class Define_S : MonoBehaviour
{
    public enum DefPart // ĳ���� �⺻ ����, Ŀ����
    {
        Hair,           // ���
        Head,           // �� ����
        Eyebrows,       // ����
        FacialHair,     // ����
        Torso,          // ��ü
        Hips,           // ��ü
    }
    public enum Popup
    {
        Unknown,
        Inventory,
        Equipment,
        Skill,
        Talk,
        Quest,
        Menu,
        Shop,
        Upgrade,
        Max
    }


    public enum KeySkill
    {
        Unknown,
        A,
        D,
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

    public enum WeaponType
    {
        unknown,
        sword
    }

    public enum ItemGrade
    {
        Common,     // �⺻
        Rare,       // ����
        Epic,       // ����
        Legendary,  // ������
    }

    public enum ItemType
    {
        Unknown,
        Use,
        Armor,
        Weapon,
        ETC,
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

    // ����
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

    public enum W_Object//���� �� ������Ʈ�� Ÿ��
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
        Dungeon,
        Boss,
    }

    public enum UIEvent
    {
        Enter,
        Exit,
        Click,
        DragIng,
        Drag,
        DragEnd,
        Drop
    }
    public enum ArmorType
    {
        Unknown,
        Helmet,       // ����
        Chest,      // ����
        Pants,      // ����
        Boots,      // �Ź�
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
}
