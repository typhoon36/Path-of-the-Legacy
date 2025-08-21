using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;



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
    Image coolDownImage;  // 쿨타임 이미지

    public override void SetInfo()
    {
        BindImage(typeof(Images));
        BindText(typeof(Texts));

        coolDownImage = GetImage((int)Images.CoolDownBlock);
        coolDownImage.gameObject.SetActive(false);

        GetText((int)Texts.MpText).text = "";

        SetEventHandler();

        // 시작할 때 스킬이 현재 키에 장착 중이라면
        if (Managers.Game.SkillBarList.TryGetValue(keySkill, out m_SkillData) == true)
        {
            m_SkillData.SkillSprite = Managers.Data.Skill[m_SkillData.SkillId].SkillSprite;
            SetSkill(m_SkillData);
        }
    }

    void Update()
    {
        UpdateCoolDown();
    }

    protected override void OnEndDragSlot(PointerEventData eventData)
    {
        // 마우스 마지막 드래그 위치가 UI가 아니라면
        if (m_SkillData.IsNull() == false && !EventSystem.current.IsPointerOverGameObject())
        {
            // 현재 전투 중인 몬스터가 없다면 초기화
            if (Managers.Game.currentMonster.IsNull() == true)
                ClearSlot();
        }

        base.OnEndDragSlot(eventData);
    }

    protected override void OnDropSlot(PointerEventData eventData)
    {
        UI_Slot dragSlot = UI_DragSlot.Inst.m_DragSlot;

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

    void ChangeSkill(UI_SkillSlot skillSlot)
    {
        // 스킬 설정
        SetSkill(skillSlot.m_SkillData);

        // 넘어온 스킬의 쿨타임 여부
        IsCoolDown(m_SkillData.IsCoolDown);

        // 기존 슬롯 삭제
        if (skillSlot is UI_SkillBarSlot)
            (skillSlot as UI_SkillBarSlot).ClearSlot();
    }

    void SetSkill(SkillData skill)
    {
        // 궁극기 경우 7렙 이상 스킬만 가능
        if (keySkill == Define.KeySkill.R)
        {
            if (skill.MinLevel < 7)
                return;
        }

        // 기존 스킬 쿨타임 여부
        IsCoolDown(m_SkillData.IsCoolDown);

        m_SkillData = skill;

        GetText((int)Texts.MpText).text = m_SkillData.SkillConsumMp.ToString();

        // 게임 데이터에 스킬 저장
        if (Managers.Game.SkillBarList.ContainsKey(keySkill) == false)
            Managers.Game.SkillBarList.Add(keySkill, m_SkillData);
        else
            Managers.Game.SkillBarList[keySkill] = m_SkillData;

        try
        {
            Icon.sprite = m_SkillData.SkillSprite;
        }
        catch
        {
            Icon.sprite = m_SkillData.SkillSprite = Managers.Data.Skill[m_SkillData.SkillId].SkillSprite;
        }

        SetColor(255);
    }

    // 쿨타임 진행
    void UpdateCoolDown()
    {
        // 쿨타임
        if (m_SkillData.IsNull() == true) return;

        if (m_SkillData.IsCoolDown == true)
        {
            // 쿨타임 객체 활성화
            if (coolDownImage.gameObject.activeSelf == false)
                coolDownImage.gameObject.SetActive(true);

            //설정된 스킬의 이미지로 아이콘 변경
            coolDownImage.sprite = m_SkillData.SkillSprite;

            // 시계 방향으로 밝아지는 fillAmount
            coolDownImage.fillAmount -= 1 * Time.smoothDeltaTime / m_SkillData.SkillCoolDown;

            // fillAmount가 0이 되면 쿨타임 끝
            if (coolDownImage.fillAmount <= 0)
            {
                m_SkillData.IsCoolDown = false;
                coolDownImage.fillAmount = 1;
                coolDownImage.gameObject.SetActive(false);
            }
        }
    }

    // 쿨타임 여부
    void IsCoolDown(bool isTrue)
    {
        coolDownImage.fillAmount = 1;

        m_SkillData.IsCoolDown = isTrue;
        coolDownImage.gameObject.SetActive(isTrue);
    }

    public override void ClearSlot()
    {
        base.ClearSlot();

        // 쿨타임 이미지 초기화
        coolDownImage.fillAmount = 1;
        coolDownImage.gameObject.SetActive(false);

        GetText((int)Texts.MpText).text = "";

        Managers.Game.SkillBarList.Remove(keySkill);
        m_SkillData = null;
    }
}
