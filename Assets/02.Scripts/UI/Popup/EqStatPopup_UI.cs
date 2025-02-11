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
        m_StatCloseBtn.onClick.AddListener(() =>
        {
            m_StatPopup.SetActive(false);
        });
        m_LevelTxt.text = "���� : " + Data_Mgr.m_StartData.Level.ToString();

        m_HPTxt.text = "����� : " + Data_Mgr.m_StartData.MaxHp.ToString();
        m_STRTxt.text = "�ٷ� : " + Data_Mgr.m_StartData.STR.ToString();
        m_DEXTxt.text = "��ø : " + Data_Mgr.m_StartData.Speed.ToString();
        m_INTTxt.text = "���� : " + Data_Mgr.m_StartData.Int.ToString();
        m_LUKTxt.text = "�� : " + Data_Mgr.m_StartData.Luk.ToString();


        //�������ϸ� ���� ����Ʈ 5����
        m_StatPointTxt.text = "���� ����Ʈ : " + Data_Mgr.m_StartData.StatPoint.ToString();

        m_HPBtn.onClick.AddListener(() =>
        {
            if (Data_Mgr.m_StartData.StatPoint > 0)
            {
                Data_Mgr.m_StartData.MaxHp += 20;
                Data_Mgr.m_StartData.StatPoint -= 1;
                m_StatPointTxt.text = "���� ����Ʈ : " + Data_Mgr.m_StartData.StatPoint.ToString();
                Data_Mgr.SaveData();
            }
        });

        m_STRBtn.onClick.AddListener(() =>
        {
            if (Data_Mgr.m_StartData.StatPoint > 0)
            {
                Data_Mgr.m_StartData.STR += 1;
                Data_Mgr.m_StartData.StatPoint -= 1;
                m_StatPointTxt.text = "���� ����Ʈ : " + Data_Mgr.m_StartData.StatPoint.ToString();
                m_STRTxt.text = "�ٷ� : " + Data_Mgr.m_StartData.STR.ToString();
                Data_Mgr.SaveData();
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
                Data_Mgr.SaveData();
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
                Data_Mgr.SaveData();
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
                Data_Mgr.SaveData();
            }
        });
        #endregion

        Data_Mgr.LoadData(); // ������ �ε�
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
                GameObject a_Item = m_EquipSlot[i].transform.GetChild(0).gameObject;
                m_ItemObj[i] = a_Item;
            }
            else
            {
                // �������� ������ ��Ȱ��ȭ �� �κ��丮 �̵� �Ұ�
                m_ItemObj[i] = null;
            }
        }
    }

    //��� ����
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

            // ��� ������ ü�� ����
            a_Player.MaxHp += 20; // MaxHp ������Ƽ�� ����Ͽ� �ִ� ü�� ����
            Data_Mgr.SaveData();

            Debug.Log(a_Player.MaxHp);
        }
    }

    //��� ����
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

            // ��� ������ ��Ȱ��ȭ
            for (int i = 10; i <= 14; i++)
            {
                if (i < a_Player.m_SkinnedObjs.Length)
                {
                    a_Player.m_SkinnedObjs[i].SetActive(false);
                }
            }

            // ��� ������ ü�� ����
            a_Player.MaxHp -= 20; // MaxHp ������Ƽ�� ����Ͽ� �ִ� ü�� ����
            Data_Mgr.SaveData();

            Debug.Log(a_Player.MaxHp);
        }
    }

}

