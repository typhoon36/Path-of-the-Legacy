using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvenPopup_UI : MonoBehaviour
{
    [Header("Inventory")]
    public GameObject m_InvenPopup;
    public Button m_CloseBtn;
    public Text m_GoldText;
    public RectTransform m_Content;
    public ScrollRect m_ScrollRect;

    Vector3 m_OriginPos;
    ItemData m_ItemData;

    [Header("ItemDesc")]
    public GameObject m_ItemDesc;


    public static InvenPopup_UI Inst;
    void Awake()
    {
        Data_Mgr.LoadData(); // 데이터 로드

        if (Inst == null)
            Inst = this;

        m_OriginPos = m_Content.anchoredPosition; // 원래 위치를 저장
        LoadInven(); // 인벤토리 로드
    }

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
                CheckInvenItems(); // 인벤토리 아이템 확인
            }
            else
                m_Content.anchoredPosition = m_OriginPos;
        }

        // 컨트롤키를 누르고 왼쪽마우스 클릭시 아이템 판매
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0) && ShopPopup_UI.Inst.m_ShopPopup.activeSelf == true)
            SellItem(m_ItemData);

        // 마우스 오른쪽 클릭시 설명창 스폰
        if (Input.GetMouseButtonDown(1) && m_InvenPopup.activeSelf == true)
        {
            Vector2 a_MousePos = Input.mousePosition;
            foreach (Transform slot in m_Content)
            {
                RectTransform a_SlotRect = slot.GetComponent<RectTransform>();
                if (RectTransformUtility.RectangleContainsScreenPoint(a_SlotRect, a_MousePos))
                {
                    Image Icon = slot.GetChild(0).GetComponent<Image>();

                    if (Icon != null && Icon.sprite != null)
                        foreach (var a_Key in IconPathMap)
                        {
                            if (Resources.Load<Sprite>(a_Key.Value) == Icon.sprite)
                            {
                                m_ItemData = Data_Mgr.CallItem(a_Key.Key);
                                ShowItemDesc(m_ItemData.Id);
                                break;
                            }
                        }

                }
            }
        }
        else if (m_InvenPopup.activeSelf == false) m_ItemDesc.SetActive(false);

    }

    void ShowItemDesc(int a_Id)
    {
        if (Desc_Nd.Inst == null)
        {
            Debug.LogError("Desc_Nd instance is not initialized.");
            return;
        }

        ItemData a_ItemData = Data_Mgr.CallItem(a_Id);
        if (a_ItemData != null)
        {
            Desc_Nd.Inst.m_DescText.text = a_ItemData.ItemDesc;
            Desc_Nd.Inst.m_NameTxt.text = a_ItemData.ItemName +" ["+ a_ItemData.ItemGrade + "]";
            Desc_Nd.Inst.m_Icon.sprite = Resources.Load<Sprite>(a_ItemData.ItemIconPath);
            Desc_Nd.Inst.m_DescObj.SetActive(true);
        }
    }

    void OnApplicationQuit()
    {
        SaveInven(); // 게임 종료 시 인벤토리 저장
    }

    void RefreshGold()
    {
        m_GoldText.text = Data_Mgr.m_StartData.Gold.ToString();
    }

    public void AddGold(int a_Amount)
    {
        Data_Mgr.m_StartData.Gold += a_Amount;
        RefreshGold();
        Data_Mgr.SaveData(); // 골드 값을 저장
    }

    public void CheckInvenItems()
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
    public static readonly Dictionary<int, string> IconPathMap = new Dictionary<int, string>
    {
        { 1,    "Items/Potions/grass_potion" },
        { 2,    "Items/Potions/wind_potion" },
        { 3,    "Items/Armor/01_Leather_chest" },
        { 4,    "Items/Armor/01_plate_chest" },
        { 5,    "Items/Armor/06_leather_pants" },
        { 6,    "Items/Armor/06_plate_pants" },
        { 7,    "Items/Armor/05_leather_boots" },
        { 8,     "Items/Armor/05_plate_boots" },
        { 9,     "Items/Weapons/Sword_1" },
        { 10,    "Items/Weapons/Sword_2" },
        { 11,    "Items/Weapons/Ax_1" },
        { 12,    "Items/Weapons/Ax_2" },
        { 13,    "Items/Weapons/Ax_3" },
        { 14,     "Items/Weapons/Hammer"},
        { 15,     "Items/Weapons/Shield"}
    };

    public void AddItem(ItemData a_ItemData)
    {
        foreach (Transform a_Slot in m_Content)
        {
            if (a_Slot.childCount > 0 && a_Slot.GetChild(0).gameObject.activeSelf == false)
            {
                // 아이템 추가
                a_Slot.GetChild(0).gameObject.SetActive(true); // 활성화
                // 활성화 후 아이템 정보 설정
                if (IconPathMap.TryGetValue(a_ItemData.Id, out string a_IconPath))
                {
                    a_Slot.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(a_IconPath); // 아이콘 로드
                }

                m_ItemData = a_ItemData;
                SaveInven(); // 인벤토리 저장
                break;
            }
        }
    }

    //아이템 판매
    public void SellItem(ItemData a_ItemData)
    {
        foreach (Transform a_Slot in m_Content)
        {
            if (a_Slot.childCount > 0 && a_Slot.GetChild(0).gameObject.activeSelf == true)
            {
                // 아이템 판매
                a_Slot.GetChild(0).gameObject.SetActive(false); // 비활성화
                // 판매 후 아이템 정보 초기화
                a_Slot.GetChild(0).GetComponent<Image>().sprite = null;

                AddGold(a_ItemData.ItemPrice); // 골드 추가
                SaveInven(); // 인벤토리 저장
                break;
            }
        }
    }

    //인벤토리 슬롯 아이템 저장
    public bool SaveItem(ItemData a_Item, int a_Count = 1)
    {
        //모든 슬롯 확인
        foreach (Transform a_Slot in m_Content)
        {
            //슬롯이 비어있으면 아이템 추가
            if (a_Slot.childCount > 0 && a_Slot.GetChild(0).gameObject.activeSelf == false)
            {
                m_ItemData = a_Item;
                AddItem(a_Item);
                return true;
            }
        }

        //인벤토리가 가득 찼을 때
        return false;
    }

    // 인벤토리 저장
    public void SaveInven()
    {
        List<int> Items = new List<int>();
        foreach (Transform a_Slot in m_Content)
        {
            if (a_Slot.childCount > 0 && a_Slot.GetChild(0).gameObject.activeSelf)
            {
                Image itemImage = a_Slot.GetChild(0).GetComponent<Image>();
                if (itemImage != null && itemImage.sprite != null)
                {
                    foreach (var key in IconPathMap)
                    {
                        if (Resources.Load<Sprite>(key.Value) == itemImage.sprite)
                        {
                            Items.Add(key.Key);
                            break;
                        }
                    }
                }
            }
        }
        PlayerPrefs.SetString("Inventory", string.Join(",", Items));
        PlayerPrefs.Save();
    }

    // 인벤토리 로드
    public void LoadInven()
    {
        string a_Data = PlayerPrefs.GetString("Inventory", "");
        if (!string.IsNullOrEmpty(a_Data))
        {
            string[] Items = a_Data.Split(',');
            foreach (string Id in Items)
            {
                if (int.TryParse(Id, out int a_Id))
                {
                    ItemData a_ItData = new ItemData { Id = a_Id };
                    AddItem(a_ItData);
                }
            }
        }
    }


}
