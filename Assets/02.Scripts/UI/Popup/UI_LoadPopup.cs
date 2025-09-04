using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//씬 전환시 로딩 팝업
public class UI_LoadPopup : UI_Popup
{
    // 메시지 string Array
    string[] LoadMessges = new string[] { Define.LoadMessage1, Define.LoadMessage2, Define.LoadMessage3 };

    // 현재 메시지 Index
    int m_CurMessageIdx = 0;

    [SerializeField] Slider m_LoadSlider;

    [SerializeField] Text m_TipText;

    // 기본 설정
    public void SetInfo(Define.Scene a_Type, int a_Time = 0)
    {
        // 구글 시트 데이터 가져오기
        OnDataRequest();

        // slider 초기화
        m_LoadSlider.value = 0;
        m_LoadSlider.minValue = 0;
        m_LoadSlider.maxValue = a_Time;

        // 출력할 메시지 선정
        m_CurMessageIdx = Random.Range(0, 3);
        m_TipText.text = $"Tip : {LoadMessges[m_CurMessageIdx]}";

        // 플레이어 정지
        Managers.Game.StopPlayer();

        // 비동기 로드 시작
        StartCoroutine(LoadAsynSceneCoroutine(a_Type, a_Time));
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_CurMessageIdx++;
            if (m_CurMessageIdx >= LoadMessges.Length)
                m_CurMessageIdx = 0;

            m_TipText.text = $"Tip : {LoadMessges[m_CurMessageIdx]}";
        }
    }

    // 비동기 로드
    float a_LoadTime = 0;
    IEnumerator LoadAsynSceneCoroutine(Define.Scene a_Type, int a_Time = 0)
    {
        yield return null;

        // Scene Load
        AsyncOperation operation = Managers.Scene.LoadAsynScene(a_Type);

        // Load 시간 확인
        while (operation.isDone == false)
        {
            a_LoadTime += Time.deltaTime;

            m_LoadSlider.value = a_LoadTime;

            // 시간이 다 되면 탈출 
            if (a_LoadTime > a_Time)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    // 구글 스프레드시트 데이터 가져오기
    void OnDataRequest()
    {
        // 이미 데이터를 받았다면 종료
        if (Managers.Data.IsData == true) return;

        StartCoroutine(Managers.Data.DataRequest(Define.StartNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.LevelNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.SkillNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.UseItemNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.WeaponItemNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.ArmorItemNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.DropItemNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.MonsterNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.ShopNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.TalkNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.QuestNumber));

        Managers.Data.IsData = true;
    }
}
