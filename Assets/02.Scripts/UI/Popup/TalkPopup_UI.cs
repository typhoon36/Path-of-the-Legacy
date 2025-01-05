using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Npc�� ��ȭ�ϴ� �˾�â
public class TalkPopup_UI : MonoBehaviour
{
    TalkData m_TalkData;
    QuestData m_QuestData;

    #region UI
    [Header("Talk Panel")]
    public GameObject m_TalkPanel;
    public Text m_TalkTxt; //��ȭ �ؽ�Ʈ
    public Text m_NameTxt; //�̸� �ؽ�Ʈ
    public Button m_NextBtn; //���� ��ư

    [Header("Quest Panel")]
    public GameObject m_QuestPanel;
    public Text m_QTitleTxt;
    public Text m_QContentTxt;
    public Text m_QTargetTxt;
    public Text m_QRewardGoldTxt;
    public Text m_QRewardExpTxt;
    public Button m_QRefuseBtn;
    public Button m_QAgreeBtn;
    #endregion

    int m_TalkIdx = 0; //��ȭ �ε���
    bool IsNext = false;
    bool IsNextTalk = false;

    [SerializeField]
    float m_TalkDelay = 0.1f;//��ȭ �ӵ� ������

    #region Singleton
    public static TalkPopup_UI Inst;
    void Awake()
    {
        Inst = this;
    }
    #endregion

    void Start()
    {
        m_QuestPanel.SetActive(false);
        m_TalkPanel.SetActive(false);
        m_NextBtn.onClick.AddListener(() =>
        {
            OnClickNext();
        });
        m_QRefuseBtn.onClick.AddListener(() =>
        {
            OnClickRefuse();
        });
        m_QAgreeBtn.onClick.AddListener(() =>
        {
            ClickAccept();
        });

        m_NextBtn.gameObject.SetActive(false);
        m_QRefuseBtn.gameObject.SetActive(false);
        m_QAgreeBtn.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            //��ȭ�� ������ �ʾҴٸ�
            if (IsNext == false)
            {
                m_TalkDelay = m_TalkDelay / 2;
                return;
            }

            if (m_QuestPanel.activeSelf == true) return;

            if (IsNextTalk == true)
                OnClickNext();
            else
                Clear();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_TalkPanel.SetActive(false);
            InvenPopup_UI.Inst.m_InvenPopup.SetActive(false);
            EqStatPopup_UI.Inst.m_StatPopup.SetActive(false);
            EqStatPopup_UI.Inst.m_EquipPopup.SetActive(false);
        }
    }

    #region Dialog
    /// ��ȭ �� ����Ʈ,�Ϲ� ��ȭ�� ������ �����ؾ��ϳ� �����ε����� ���� �Լ��� ������ ���

    //<�Ϲ� ��ȭ ����>
    public void SetInfo(string Info, string a_Name = null)
    {
        if (Info != null)
        {
            if (a_Name != null)
                m_NameTxt.text = a_Name;

            //��ȭ ���� �� ����
            IsNextTalk = false;
            StartCoroutine(TypingText(Info));
            return;
        }
    }

    //<����Ʈ ��ȭ ����>
    public void SetInfo(TalkData a_Talk, QuestData a_Quest, string a_Name = null)
    {
        // �����͵��� ������ ����
        if (a_Talk == null || a_Quest == null) return;

        // ��ȭ ����
        if (a_Name != null)
            m_NameTxt.text = a_Name;

        m_TalkData = a_Talk;
        m_QuestData = a_Quest;

        m_TalkIdx = 0;

        SetQuest();
        NextTalk();
    }
    #endregion

    void NextTalk()
    {
        if (m_TalkIdx >= m_TalkData.questStartTalk.Count)
        {
            Clear();
            return;
        }

        StartCoroutine(TypingText(m_TalkData.questStartTalk[m_TalkIdx]));

        m_TalkIdx++;

        // ���� ��ȭ�� ������ ����Ʈ ���� Ȱ��ȭ
        if (m_TalkIdx >= m_TalkData.questStartTalk.Count)
        {
            IsNextTalk = false;
            m_QuestPanel.SetActive(true);
            QuestActive(true); // ����Ʈ ��ư Ȱ��ȭ
        }
        else
            IsNextTalk = true;
    }

    //��ȭ Ÿ���� ȿ�� �ڷ�ƾ
    IEnumerator TypingText(string a_Text)
    {
        m_TalkTxt.text = "";

        IsNext = false;
        m_TalkDelay = 0.05f;

        //Ÿ���� ȿ��
        foreach (var Letter in a_Text)
        {
            m_TalkTxt.text += Letter;
            yield return new WaitForSeconds(m_TalkDelay);
        }


        IsNext = true;

        //������ ��ȭ�� ������ ���� ��ư Ȱ��ȭ
        if (IsNextTalk == true)
            m_NextBtn.gameObject.SetActive(true);

        //����Ʈ�� ������ ����,���� ��ư Ȱ��ȭ
        if (m_QuestPanel.activeSelf == true)
            QuestActive(true);
    }

    #region Buttons
    void OnClickNext()
    {
        m_NextBtn.gameObject.SetActive(false);
        NextTalk();
    }
    void OnClickRefuse()
    {
        QuestActive(false);
        SetInfo(m_TalkData.refusalTalk);
    }
    void ClickAccept()
    {
        QuestActive(false);
        SetInfo(m_TalkData.acceptTalk);


        // ����Ʈ ���� �� ����Ʈ ��� ����
        QuestPopup_UI.Inst.CreateNode(m_QuestData);
    }
    #endregion

    void QuestActive(bool Active)
    {
        m_TalkPanel.SetActive(!Active);
        m_QuestPanel.SetActive(Active);
        m_QAgreeBtn.gameObject.SetActive(Active);
        m_QRefuseBtn.gameObject.SetActive(Active);
    }

    //����Ʈ UI ����
    void SetQuest()
    {
        m_QTitleTxt.text = m_QuestData.TitleName;
        m_QContentTxt.text = m_QuestData.Desc;
        m_QTargetTxt.text = m_QuestData.TargetDesc;
        m_QRewardGoldTxt.text = "< ���� :" + m_QuestData.RewardGold.ToString() + "��� ,";
        m_QRewardExpTxt.text = "Exp :"+ m_QuestData.RewardExp.ToString() + ">";
    }

    //�ʱ�ȭ
    public void Clear()
    {
        m_QuestPanel.SetActive(false);
        m_TalkPanel.SetActive(false);
        m_NextBtn.gameObject.SetActive(false);
        m_QAgreeBtn.gameObject.SetActive(false);
        m_QRefuseBtn.gameObject.SetActive(false);

    }
}
