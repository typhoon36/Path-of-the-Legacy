using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public abstract class ItemData
{
    public int Id;
    public string ItemName;
    public Define.ItemType ItemType = Define.ItemType.Unknown;
    public Define.ItemGrade ItemGrade = Define.ItemGrade.Common;
    public int ItemPrice;
    public int ItemMaxCount = 99;

    public GameObject ItemObject;

    [TextArea] public string ItemDesc;
    
    public Sprite ItemIcon;

    // Deep Copy (깊은 복사)
    public ItemData ItemClone()
    {
        if (this is EquipmentData)
        {
            if (this is ArmorItemData)
            {
                return AddItemValue<ArmorItemData>((this as ArmorItemData).ArmorClone());
            }
            else if (this is WeaponItemData)
            {
                return AddItemValue<WeaponItemData>((this as WeaponItemData).WeaponClone());
            }
        }
        else if (this is UseItemData)
        {
            return AddItemValue<UseItemData>((this as UseItemData).UseClone());
        }

        return null;
    }

    T AddItemValue<T>(T Item) where T : ItemData
    {
        Item.Id = this.Id;
        Item.ItemName = this.ItemName;
        Item.ItemType = this.ItemType;
        Item.ItemGrade = this.ItemGrade;
        Item.ItemPrice = this.ItemPrice;
        Item.ItemMaxCount = this.ItemMaxCount;
        Item.ItemObject = this.ItemObject;
        Item.ItemDesc = this.ItemDesc;
        Item.ItemIcon = this.ItemIcon;

        return Item;
    }
}
