using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//씬 전환시 로딩 팝업
public class UI_LoadPopup : UI_Popup
{
    // 메시지 string Array
     string[]        loadMessges = new string[]{Define.LoadMessage1, Define.LoadMessage2, Define.LoadMessage3};

    // 현재 메시지 Index
     int             currentMessageIndex = 0;

    [SerializeField]
     Slider          loadSlider;

    [SerializeField]
     Text tipText;

    // 기본 설정
    public void SetInfo(Define.Scene type, int plusTime = 0)
    {
        // 구글 시트 데이터 가져오기
        OnDataRequest();

        // slider 초기화
        loadSlider.value = 0;
        loadSlider.minValue = 0;
        loadSlider.maxValue = plusTime;

        // 출력할 메시지 선정
        currentMessageIndex = Random.Range(0,3);
        tipText.text = $"Tip : {loadMessges[currentMessageIndex]}";

        // 플레이어 정지
        Managers.Game.StopPlayer();

        // 비동기 로드 시작
        StartCoroutine(LoadAsynSceneCoroutine(type, plusTime));
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            currentMessageIndex++;
            if (currentMessageIndex >= loadMessges.Length)
                currentMessageIndex = 0;

            tipText.text = $"Tip : {loadMessges[currentMessageIndex]}";
        }
    }
    
    // 비동기 로드
     float loadTime = 0;
     IEnumerator LoadAsynSceneCoroutine(Define.Scene type, int plusTime = 0)
    {
        yield return null;

        // Scene Load
        AsyncOperation operation = Managers.Scene.LoadAsynScene(type);

        // Load 시간 확인
        while (operation.isDone == false)
        {
            loadTime += Time.deltaTime;

            loadSlider.value = loadTime;

            // 시간이 다 되면 탈출 
            if (loadTime > plusTime)
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
