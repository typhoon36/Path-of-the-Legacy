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
        // SettingBtn�� ������ SettingPanel�� Ȱ��ȭ


        m_SettingBtn.onClick.AddListener(() => { m_SettingPanel.SetActive(true); });

        if (m_ResumeBtn != null)
            m_ResumeBtn.onClick.AddListener(() => { m_MenuPanel.SetActive(false); Time.timeScale = 1; });

        if (m_SettingBtn != null)
            m_SettingBtn.onClick.AddListener(() => { m_SettingPanel.SetActive(true); });

        if (m_QuitBtn != null)
            m_QuitBtn.onClick.AddListener(() =>
            {
#if UNITY_EDITOR
                EditorApplication.isPlaying = false; // ����Ƽ �����Ϳ��� �÷��� ����
#else
                SceneManager.LoadScene("TitleScene"); // ���� ���¿����� Ÿ��Ʋ ȭ������ ��ȯ
#endif
            });
    }

    void Update()
    {
        // Escape Ű�� ������ �޴��� ���� �ݴ´�.
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            m_MenuPanel.SetActive(!m_MenuPanel.activeSelf);
            IsPressed = m_MenuPanel.activeSelf;

            //���� �����ڸ� ����Ͽ� ���� �Ͻ�����(0�� ����, 1�� ����)
            Time.timeScale = m_MenuPanel.activeSelf ? 0 : 1;
        }

    }



}
