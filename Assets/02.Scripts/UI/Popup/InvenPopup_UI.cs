using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvenPopup_UI : MonoBehaviour
{
    public GameObject m_InvenPopup;
    public Button m_CloseBtn;
    public RectTransform m_Content;
    public ScrollRect m_ScrollRect; // ScrollRect ������Ʈ �߰�

    private Vector3 m_OriginalPosition;

    public Text m_GoldText;

    ItemData m_ItemData;

    #region Singleton
    public static InvenPopup_UI Inst;
    private void Awake()
    {
        Data_Mgr.LoadData(); // ������ �ε�

        if (Inst == null)
            Inst = this;

        m_OriginalPosition = m_Content.anchoredPosition; // ���� ��ġ�� ����
    }
    #endregion

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
                m_Content.anchoredPosition = m_OriginalPosition;
                CheckInventoryItems(); // �κ��丮 ������ Ȯ��
            }
            else
            {
                m_Content.anchoredPosition = m_OriginalPosition;
            }
        }

        //��Ʈ��Ű�� ������ ���ʸ��콺 Ŭ���� ������ �Ǹ�
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0) && ShopPopup_UI.Inst.m_ShopPopup.activeSelf == true)
        {
          
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
        Data_Mgr.SaveData(); // ��� ���� ����
    }



    private void CheckInventoryItems()
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
    public void AddItem(ItemData a_ItemData)
    {
        foreach (Transform a_Slot in m_Content)
        {
            if (a_Slot.GetChild(0).gameObject.activeSelf == false)
            {
                // ������ �߰�
                a_Slot.GetChild(0).gameObject.SetActive(true); // Ȱ��ȭ
                // Ȱ��ȭ �� ������ ���� ����
                a_Slot.GetChild(0).GetComponent<Image>().sprite = a_ItemData.ItemIcon;

                break;
            }
        }
    }

    //������ �Ǹ�
    public void SellItem(ItemData a_ItemData)
    {
        foreach (Transform a_Slot in m_Content)
        {
            if (a_Slot.GetChild(0).gameObject.activeSelf == true)
            {
                // ������ �Ǹ�
                a_Slot.GetChild(0).gameObject.SetActive(false); // ��Ȱ��ȭ
                // �Ǹ� �� ������ ���� �ʱ�ȭ
                a_Slot.GetChild(0).GetComponent<Image>().sprite = null;
                break;
            }
        }
    }
}
