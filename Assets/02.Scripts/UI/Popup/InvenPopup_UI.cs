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
        Data_Mgr.LoadData(); // ������ �ε�

        if (Inst == null)
            Inst = this;

        m_OriginPos = m_Content.anchoredPosition; // ���� ��ġ�� ����
        LoadInven(); // �κ��丮 �ε�
    }

    void Start()
    {
        m_InvenPopup.gameObject.SetActive(false);// �κ��丮 �˾� ��Ȱ��ȭ���·� ����
        m_CloseBtn.onClick.AddListener(() => m_InvenPopup.SetActive(false));
    }


    void Update()
    {
        RefreshGold(); // ��� ����

        if (Input.GetKeyDown(KeyCode.I))
        {
            m_InvenPopup.SetActive(!m_InvenPopup.activeSelf);

            if (m_InvenPopup.activeSelf)
            {
                m_Content.anchoredPosition = m_OriginPos;
                CheckInvenItems(); // �κ��丮 ������ Ȯ��
            }
            else
                m_Content.anchoredPosition = m_OriginPos;
        }

        // ��Ʈ��Ű�� ������ ���ʸ��콺 Ŭ���� ������ �Ǹ�
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0) && ShopPopup_UI.Inst.m_ShopPopup.activeSelf == true)
            SellItem(m_ItemData);

        // ���콺 ������ Ŭ���� ����â ����
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
        SaveInven(); // ���� ���� �� �κ��丮 ����
    }

    void RefreshGold()
    {
        m_GoldText.text = Data_Mgr.m_StartData.Gold.ToString();
    }

    public void AddGold(int a_Amount)
    {
        Data_Mgr.m_StartData.Gold += a_Amount;
        RefreshGold();
        Data_Mgr.SaveData(); // ��� ���� ����
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

        // ��� ������ �⺻ ���� ����
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

        m_ScrollRect.enabled = hasItems; // ��ũ�� Ȱ��ȭ ���� ����
    }

    //������ �߰�
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
                // ������ �߰�
                a_Slot.GetChild(0).gameObject.SetActive(true); // Ȱ��ȭ
                // Ȱ��ȭ �� ������ ���� ����
                if (IconPathMap.TryGetValue(a_ItemData.Id, out string a_IconPath))
                {
                    a_Slot.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(a_IconPath); // ������ �ε�
                }

                m_ItemData = a_ItemData;
                SaveInven(); // �κ��丮 ����
                break;
            }
        }
    }

    //������ �Ǹ�
    public void SellItem(ItemData a_ItemData)
    {
        foreach (Transform a_Slot in m_Content)
        {
            if (a_Slot.childCount > 0 && a_Slot.GetChild(0).gameObject.activeSelf == true)
            {
                // ������ �Ǹ�
                a_Slot.GetChild(0).gameObject.SetActive(false); // ��Ȱ��ȭ
                // �Ǹ� �� ������ ���� �ʱ�ȭ
                a_Slot.GetChild(0).GetComponent<Image>().sprite = null;

                AddGold(a_ItemData.ItemPrice); // ��� �߰�
                SaveInven(); // �κ��丮 ����
                break;
            }
        }
    }

    //�κ��丮 ���� ������ ����
    public bool SaveItem(ItemData a_Item, int a_Count = 1)
    {
        //��� ���� Ȯ��
        foreach (Transform a_Slot in m_Content)
        {
            //������ ��������� ������ �߰�
            if (a_Slot.childCount > 0 && a_Slot.GetChild(0).gameObject.activeSelf == false)
            {
                m_ItemData = a_Item;
                AddItem(a_Item);
                return true;
            }
        }

        //�κ��丮�� ���� á�� ��
        return false;
    }

    // �κ��丮 ����
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

    // �κ��丮 �ε�
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
