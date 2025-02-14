using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//장비&스텟 팝업창
public class EqStatPopup_UI : MonoBehaviour
{
    [Header("EquipPopup")]
    public GameObject m_EquipPopup;
    public Button m_EquipCloseBtn;

    //장비슬롯
    public GameObject[] m_EquipSlot;
    public GameObject[] m_ItemObj;
    public Transform m_EquipContent;

    [Header("StatPopup")]
    public GameObject m_StatPopup;
    public Button m_StatCloseBtn;
    public Button m_HPBtn;
    public Button m_STRBtn;
    public Button m_DEXBtn;
    public Button m_INTBtn;
    public Button m_LUKBtn;
    public Text m_HPTxt;
    public Text m_STRTxt;
    public Text m_DEXTxt;
    public Text m_INTTxt;
    public Text m_LUKTxt;
    public Text m_LevelTxt;
    public Text m_StatPointTxt;

    ItemData m_ItemData;

    #region Singleton
    public static EqStatPopup_UI Inst;
    void Awake()
    {
        Inst = this;
    }
    #endregion

    void Start()
    {
        #region EquipPopup
        m_EquipPopup.gameObject.SetActive(false);

        m_EquipCloseBtn.onClick.AddListener(() => { m_EquipPopup.SetActive(false); });
        #endregion

        #region StatPopup
        m_StatPopup.gameObject.SetActive(false);
        m_StatCloseBtn.onClick.AddListener(() => { m_StatPopup.SetActive(false); });
        m_LevelTxt.text = "레벨 : " + Data_Mgr.m_StartData.Level.ToString();

        m_HPTxt.text = "생명력 : " + Data_Mgr.m_StartData.MaxHp.ToString();
        m_STRTxt.text = "근력 : " + Data_Mgr.m_StartData.STR.ToString();
        m_DEXTxt.text = "민첩 : " + Data_Mgr.m_StartData.Speed.ToString();
        m_INTTxt.text = "기억력 : " + Data_Mgr.m_StartData.Int.ToString();
        m_LUKTxt.text = "운 : " + Data_Mgr.m_StartData.Luk.ToString();

        //레벨업하면 스텟 포인트 5증가
        m_StatPointTxt.text = "스텟 포인트 : " + Data_Mgr.m_StartData.StatPoint.ToString();

        m_HPBtn.onClick.AddListener(() =>
        {
            if (Data_Mgr.m_StartData.StatPoint > 0)
            {
                Data_Mgr.m_StartData.MaxHp += 20;
                Data_Mgr.m_StartData.StatPoint -= 1;
                m_StatPointTxt.text = "스텟 포인트 : " + Data_Mgr.m_StartData.StatPoint.ToString();
                Data_Mgr.SaveData();
            }
        });

        m_STRBtn.onClick.AddListener(() =>
        {
            if (Data_Mgr.m_StartData.StatPoint > 0)
            {
                Data_Mgr.m_StartData.STR += 1;
                Data_Mgr.m_StartData.StatPoint -= 1;
                m_StatPointTxt.text = "스텟 포인트 : " + Data_Mgr.m_StartData.StatPoint.ToString();
                m_STRTxt.text = "근력 : " + Data_Mgr.m_StartData.STR.ToString();
                Data_Mgr.SaveData();
            }
        });

        m_DEXBtn.onClick.AddListener(() =>
        {
            if (Data_Mgr.m_StartData.StatPoint > 0)
            {
                Data_Mgr.m_StartData.Speed += 1;
                Data_Mgr.m_StartData.StatPoint -= 1;
                m_StatPointTxt.text = "스텟 포인트 : " + Data_Mgr.m_StartData.StatPoint.ToString();
                m_DEXTxt.text = "민첩 : " + Data_Mgr.m_StartData.Speed.ToString();
                Data_Mgr.SaveData();
            }
        });

        m_INTBtn.onClick.AddListener(() =>
        {
            if (Data_Mgr.m_StartData.StatPoint > 0)
            {
                Data_Mgr.m_StartData.Int += 1;
                Data_Mgr.m_StartData.StatPoint -= 1;
                m_StatPointTxt.text = "스텟 포인트 : " + Data_Mgr.m_StartData.StatPoint.ToString();
                m_INTTxt.text = "기억력 : " + Data_Mgr.m_StartData.Int.ToString();
                Data_Mgr.SaveData();
            }
        });

        m_LUKBtn.onClick.AddListener(() =>
        {
            if (Data_Mgr.m_StartData.StatPoint > 0)
            {
                Data_Mgr.m_StartData.Luk += 1;
                Data_Mgr.m_StartData.StatPoint -= 1;
                m_StatPointTxt.text = "스텟 포인트 : " + Data_Mgr.m_StartData.StatPoint.ToString();
                m_LUKTxt.text = "운 : " + Data_Mgr.m_StartData.Luk.ToString();
                Data_Mgr.SaveData();
            }
        });
        #endregion

        Data_Mgr.LoadData(); // 데이터 로드

        LoadEquip(); // 장비 로드
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            m_EquipPopup.SetActive(!m_EquipPopup.activeSelf);

            if (m_EquipPopup.activeSelf)
                CheckEquipSlot(); // 장비 슬롯 아이템 확인
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            m_StatPopup.SetActive(!m_StatPopup.activeSelf);
        }
    }

    //장비 슬롯에 아이템이 있는지 확인
    public void CheckEquipSlot()
    {
        // 장비 슬롯에 아이템이 있는지 확인
        for (int i = 0; i < m_EquipSlot.Length; i++)
        {
            if (m_EquipSlot[i].transform.childCount > 0)
            {
                GameObject a_Item = m_EquipSlot[i].transform.GetChild(0).gameObject;
                m_ItemObj[i] = a_Item;
            }
            else
            {
                // 아이템이 없으면 비활성화 후 인벤토리 이동 불가
                m_ItemObj[i] = null;
            }
        }
    }

    void OnApplicationQuit()
    {
        SaveEquip();
    }

    //장비 장착
    public void SetEquip(GameObject a_Item)
    {
        Player_Ctrl a_Player = FindObjectOfType<Player_Ctrl>();

        if (a_Player != null)
        {
            foreach (GameObject obj in a_Player.m_SkinnedObjs)
            {
                obj.SetActive(true);

                if (obj.name != a_Item.name)
                {
                    a_Player.m_SkinnedObjs[1].gameObject.SetActive(false);
                    a_Player.m_SkinnedObjs[2].gameObject.SetActive(false);
                    a_Player.m_SkinnedObjs[3].gameObject.SetActive(false);
                    a_Player.m_SkinnedObjs[5].gameObject.SetActive(false);
                    a_Player.m_SkinnedObjs[6].gameObject.SetActive(false);
                    a_Player.m_SkinnedObjs[7].gameObject.SetActive(false);
                    a_Player.m_SkinnedObjs[8].gameObject.SetActive(false);
                    a_Player.m_SkinnedObjs[9].gameObject.SetActive(false);
                }
            }

            a_Item.SetActive(true);

            // 장비 장착시 체력 증가
            a_Player.MaxHp += 20; // MaxHp 프로퍼티를 사용하여 최대 체력 증가

            // 장비 슬롯에 아이템 추가
            for (int i = 0; i < m_EquipSlot.Length; i++)
            {
                if (m_EquipSlot[i].transform.childCount > 0
                    && m_EquipSlot[i].transform.GetChild(0).gameObject == a_Item)
                {
                    m_ItemObj[i] = a_Item;
                    break;
                }
            }

            Data_Mgr.SaveData();

            SaveEquip();
        }
    }

    //장비 해제
    public void RemoveEquip(GameObject a_Item)
    {
        Player_Ctrl a_Player = FindObjectOfType<Player_Ctrl>();
        if (a_Player != null)
        {
            foreach (GameObject obj in a_Player.m_SkinnedObjs)
            {
                if (obj.name == a_Item.name)
                    obj.SetActive(false);
            }

            a_Player.m_SkinnedObjs[0].SetActive(true);
            a_Player.m_SkinnedObjs[4].SetActive(true);
            a_Player.m_SkinnedObjs[7].SetActive(true);
            a_Player.m_SkinnedObjs[8].SetActive(true);
            a_Player.m_SkinnedObjs[9].SetActive(true);

            // 장비 해제시 비활성화
            for (int i = 10; i <= 14; i++)
            {
                if (i < a_Player.m_SkinnedObjs.Length)
                {
                    a_Player.m_SkinnedObjs[i].SetActive(false);
                }
            }

            // 장비 해제시 체력 감소
            a_Player.MaxHp -= 20; // MaxHp 프로퍼티를 사용하여 최대 체력 감소
            Data_Mgr.SaveData();
        }

    }

    // 장착 저장
    public void SaveEquip()
    {
        List<int> Items = new List<int>();
        for (int i = 0; i < m_EquipSlot.Length; i++)
        {
            GameObject a_Slot = m_EquipSlot[i];
            if (a_Slot.transform.childCount > 0 && a_Slot.transform.GetChild(0).gameObject.activeSelf)
            {
                GameObject a_ItemObj = a_Slot.transform.GetChild(0).gameObject;

                if (int.TryParse(a_ItemObj.name, out int itemId))
                    Items.Add(itemId);

            }
        }
        string a_SvData = string.Join(",", Items);
        PlayerPrefs.SetString("Equipment", a_SvData);
        PlayerPrefs.Save();
    }

    public void LoadEquip()
    {
        string a_EqData = PlayerPrefs.GetString("Equipment", "");

        if (!string.IsNullOrEmpty(a_EqData))
        {
            string[] Items = a_EqData.Split(',');
            for (int i = 0; i < Items.Length; i++)
            {
                if (int.TryParse(Items[i], out int a_Id))
                {
                    ItemData a_ItData = new ItemData { Id = a_Id };
                    AddItem(a_ItData, i + 1); 

                    SetEquip(m_ItemObj[i]);

                    //장비 0번 슬롯만 비활성화
                    if (i == 0) m_ItemObj[i].SetActive(false);
                    
                }
            }
        }
        else
            SetDefEquip(); // 기본 장비 설정
    }

    // 기본 장비 설정
    void SetDefEquip()
    {
        int[] a_DefIds = { 3, 5, 7 };
        int[] a_EquipSlots = { 1, 2, 3 };

        for (int i = 0; i < a_DefIds.Length; i++)
        {
            int a_SlotIdx = a_EquipSlots[i];

            if (a_SlotIdx < m_EquipSlot.Length)
            {
                ItemData a_ItData = new ItemData { Id = a_DefIds[i] };
                AddItem(a_ItData, a_SlotIdx);
            }
        }
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

    public void AddItem(ItemData a_ItemData, int a_SlotIdx)
    {
        if (a_SlotIdx < 0 || a_SlotIdx >= m_EquipSlot.Length) return;

        // 장비 슬롯에 아이템 추가
        GameObject a_Slot = m_EquipSlot[a_SlotIdx];

        // 슬롯에 아이템이 있으면 활성화
        if (a_Slot.transform.childCount > 0)
        {

            GameObject a_ItemObj = a_Slot.transform.GetChild(0).gameObject;
            a_ItemObj.SetActive(true); // 활성화
            a_ItemObj.name = a_ItemData.Id.ToString(); // 아이템 ID 설정

            // 아이콘 설정
            if (IconPathMap.TryGetValue(a_ItemData.Id, out string a_IconPath))
                a_ItemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>(a_IconPath);

            m_ItemObj[a_SlotIdx] = a_ItemObj;
            m_ItemData = a_ItemData;
        }
    }
}
