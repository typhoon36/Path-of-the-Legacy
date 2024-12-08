using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * State �� ���� ���� 
 */
public class Define_S : MonoBehaviour
{
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
        Weapon
    }

    public enum WeaponType
    {
        unknown,
        sword
    }

    public enum UseType
    {
        Unknown,
        Hp,
        Mp,
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
        Die
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
        Monster = 8,
        Ground = 9,
        Block = 10,
        Npc = 11,
    }

    public enum UIEvent
    {
        Enter,
        Exit,
        Click,
        DragIng,
        Drag,
        DragEnd,
        Drop,
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
