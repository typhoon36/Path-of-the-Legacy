using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



//UI_SkillPopup.cs에서 사용되며 스킬을 저장한다.

public class UI_SkillPopupSlot : UI_SkillSlot
{
    enum Gameobjects
    {
        LevelBlock,
    }

    enum Texts
    {
        SkillLevelText,
    }

    [SerializeField] int     skillId;

    public override void SetInfo()
    {
        // 자식 객체 불러오기
        BindObject(typeof(Gameobjects));
        BindText(typeof(Texts));

        // 게임데이터에 스킬 아이디 존재 확인
        if (Managers.Data.Skill.TryGetValue(skillId, out SkillData) == false)
            Debug.Log($"SkillData {skillId} : Failed");

        GetText((int)Texts.SkillLevelText).text = SkillData.minLevel.ToString();
        icon.sprite = SkillData.skillSprite;

        // 시작 시 스킬이 흭득 상태인지 확인
        foreach(SkillData a_Skill in Managers.Game.CurrentSkill)
        {
            // 획득 상태면 Lock 해제
            if (skillId == a_Skill.skillId)
            {
                SkillData.isLock = a_Skill.isLock;
                break;
            }
        }

        if (SkillData.isLock == false)
            Managers.Resource.Destroy(GetObject((int)Gameobjects.LevelBlock));
        
        base.SetInfo();
    }

    protected override void OnClickSlot(PointerEventData eventData)
    {
        if (Input.GetMouseButtonUp(1) && SkillData.isLock == true)
        {
            if (LevelCheck() == true)
            {
                UI_ConfirmPopup confirmPopup = Managers.UI.ShowPopupUI<UI_ConfirmPopup>();
                if (confirmPopup.IsNull() == true) return;
                
                confirmPopup.SetInfo(()=>
                {
                    SkillData.isLock = false;
                    Managers.Game.CurrentSkill.Add(this.SkillData);
                    Managers.Resource.Destroy(GetObject((int)Gameobjects.LevelBlock));
                }, Define.SkillOpenMessage);
            }
            else
                Managers.UI.MakeSubItem<UI_Guide>().SetInfo("레벨이 부족합니다.", new Color(1f, 0.5f, 0f));
        }
    }

    protected override void OnBeginDragSlot(PointerEventData eventData)
    {
        if (SkillData.isLock == false)
            base.OnBeginDragSlot(eventData);
    }

    protected override void OnDragSlot(PointerEventData eventData)
    {
        if (SkillData.isLock == false)
            base.OnDragSlot(eventData);
    }

    protected override void OnEndDragSlot(PointerEventData eventData)
    {
        if (SkillData.isLock == false && SkillData.IsNull() == false)
            base.OnEndDragSlot(eventData);
    }

    // 스킬 레벨 체크
     bool LevelCheck() { return Managers.Game.Level >= SkillData.minLevel; }
}
