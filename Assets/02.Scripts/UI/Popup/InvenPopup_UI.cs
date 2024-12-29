using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvenPopup_UI : MonoBehaviour
{
    public GameObject m_InvenPopup;
    public Button m_CloseBtn;
    public RectTransform m_Content;
    public ScrollRect m_ScrollRect; // ScrollRect 컴포넌트 추가

    private Vector3 m_OriginalPosition;

    public Text m_GoldText;

    #region Singleton
    public static InvenPopup_UI Inst;
    private void Awake()
    {
        Data_Mgr.LoadData(); // 데이터 로드

        if (Inst == null)
            Inst = this;

        m_OriginalPosition = m_Content.anchoredPosition; // 원래 위치를 저장
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
                m_Content.anchoredPosition = m_OriginalPosition;
                CheckInventoryItems(); // 인벤토리 아이템 확인
            }
            else
            {
                m_Content.anchoredPosition = m_OriginalPosition;
            }
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

    public void AddItem()
    {
        

    }
}
