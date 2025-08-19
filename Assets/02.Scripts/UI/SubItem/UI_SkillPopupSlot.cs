using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



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

    [SerializeField] int skillId;

    public override void SetInfo()
    {
        // 자식 객체 불러오기
        BindObject(typeof(Gameobjects));
        BindText(typeof(Texts));


        if (Managers.Data.Skill.TryGetValue(skillId, out skillData) == false)
            Debug.Log($"SkillData {skillId} : Failed");

        GetText((int)Texts.SkillLevelText).text = skillData.MinLevel.ToString();
        icon.sprite = skillData.SkillSprite;


        foreach (SkillData skill in Managers.Game.CurrentSkill)
        {

            if (skillId == skill.SkillId)
            {
                skillData.IsLock = skill.IsLock;
                break;
            }
        }

        if (skillData.IsLock == false)
            Managers.Resource.Destroy(GetObject((int)Gameobjects.LevelBlock));

        base.SetInfo();
    }

    protected override void OnClickSlot(PointerEventData eventData)
    {
        if (Input.GetMouseButtonUp(1) && skillData.IsLock == true)
        {
            if (LevelCheck() == true)
            {
                UI_ConfirmPopup confirmPopup = Managers.UI.ShowPopupUI<UI_ConfirmPopup>();
                if (confirmPopup.IsNull() == true) return;

                confirmPopup.SetInfo(() =>
                {
                    skillData.IsLock = false;
                    Managers.Game.CurrentSkill.Add(this.skillData);
                    Managers.Resource.Destroy(GetObject((int)Gameobjects.LevelBlock));
                }, Define.SkillOpenMessage);
            }
            else
                Managers.UI.MakeSubItem<UI_Guide>().SetInfo("레벨이 부족합니다.", new Color(1f, 0.5f, 0f));
        }
    }

    protected override void OnBeginDragSlot(PointerEventData eventData)
    {
        if (skillData.IsLock == false)
            base.OnBeginDragSlot(eventData);
    }

    protected override void OnDragSlot(PointerEventData eventData)
    {
        if (skillData.IsLock == false)
            base.OnDragSlot(eventData);
    }

    protected override void OnEndDragSlot(PointerEventData eventData)
    {
        if (skillData.IsLock == false && skillData.IsNull() == false)
            base.OnEndDragSlot(eventData);
    }

    // 스킬 레벨 체크
    bool LevelCheck() { return Managers.Game.Level >= skillData.MinLevel; }
}
