using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Npc와 대화하는 팝업창
public class TalkPopup_UI : MonoBehaviour
{
    TalkData m_TalkData;
    QuestData m_QuestData;

    #region UI
    [Header("Talk Panel")]
    public GameObject m_TalkPanel;
    public Text m_TalkTxt; //대화 텍스트
    public Text m_NameTxt; //이름 텍스트
    public Button m_NextBtn; //다음 버튼

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

    int m_TalkIdx = 0; //대화 인덱스
    bool IsNext = false;
    bool IsNextTalk = false;

    [SerializeField]
    float m_TalkDelay = 0.1f;//대화 속도 딜레이

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
            //대화가 끝나지 않았다면
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
    /// 대화 시 퀘스트,일반 대화로 나눠서 설정해야하나 오버로딩으로 같은 함수를 나눠서 사용

    //<일반 대화 설정>
    public void SetInfo(string Info, string a_Name = null)
    {
        if (Info != null)
        {
            if (a_Name != null)
                m_NameTxt.text = a_Name;

            //대화 진행 후 종료
            IsNextTalk = false;
            StartCoroutine(TypingText(Info));
            return;
        }
    }

    //<퀘스트 대화 설정>
    public void SetInfo(TalkData a_Talk, QuestData a_Quest, string a_Name = null)
    {
        // 데이터들이 없으면 리턴
        if (a_Talk == null || a_Quest == null) return;

        // 대화 설정
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

        // 다음 대화가 없으면 퀘스트 정보 활성화
        if (m_TalkIdx >= m_TalkData.questStartTalk.Count)
        {
            IsNextTalk = false;
            m_QuestPanel.SetActive(true);
            QuestActive(true); // 퀘스트 버튼 활성화
        }
        else
            IsNextTalk = true;
    }

    //대화 타이핑 효과 코루틴
    IEnumerator TypingText(string a_Text)
    {
        m_TalkTxt.text = "";

        IsNext = false;
        m_TalkDelay = 0.05f;

        //타이핑 효과
        foreach (var Letter in a_Text)
        {
            m_TalkTxt.text += Letter;
            yield return new WaitForSeconds(m_TalkDelay);
        }


        IsNext = true;

        //다음가 대화가 있으면 다음 버튼 활성화
        if (IsNextTalk == true)
            m_NextBtn.gameObject.SetActive(true);

        //퀘스트가 켜지면 수락,거절 버튼 활성화
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


        // 퀘스트 수락 시 퀘스트 노드 생성
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

    //퀘스트 UI 설정
    void SetQuest()
    {
        m_QTitleTxt.text = m_QuestData.TitleName;
        m_QContentTxt.text = m_QuestData.Desc;
        m_QTargetTxt.text = m_QuestData.TargetDesc;
        m_QRewardGoldTxt.text = "< 보상 :" + m_QuestData.RewardGold.ToString() + "골드 ,";
        m_QRewardExpTxt.text = "Exp :"+ m_QuestData.RewardExp.ToString() + ">";
    }

    //초기화
    public void Clear()
    {
        m_QuestPanel.SetActive(false);
        m_TalkPanel.SetActive(false);
        m_NextBtn.gameObject.SetActive(false);
        m_QAgreeBtn.gameObject.SetActive(false);
        m_QRefuseBtn.gameObject.SetActive(false);

    }
}
