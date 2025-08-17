using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   ArmorItemData.cs
 * Desc :   방어 아이템 스탯 데이터 (방어도, 체력, 마나, 속도)
 *
 & Functions
 &  : ArmorClone()  - 방어구 깊은 복사
 *
 */

[Serializable]
public class ArmorItemData : EquipmentData
{
    public Define.ArmorType armorType = Define.ArmorType.Unknown;

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
        ArmorItemData armor = new ArmorItemData();

        (this as EquipmentData).EquipmentClone(armor);

        armor.armorType = this.armorType;
        armor.Defnece = this.Defnece;
        armor.Hp = this.Hp;
        armor.Mp = this.Mp;
        armor.MoveSpeed = this.MoveSpeed;

        armor.CharEquipment = this.CharEquipment;

        return armor;
    }
}
