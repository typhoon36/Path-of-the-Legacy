using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public class RewardItem
{
    public int ItemId;
    public int ItemCount;
}

[Serializable]
public class QuestData
{
    public int Id;
    public string TitleName;
    public Define.QuestType QuestType;
    public int MinLevel;
    public int TargetId;
    public int TargetCount;
    public int CurrnetTargetCount;
    public int RewardGold;
    public int RewardExp;
    public List<RewardItem> RewardItems;
    public string Description;
    public string TargetDescription;
    public Vector3 TargetPos;

    public bool IsAccept = false;   // 수락 상태
    public bool IsClear = false;    // 클리어 상태

    // 퀘스트 성공
    public void QuestClear()
    {
        IsClear = true;

        // 보상 지급
        foreach(RewardItem rewardItem in RewardItems)
            Managers.Game.m_PlayScene.Inventory.AcquireItem(Managers.Data.CallItem(rewardItem.ItemId), rewardItem.ItemCount);

        Managers.Game.Gold += RewardGold;
        Managers.Game.Exp += RewardExp;
    }
}
