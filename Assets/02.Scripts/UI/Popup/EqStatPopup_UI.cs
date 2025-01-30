using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//���&���� �˾�â
public class EqStatPopup_UI : MonoBehaviour
{
    [Header("EquipPopup")]
    public GameObject m_EquipPopup;
    public Button m_EquipCloseBtn;

    //��񽽷�
    public GameObject[] m_EquipSlot;
    public GameObject[] m_ItemObj;

    [Header("StatPopup")]
    public GameObject m_StatPopup;
    public Button m_StatCloseBtn;
    public Button m_STRBtn;
    public Button m_DEXBtn;
    public Button m_INTBtn;
    public Button m_LUKBtn;
    public Text m_STRTxt;
    public Text m_DEXTxt;
    public Text m_INTTxt;
    public Text m_LUKTxt;
    public Text m_LevelTxt;
    public Text m_StatPointTxt;

    EventHandler_UI m_EquipCloseBtnHandler;

    #region Singleton
    public static EqStatPopup_UI Inst;
    private void Awake()
    {
        Inst = this;
    }
    #endregion

    void Start()
    {
        #region EquipPopup
        m_EquipPopup.gameObject.SetActive(false);
        m_EquipCloseBtnHandler = m_EquipCloseBtn.gameObject.AddComponent<EventHandler_UI>();
        m_EquipCloseBtnHandler.OnClickHandler += OnCloseBtnClick;
        #endregion

        #region StatPopup
        m_StatPopup.gameObject.SetActive(false);
        m_StatCloseBtn.onClick.AddListener(() =>
        {
            m_StatPopup.SetActive(false);
        });
        m_LevelTxt.text = "���� : " + Data_Mgr.m_StartData.Level.ToString();

        //�������ϸ� ���� ����Ʈ 5����
        m_StatPointTxt.text = "���� ����Ʈ : " + Data_Mgr.m_StartData.StatPoint.ToString();

        m_STRBtn.onClick.AddListener(() =>
        {
            if (Data_Mgr.m_StartData.StatPoint > 0)
            {
                Data_Mgr.m_StartData.STR += 1;
                Data_Mgr.m_StartData.StatPoint -= 1;
                m_StatPointTxt.text = "���� ����Ʈ : " + Data_Mgr.m_StartData.StatPoint.ToString();
                m_STRTxt.text = "�ٷ� : " + Data_Mgr.m_StartData.STR.ToString();
            }
        });

        m_DEXBtn.onClick.AddListener(() =>
        {
            if (Data_Mgr.m_StartData.StatPoint > 0)
            {
                Data_Mgr.m_StartData.Speed += 1;
                Data_Mgr.m_StartData.StatPoint -= 1;
                m_StatPointTxt.text = "���� ����Ʈ : " + Data_Mgr.m_StartData.StatPoint.ToString();
                m_DEXTxt.text = "��ø : " + Data_Mgr.m_StartData.Speed.ToString();
            }
        });

        m_INTBtn.onClick.AddListener(() =>
        {
            if (Data_Mgr.m_StartData.StatPoint > 0)
            {
                Data_Mgr.m_StartData.Int += 1;
                Data_Mgr.m_StartData.StatPoint -= 1;
                m_StatPointTxt.text = "���� ����Ʈ : " + Data_Mgr.m_StartData.StatPoint.ToString();
                m_INTTxt.text = "���� : " + Data_Mgr.m_StartData.Int.ToString();
            }
        });

        m_LUKBtn.onClick.AddListener(() =>
        {
            if (Data_Mgr.m_StartData.StatPoint > 0)
            {
                Data_Mgr.m_StartData.Luk += 1;
                Data_Mgr.m_StartData.StatPoint -= 1;
                m_StatPointTxt.text = "���� ����Ʈ : " + Data_Mgr.m_StartData.StatPoint.ToString();
                m_LUKTxt.text = "�� : " + Data_Mgr.m_StartData.Luk.ToString();
            }
        });
        #endregion

        Data_Mgr.LoadData(); // ������ �ε�

        // ��� ������ ������ ������Ʈ �迭 �ʱ�ȭ
        m_ItemObj = new GameObject[m_EquipSlot.Length];
        CheckEquipSlot();

        // ������ �̺�Ʈ �ڵ鷯 ���
        Data_Mgr.OnLevelUp += OnLevelUp;

        // ��� ���� ������ �ʱ�ȭ
        for (int i = 0; i < m_EquipSlot.Length; i++)
        {
            if (m_EquipSlot[i].transform.childCount > 0)
            {
                GameObject item = m_EquipSlot[i].transform.GetChild(0).gameObject;
                m_ItemObj[i] = item;
            }
            else
            {
                m_ItemObj[i] = null;
            }
        }
    }

    void OnLevelUp()
    {
        Data_Mgr.m_StartData.StatPoint += 5;
        m_LevelTxt.text = "���� : " + Data_Mgr.m_StartData.Level.ToString();
        m_StatPointTxt.text = "���� ����Ʈ : " + Data_Mgr.m_StartData.StatPoint.ToString();
    }

    void OnDestroy()
    {
        // �̺�Ʈ �ڵ鷯 ����
        Data_Mgr.OnLevelUp -= OnLevelUp;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            m_EquipPopup.SetActive(!m_EquipPopup.activeSelf);
            if (m_EquipPopup.activeSelf)
                CheckEquipSlot(); // ��� ���� ������ Ȯ��

        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            m_StatPopup.SetActive(!m_StatPopup.activeSelf);
        }

    }

    public void CheckEquipSlot()
    {
        // ��� ���Կ� �������� �ִ��� Ȯ��
        for (int i = 0; i < m_EquipSlot.Length; i++)
        {
            if (m_EquipSlot[i].transform.childCount > 0)
            {
                GameObject item = m_EquipSlot[i].transform.GetChild(0).gameObject;
                m_ItemObj[i] = item;
            }
            else
            {
                // �������� ������ ��Ȱ��ȭ �� �κ��丮 �̵� �Ұ�
                m_ItemObj[i] = null;
            }
        }
    }

    void OnCloseBtnClick(UnityEngine.EventSystems.PointerEventData eventData)
    {
        m_EquipPopup.SetActive(false);
    }

    //��� ����
    public void SetEquip(GameObject item)
    {
        Player_Ctrl a_Player = FindObjectOfType<Player_Ctrl>();
        if (a_Player != null)
        {
            foreach (GameObject obj in a_Player.m_SkinnedObjs)
            {
                obj.SetActive(true);

                //������������ ������(���� ö�������� �����ϸ� Starter_Chest�� ��Ȱ��ȭ)
                if (obj.name != item.name)
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

            // ������ ������ Ȱ��ȭ
            item.SetActive(true);
        }
    }

    //��� ����
    public void RemoveEquip(GameObject item)
    {
        Player_Ctrl a_Player = FindObjectOfType<Player_Ctrl>();
        if (a_Player != null)
        {
            foreach (GameObject obj in a_Player.m_SkinnedObjs)
            {
                if (obj.name == item.name)
                {
                    obj.SetActive(false);
                }
            }

            // Ư�� �ε��� Ȱ��ȭ
            a_Player.m_SkinnedObjs[0].SetActive(true);
            a_Player.m_SkinnedObjs[4].SetActive(true);
            a_Player.m_SkinnedObjs[7].SetActive(true);
            a_Player.m_SkinnedObjs[8].SetActive(true);
            a_Player.m_SkinnedObjs[9].SetActive(true);

            // �߰��� �κ�: 10~14 �ε��� ��Ȱ��ȭ
            for (int i = 10; i <= 14; i++)
            {
                if (i < a_Player.m_SkinnedObjs.Length)
                {
                    a_Player.m_SkinnedObjs[i].SetActive(false);
                }
            }
        }
    }
}
