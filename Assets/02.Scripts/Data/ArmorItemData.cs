using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public class ArmorItemData : EquipmentData
{
    public Define.ArmorType ArmorType = Define.ArmorType.Unknown;

    // 기본 스탯
    public int Defnece=0;
    public int Hp=0;
    public int Mp=0;
    public int MoveSpeed=0;

    // 강화 시 추가 스탯
    public int AddDefnece=0;
    public int AddHp=0;
    public int AddMp=0;
    public int AddMoveSpeed=0;

    [NonSerialized]
    public List<GameObject> CharEquipment;  // 캐릭터 파츠 활성화

    // 깊은 복사용
    public ArmorItemData ArmorClone()
    {
        ArmorItemData a_Armor = new ArmorItemData();

        (this as EquipmentData).EquipmentClone(a_Armor);

        a_Armor.ArmorType = this.ArmorType;
        a_Armor.Defnece = this.Defnece;
        a_Armor.Hp = this.Hp;
        a_Armor.Mp = this.Mp;
        a_Armor.MoveSpeed = this.MoveSpeed;

        a_Armor.CharEquipment = this.CharEquipment;

        return a_Armor;
    }
}
