using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   UseItemData.cs
 * Desc :   소비 아이템 데이터
 *
 & Functions
 &  : UseItem()     - 아이템 사용 (체력 or 마나)
 &  : UseClone()    - 소비 아이템 깊은 복사
 *
 */

[Serializable]
public class UseItemData : ItemData
{
    public Define.UseType UseType = Define.UseType.Unknown;
    public int UseValue = 0;
    public int ItemCount = 0;

    public bool UseItem(ItemData item)
    {
        if ((item is UseItemData) == false)
            return false;

        UseItemData useItem = item as UseItemData;

        if (useItem.UseType == Define.UseType.Hp)
            Managers.Game.Hp += useItem.UseValue;
        else if (useItem.UseType == Define.UseType.Mp)
            Managers.Game.Mp += useItem.UseValue;

        return true;
    }

    public UseItemData UseClone()
    {
        UseItemData useItem = new UseItemData();
        useItem.UseType = this.UseType;
        useItem.UseValue = this.UseValue;
        useItem.ItemCount = this.ItemCount;

        return useItem;
    }
}
