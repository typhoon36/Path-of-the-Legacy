using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvenPopup_UI : MonoBehaviour
{
    public GameObject m_InvenPopup;
    public Button m_CloseBtn;
    public RectTransform m_Content;
    public ScrollRect m_ScrollRect;

    Vector3 m_OriginPos;

    public Text m_GoldText;

    ItemData m_ItemData;

    #region Singleton
    public static InvenPopup_UI Inst;
    void Awake()
    {
        Data_Mgr.LoadData(); // 데이터 로드

        if (Inst == null)
            Inst = this;

        m_OriginPos = m_Content.anchoredPosition; // 원래 위치를 저장
    }
    #endregion

    void Start()
    {
        m_InvenPopup.gameObject.SetActive(false);// 인벤토리 팝업 비활성화상태로 시작
        m_CloseBtn.onClick.AddListener(() => m_InvenPopup.SetActive(false));
    }

    void Update()
    {
        RefreshGold(); // 골드 갱신

        if (Input.GetKeyDown(KeyCode.I))
        {
            m_InvenPopup.SetActive(!m_InvenPopup.activeSelf);

            if (m_InvenPopup.activeSelf)
            {
                m_Content.anchoredPosition = m_OriginPos;
                CheckInventoryItems(); // 인벤토리 아이템 확인
            }
            else
            {
                m_Content.anchoredPosition = m_OriginPos;
            }
        }

        //컨트롤키를 누르고 왼쪽마우스 클릭시 아이템 판매
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0) && ShopPopup_UI.Inst.m_ShopPopup.activeSelf == true)
        {
            SellItem(m_ItemData);
        }
    }

    void RefreshGold()
    {
        m_GoldText.text = Data_Mgr.m_StartData.Gold.ToString();
    }

    public void AddGold(int amount)
    {
        Data_Mgr.m_StartData.Gold += amount;
        RefreshGold();
        Data_Mgr.SaveData(); // 골드 값을 저장
    }



    void CheckInventoryItems()
    {
        bool hasItems = false;
        foreach (Transform slot in m_Content)
        {
            foreach (Transform item in slot)
            {
                if (item.gameObject.activeSelf)
                {
                    hasItems = true;
                    break;
                }
            }
            if (hasItems) break;
        }

        // 장비가 없으면 기본 장비로 설정
        if (!hasItems)
        {
            for (int i = 0; i < 6; i++)
            {
                if (i < m_Content.childCount)
                {
                    m_Content.GetChild(i).gameObject.SetActive(true);
                }
            }
        }

        m_ScrollRect.enabled = hasItems; // 스크롤 활성화 여부 설정
    }

    //아이템 추가
    //주소를 받아오기(ReadOnly인 이유는 다른 곳에서 값을 변경하지 못하게 하기 위함)
    static readonly Dictionary<int, string> IconPathMap = new Dictionary<int, string>
    {
        { 1,    "Items/Potions/grass_potion" },
        { 2,    "Items/Potions/wind_potion" },
        { 3,    "Items/Armor/01_Leather_chest" },
        { 4,    "Items/Armor/01_plate_chest" },
        { 5,    "Items/Armor/06_leather_pants" },
        { 6,    "Items/Armor/06_plate_pants" },
        { 7,    "Items/Armor/05_leather_boots" },
        {8,     "Items/Armor/05_plate_boots" },
        {9,     "Items/Weapons/Sword_1" },
        {10,    "Items/Weapons/Sword_2" },
        {11,    "Items/Weapons/Ax_1" },
        {12,    "Items/Weapons/Ax_2" },
        {13,    "Items/Weapons/Ax_3" }
    };

    public void AddItem(ItemData a_ItemData)
    {
        foreach (Transform a_Slot in m_Content)
        {
            if (a_Slot.GetChild(0).gameObject.activeSelf == false)
            {
                // 아이템 추가
                a_Slot.GetChild(0).gameObject.SetActive(true); // 활성화
                // 활성화 후 아이템 정보 설정
                if (IconPathMap.TryGetValue(a_ItemData.Id, out string iconPath))
                {
                    a_Slot.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath); // 아이콘 로드
                }
                break;
            }
        }
    }

    //아이템 판매
    public void SellItem(ItemData a_ItemData)
    {
        foreach (Transform a_Slot in m_Content)
        {
            if (a_Slot.GetChild(0).gameObject.activeSelf == true)
            {
                // 아이템 판매
                a_Slot.GetChild(0).gameObject.SetActive(false); // 비활성화
                // 판매 후 아이템 정보 초기화
                a_Slot.GetChild(0).GetComponent<Image>().sprite = null;
                break;
            }
        }
    }
}
