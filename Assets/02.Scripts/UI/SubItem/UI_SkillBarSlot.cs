using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


// Scene UI의 하단 퀵슬롯에서 스킬바로 사용되며
// 스킬이 적용될 시 key를 눌러 스킬 사용이 가능.


public class UI_SkillBarSlot : UI_SkillSlot
{
    enum Images
    {
        CoolDownBlock,
        ItemImage,
    }

    enum Texts
    {
        MpText,
    }

    [SerializeField] Define.KeySkill keySkill;       // 입력 key
    Image m_CoolDownImage;  // 쿨타임 이미지

    public override void SetInfo()
    {
        BindImage(typeof(Images));
        BindText(typeof(Texts));

        m_CoolDownImage = GetImage((int)Images.CoolDownBlock);
        m_CoolDownImage.gameObject.SetActive(false);

        GetText((int)Texts.MpText).text = "";

        SetEventHandler();

        // 시작할 때 스킬이 현재 키에 장착 중이라면
        if (Managers.Game.SkillBarList.TryGetValue(keySkill, out SkillData) == true)
        {
            SkillData.skillSprite = Managers.Data.Skill[SkillData.skillId].skillSprite;
            SetSkill(SkillData);
        }
    }

    void Update() { UpdateCoolDown(); }

    protected override void OnEndDragSlot(PointerEventData eventData)
    {
        // 마우스 마지막 드래그 위치가 UI가 아니라면
        if (SkillData.IsNull() == false && !EventSystem.current.IsPointerOverGameObject())
        {
            // 현재 전투 중인 몬스터가 없다면 초기화
            if (Managers.Game.currentMonster.IsNull() == true)
                ClearSlot();
        }

        base.OnEndDragSlot(eventData);
    }

    protected override void OnDropSlot(PointerEventData eventData)
    {
        UI_Slot dragSlot = UI_DragSlot.instance.dragSlotItem;

        if (dragSlot.IsNull() == false)
        {
            // 자기 자신이라면
            if (dragSlot == this)
                return;

            // 스킬 슬롯 확인
            if ((dragSlot is UI_SkillSlot) == false)
                return;

            // 스킬 장착
            ChangeSkill(dragSlot as UI_SkillSlot);
        }
    }

    void ChangeSkill(UI_SkillSlot a_SkillSlot)
    {
        // 스킬 설정
        SetSkill(a_SkillSlot.SkillData);

        // 넘어온 스킬의 쿨타임 여부
        IsCoolDown(SkillData.isCoolDown);

        // 기존 슬롯 삭제
        if (a_SkillSlot is UI_SkillBarSlot)
            (a_SkillSlot as UI_SkillBarSlot).ClearSlot();
    }

    void SetSkill(SkillData a_Skill)
    {
        // 궁극기 경우 5렙 이상 스킬만 가능
        if (keySkill == Define.KeySkill.R)
        {
            if (a_Skill.minLevel < 5) return;
        }

        SkillData = a_Skill; // skillData 먼저 할당

        IsCoolDown(SkillData.isCoolDown); // skillData가 null이 아님


        GetText((int)Texts.MpText).text = SkillData.skillConsumMp.ToString();

        // 게임 데이터에 스킬 저장
        if (Managers.Game.SkillBarList.ContainsKey(keySkill) == false)
            Managers.Game.SkillBarList.Add(keySkill, SkillData);
        else
            Managers.Game.SkillBarList[keySkill] = SkillData;

        try
        {
            icon.sprite = SkillData.skillSprite;
        }
        catch
        {
            icon.sprite = SkillData.skillSprite = Managers.Data.Skill[SkillData.skillId].skillSprite;
        }

        SetColor(255);
    }

    // 쿨타임 진행
    void UpdateCoolDown()
    {
        // 쿨타임
        if (SkillData.IsNull() == true) return;

        if (SkillData.isCoolDown == true)
        {
            // 쿨타임 객체 활성화
            if (m_CoolDownImage.gameObject.activeSelf == false)
                m_CoolDownImage.gameObject.SetActive(true);

            // 시계 방향으로 밝아지는 fillAmount
            m_CoolDownImage.fillAmount -= 1 * Time.smoothDeltaTime / SkillData.skillCoolDown;

            // fillAmount가 0이 되면 쿨타임 끝
            if (m_CoolDownImage.fillAmount <= 0)
            {
                SkillData.isCoolDown = false;
                m_CoolDownImage.fillAmount = 1;
                m_CoolDownImage.gameObject.SetActive(false);
            }
        }
    }

    // 쿨타임 여부
    void IsCoolDown(bool IsTrue)
    {
        m_CoolDownImage.fillAmount = 1;

        SkillData.isCoolDown = IsTrue;
        m_CoolDownImage.gameObject.SetActive(IsTrue);
    }

    public override void ClearSlot()
    {
        base.ClearSlot();

        // 쿨타임 이미지 초기화
        m_CoolDownImage.fillAmount = 1;
        m_CoolDownImage.gameObject.SetActive(false);

        GetText((int)Texts.MpText).text = "";

        Managers.Game.SkillBarList.Remove(keySkill);
        SkillData = null;
    }
}
