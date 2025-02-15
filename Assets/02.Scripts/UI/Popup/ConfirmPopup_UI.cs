using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ConfirmPopup_UI : MonoBehaviour
{
    public GameObject m_ConfirmObj;
    public Button m_ConfirmBtn;
    public Button m_CancelBtn;

    public Define_S.Scene m_TargetScene;

    private EventHandler_UI m_ConfirmBtnHandler;
    private EventHandler_UI m_CancelBtnHandler;

    public Text m_ConfirmText;

    #region Singleton
    public static ConfirmPopup_UI Inst;
    void Awake()
    {
        if (Inst == null)
        {
            Inst = this;
        }
    }
    #endregion

    private void Start()
    {
        m_ConfirmObj.gameObject.SetActive(false);

        m_ConfirmBtnHandler = m_ConfirmBtn.gameObject.AddComponent<EventHandler_UI>();
        m_CancelBtnHandler = m_CancelBtn.gameObject.AddComponent<EventHandler_UI>();

        m_ConfirmBtnHandler.OnClickHandler += OnConfirmBtnClick;
        m_CancelBtnHandler.OnClickHandler += OnCancelBtnClick;
    }

    void OnConfirmBtnClick(PointerEventData eventData)
    {
        m_ConfirmObj.SetActive(false);
        Time.timeScale = 1;
        Scene_Mgr.Inst.ChangeScene(m_TargetScene);
    }

    private void OnCancelBtnClick(PointerEventData eventData)
    {
        m_ConfirmObj.SetActive(false);
        Time.timeScale = 1;
    }

    public void ShowPopup(Define_S.Scene scene)
    {
        if (m_ConfirmObj != null)
        {
            m_TargetScene = scene; // m_TargetScene 설정
           
            if(scene == Define_S.Scene.Boss)
            {
                m_ConfirmText.text = "보스전으로 이동하시겠습니까?";
            }
            else if (scene == Define_S.Scene.Game)
            {
                m_ConfirmText.text = "마을로 이동하시겠습니까?";
            }
            else if(scene == Define_S.Scene.Dungeon)
            {
                m_ConfirmText.text = "던전으로 이동하시겠습니까?";
            }

            m_ConfirmObj.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
