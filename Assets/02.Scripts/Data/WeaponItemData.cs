using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public class WeaponItemData : EquipmentData
{
    public Define.WeaponType WeaponType = Define.WeaponType.Unknown;
    
    public int Attack=0;
    public int AddAttack=0;

    [NonSerialized]
    public GameObject charEquipment;

    public WeaponItemData WeaponClone()
    {
        WeaponItemData weapon = new WeaponItemData();

        (this as EquipmentData).EquipmentClone(weapon);

        weapon.WeaponType = this.WeaponType;
        weapon.Attack = this.Attack;
        weapon.charEquipment = this.charEquipment;

        return weapon;
    }
}
