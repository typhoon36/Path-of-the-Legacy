using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 무기 아이템 데이터 (공격력)
[Serializable]
public class WeaponItemData : EquipmentData
{
    public Define.WeaponType weaponType = Define.WeaponType.Unknown;
    
    public int attack=0;
    public int addAttack=0;

    [NonSerialized]
    public GameObject charEquipment;

    public WeaponItemData WeaponClone()
    {
        WeaponItemData weapon = new WeaponItemData();

        (this as EquipmentData).EquipmentClone(weapon);

        weapon.weaponType = this.weaponType;
        weapon.attack = this.attack;
        weapon.charEquipment = this.charEquipment;

        return weapon;
    }
}
