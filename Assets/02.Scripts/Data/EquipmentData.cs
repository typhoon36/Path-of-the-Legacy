using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   EquipmentData.cs
 * Desc :   모든 장비 아이템의 부모
 *
 & Functions
 &  : EquipmentClone()  - 장비 깊은 복사
 *
 */

public class EquipmentData : ItemData
{
    public int MinLevel;            // 최소 장착 레벨
    public int UpgradeValue = 0;    // +1 강화마다 올라가는 수치
    public int UpgradeCount = 0;    // 업그레이드 횟수

    public void EquipmentClone(EquipmentData equip)
    {
        equip.MinLevel = this.MinLevel;
        equip.UpgradeValue = this.UpgradeValue;
        equip.UpgradeCount = this.UpgradeCount;
    }
}
