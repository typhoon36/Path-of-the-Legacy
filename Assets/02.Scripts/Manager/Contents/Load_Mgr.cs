using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Load_Mgr : MonoBehaviour
{
    //Help Text�� ���� String �迭
    public string[] m_HelpStr = new string[] { Define_S.LoadMsg1, Define_S.LoadMsg2 };

    public Text m_HelpTxt;

    int m_HelpIdx = 0;

    public Slider m_LoadingBar;


    void Start()
    {
        SetInfo(Define_S.Scene.Game, 2);
    }

    public void SetInfo(Define_S.Scene a_Type, int a_Time = 0)
    {
        m_LoadingBar.value = 0;
        m_LoadingBar.minValue = 0;
        m_LoadingBar.maxValue = a_Time;

        //��� �޽��� ����
        m_HelpIdx = Random.Range(0, 2);
        m_HelpTxt.text = $"Tip: {m_HelpStr[m_HelpIdx]}";

        //�񵿱� �ε� ����
        StartCoroutine(LoadAsycCo(a_Type, a_Time));
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_HelpIdx++;
            if (m_HelpIdx >= m_HelpStr.Length)
                m_HelpIdx = 0;

            m_HelpTxt.text = $"Tip : {m_HelpStr[m_HelpIdx]}";
        }

    }

    float a_LoadTime = 0;
    IEnumerator LoadAsycCo(Define_S.Scene a_Type, int a_Time = 0)
    {
        yield return null;

        // Scene Load
        AsyncOperation async = SceneManager.LoadSceneAsync(a_Type.ToString() + "Scene");
        async.allowSceneActivation = false;

        // Load �ð� Ȯ��
        while (!async.isDone)
        {
            a_LoadTime += Time.deltaTime;
            m_LoadingBar.value = a_LoadTime;

            // �ð� �ٵɽ� true
            if (a_LoadTime >= a_Time)
            {
                //��ٷȴٰ� true(�ڿ����������̰�)   
                yield return new WaitForSeconds(0.9f);
                async.allowSceneActivation = true;
            }

            yield return null;
        }
    }

}
