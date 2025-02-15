using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class Menu_Mgr : MonoBehaviour
{
    public bool IsPressed { get; private set; } = false;

    [Header("Menu")]
    public GameObject m_MenuPanel;
    public Button m_ResumeBtn;
    public Button m_SettingBtn;
    public Button m_QuitBtn;

    [Header("Setting")]
    public GameObject m_SettingPanel;

    void Start()
    {
        m_MenuPanel.SetActive(false);
        // SettingBtn을 누르면 SettingPanel을 활성화


        m_SettingBtn.onClick.AddListener(() => { m_SettingPanel.SetActive(true); });

        if (m_ResumeBtn != null)
            m_ResumeBtn.onClick.AddListener(() => { m_MenuPanel.SetActive(false); Time.timeScale = 1; });

        if (m_SettingBtn != null)
            m_SettingBtn.onClick.AddListener(() => { m_SettingPanel.SetActive(true); });

        if (m_QuitBtn != null)
            m_QuitBtn.onClick.AddListener(() =>
            {
#if UNITY_EDITOR
                EditorApplication.isPlaying = false; // 유니티 에디터에서 플레이 멈춤
#else
                SceneManager.LoadScene("TitleScene"); // 빌드 상태에서는 타이틀 화면으로 전환
#endif
            });
    }

    void Update()
    {
        // Escape 키를 누르면 메뉴를 열고 닫는다.
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            m_MenuPanel.SetActive(!m_MenuPanel.activeSelf);
            IsPressed = m_MenuPanel.activeSelf;

            //삼항 연산자를 사용하여 게임 일시정지(0은 정지, 1은 실행)
            Time.timeScale = m_MenuPanel.activeSelf ? 0 : 1;
        }

    }



}
