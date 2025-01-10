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

        Data_Mgr.LoadData();//<--���� �����͸� �ҷ����� ����Ʈ �����͸� �������� ���� 

        // ������ ����Ʈ �����͸� �����ͼ� �������� ����Ʈ ����
        foreach (var Id in Data_Mgr.m_AcceptedQuest)
        {
            var a_Quest = Data_Mgr.m_QuestData.Find(q => q.Id == Id);

            //������ ���¶�� ��� ����
            if (a_Quest != null && a_Quest.IsClear == false)
                CreateNode(a_Quest);
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            m_QuestPopup.SetActive(!m_QuestPopup.activeSelf);
        }
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
}
