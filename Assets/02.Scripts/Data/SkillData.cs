using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   SkillData.cs
 * Desc :   스킬 데이터
 */

[Serializable]
public class SkillData
{
    public int SkillId;
    public string SkillName;
    public int MinLevel;
    public int SkillCoolDown;
    public int SkillConsumMp;
    public bool IsCoolDown = false;
    public bool IsLock = true;
    public string Discription;
    public Sprite SkillSprite;
    public List<int> PowerList;
}