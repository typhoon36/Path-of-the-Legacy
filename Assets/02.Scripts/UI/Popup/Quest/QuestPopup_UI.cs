using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// ����Ʈ �˾� UI(������ ����Ʈ�� ������ �ִ��� �����ִ� �˾�)
public class QuestPopup_UI : MonoBehaviour
{
    [Header("QuestPopup")]
    public GameObject m_QuestPopup;
    public GameObject m_QuestContent;

    [Header("QuestButton")]
    public Button m_CloseBtn;
    public Button m_ProgresBtn;
    public Button m_CompleteBtn;

    [Header("QuestText")]
    public Text m_QuestTitle;
    public Text m_QuestDesc;

    [Header("QuestNode")]
    public GameObject m_QuestNode;
    QuestData m_QuestData;

    [Header("QuestInfo")]
    public GameObject m_QuestInfoObj;
    public Text m_QuestInfo;
    public Text m_QuestTargetCnt;
    public Button m_QuestCloseBtn;
    public Button m_QuestOpenBtn;

    #region Singleton
    public static QuestPopup_UI Inst;
    void Awake()
    {
        Inst = this;
    }
    #endregion


    void Start()
    {
        m_QuestPopup.SetActive(false);
        m_CloseBtn.onClick.AddListener(() => m_QuestPopup.SetActive(false));
        m_QuestTitle.text = "";
        m_QuestDesc.text = "";
        m_QuestTargetCnt.text = "";
        m_QuestInfo.text = "";

        Data_Mgr.LoadData(); // ���� �����͸� �ҷ����� ����Ʈ �����͸� �������� ���� 

        // ������ ����Ʈ �����͸� �����ͼ� �������� ����Ʈ ����
        foreach (var Id in Data_Mgr.m_AcceptedQuest)
        {
            var a_Quest = Data_Mgr.m_QuestData.Find(q => q.Id == Id);

            // ������ ���¶�� ��� ����
            if (a_Quest != null && a_Quest.IsClear == false)
            {
                CreateNode(a_Quest);
                m_QuestData = a_Quest; // ���� ����Ʈ �����͸� ����
            }
        }

        // ����Ʈ �ݱ� ��ư
        m_QuestCloseBtn.onClick.AddListener(() =>
        {
            m_QuestInfoObj.SetActive(false);

            //m_QuestInfoObj�� ��������  ��ư ��ġ ����
            if(m_QuestInfoObj.activeSelf == false)
            {
                RectTransform a_Rect = m_QuestOpenBtn.GetComponent<RectTransform>();
                a_Rect.anchoredPosition = new Vector2(600, 120);
            }
        });

        // ����Ʈ ���� ��ư
        m_QuestOpenBtn.onClick.AddListener(() =>
        {
            m_QuestInfoObj.SetActive(true);

            if(m_QuestInfoObj.activeSelf == true)
            {
                RectTransform a_Rect = m_QuestOpenBtn.GetComponent<RectTransform>();
                a_Rect.anchoredPosition = new Vector2(375, 120);
            }
        });

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            m_QuestPopup.SetActive(!m_QuestPopup.activeSelf);
        }
        RefreshUI();
    }

    // �������� ��� ����
    public void CreateNode(QuestData a_QuestData)
    {
        if (a_QuestData == null) return;

        GameObject a_Obj = Instantiate(m_QuestNode, m_QuestContent.transform);
        Quest_Nd a_Node = a_Obj.GetComponent<Quest_Nd>();
        a_Node.Init(a_QuestData);

        // ��� Ŭ���� ����Ʈ ���� �����ֱ�
        a_Node.m_NodeBtn.onClick.AddListener(() =>
        {
            m_QuestData = a_QuestData;
            // ����Ʈ ���� ǥ��
            m_QuestTitle.text = m_QuestData.TitleName;

            // ����Ʈ ����, ��ǥ, ���� ǥ��
            m_QuestDesc.text = m_QuestData.Desc + "\n" + m_QuestData.TargetCnt + "����" + "\n" + m_QuestData.RewardGold +
            "���" + "\n" + m_QuestData.RewardExp + "����ġ";
        });
    }


    //����Ʈ ��ǥ ���� �ݿ�
    public void QuestTargetCnt(GameObject a_Obj)
    {
        // ������ ����Ʈ�� ������ ����
        if (m_QuestData == null) return;

        // ���� üũ
        if (a_Obj.GetComponent<MonsterStat>())
        {
            // ������ ����Ʈ��ŭ �ݺ�
            foreach (var a_Quest in Data_Mgr.m_QuestData)
            {
                // ������Ʈ ID�� ����Ʈ Ÿ�� ID�� ���ٸ�
                if (a_Quest.TargetId == a_Obj.GetComponent<MonsterStat>().m_Id && a_Quest.IsAccept && !a_Quest.IsClear)
                {
                    // ����Ʈ ��ǥ ���� ����
                    a_Quest.CurTargetCnt++;
                    RefreshUI();

                    // ����Ʈ ��ǥ ������ ���� ��ǥ ������ ���ٸ�
                    if (a_Quest.CurTargetCnt >= a_Quest.TargetCnt)
                        // ����Ʈ �Ϸ�
                        a_Quest.IsClear = true;


                    // ������ ����(��ǥ ���� �ݿ� �� �Ϸ� ����Ʈ ����)
                    Data_Mgr.SaveData();
                    return;
                }
            }
        }
    }

    public void RefreshUI()
    {
        if (m_QuestData == null) return;

        if (m_QuestData != null)
        {
            m_QuestInfo.text = m_QuestData.Desc;
            m_QuestTargetCnt.text = m_QuestData.CurTargetCnt + "/" + m_QuestData.TargetCnt;

            // ����Ʈ �Ϸ�� m_QuestTargetCnt �ؽ�Ʈ ����
            if (m_QuestData.IsClear)
                m_QuestTargetCnt.text = m_QuestData.CurTargetCnt + "/" + m_QuestData.TargetCnt + " (�Ϸ�)";
            

        }

    }
}
