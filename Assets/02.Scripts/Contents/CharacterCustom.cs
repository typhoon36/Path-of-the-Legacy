using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


// 캐릭터 커스텀 제어 ( 커스텀 부위 : 머리카락, 눈썹, 수염, 귀,코 )
public class CharacterCustom : MonoBehaviour
{
    public bool stopRotation = false;    // 회전 제어

    [SerializeField]
    float rotationSpeed = 3.5f;     // 회전 속도
    float currentRotation_Y = 0.01f;    // 캐릭터 Y 회전값 

    // 부위별 파츠 리스트
    [SerializeField] List<GameObject> hairList = new List<GameObject>();
    [SerializeField] List<GameObject> eyebrowsList = new List<GameObject>();
    [SerializeField] List<GameObject> facialHairList = new List<GameObject>();
    [SerializeField] List<GameObject> EarsList = new List<GameObject>();
    [SerializeField] List<GameObject> NoseList = new List<GameObject>();

    // 부위별 현재 List index
    int currentHairIndex = 0;
    int currentEyebrowsIndex = 0;
    int currentFacialHairIndex = 0;
    int CurrentEarsIndex = 0;
    int CurrentNoseIndex = 0;


    void Update()
    {
        CharaterRotation();
    }

    // 파츠 변경 버튼을 누를 때 호출
    public void NextPart(Define.DefaultPart a_PartType, bool IsNext)
    {
        // 부위 타입에 맞게 적용
        switch (a_PartType)
        {
            case Define.DefaultPart.Hair:
                ChangePart(hairList, ref currentHairIndex, IsNext);
                break;
            case Define.DefaultPart.Eyebrows:
                ChangePart(eyebrowsList, ref currentEyebrowsIndex, IsNext);
                break;
            case Define.DefaultPart.FacialHair:
                ChangePart(facialHairList, ref currentFacialHairIndex, IsNext);
                break;
            case Define.DefaultPart.Ears:
                ChangePart(EarsList, ref CurrentEarsIndex, IsNext);
                break;
            case Define.DefaultPart.Nose:
                ChangePart(NoseList, ref CurrentNoseIndex, IsNext);
                break;
        }
    }

    //커스텀 저장 ( 저장 버튼을 누를 때 호출 )
    public void SaveCustom()
    {
        // 딕셔너리 생성
        Managers.Game.DefaultPart = new Dictionary<Define.DefaultPart, SkinnedData>();

        // GameManager의 데이터에 저장
        Managers.Game.DefaultPart.Add(Define.DefaultPart.Hair, SetSkinned(hairList[currentHairIndex]));
        Managers.Game.DefaultPart.Add(Define.DefaultPart.Eyebrows, SetSkinned(eyebrowsList[currentEyebrowsIndex]));
        Managers.Game.DefaultPart.Add(Define.DefaultPart.FacialHair, SetSkinned(facialHairList[currentFacialHairIndex]));
        Managers.Game.DefaultPart.Add(Define.DefaultPart.Ears, SetSkinned(EarsList[CurrentEarsIndex]));
        Managers.Game.DefaultPart.Add(Define.DefaultPart.Nose, SetSkinned(NoseList[CurrentNoseIndex]));
    }

    // 캐릭터 회전 (Update)
    void CharaterRotation()
    {
        // 회전 제어
        if (stopRotation == true) return;

        // UI를 클릭하면 회전 X
        if (Input.GetMouseButtonDown(0) == true || Input.GetMouseButtonDown(1) == true)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
        }

        //회전제어
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            SetRotation(-Input.GetAxis("Horizontal"));
        }
        else if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            SetRotation(-Input.GetAxis("Mouse X"));
        }
    }

    // 회전 설정
    void SetRotation(float horizontal)
    {
        currentRotation_Y += horizontal * rotationSpeed;

        transform.localRotation = Quaternion.Euler(0f, currentRotation_Y, 0f);
    }

    // 파츠 부위 변경
    void ChangePart(List<GameObject> partList, ref int currentIndex, bool isNext)
    {
        // 현재 부위 비활성화
        partList[currentIndex].SetActive(false);

        // ( 다음 ) Button
        if (isNext == true)
        {
            currentIndex++;

            // 다음버튼 눌렀을 때 현재 인덱스가 마지막이라면 처음으로 이동
            if (currentIndex >= partList.Count)
                currentIndex = 0;
        }
        // ( 이전 ) Button
        else
        {
            currentIndex--;

            // 뒤로버튼 눌렀을 때 현재 인덱스가 처음이라면 마지막으로 이동
            if (currentIndex < 0)
                currentIndex = partList.Count - 1;
        }

        // 변경된 부위 활성화
        partList[currentIndex].SetActive(true);
    }

    // SkinnedMeshRenderer 필요 정보 저장
    SkinnedData SetSkinned(GameObject skinnedObject)
    {
        // SkinnedMeshRenderer 컴포넌트 받기
        SkinnedMeshRenderer skinnedMesh = skinnedObject.GetComponent<SkinnedMeshRenderer>();

        // 이름, localBounds, rootBone을 저장
        SkinnedData skinned = new SkinnedData()
        {
            sharedMeshName = skinnedMesh.name,
            bounds = skinnedMesh.localBounds,
            rootBoneName = skinnedMesh.rootBone.name,
        };

        // bones 저장
        skinned.bones = new List<string>();
        foreach (Transform child in skinnedMesh.bones)
            skinned.bones.Add(child.name);

        return skinned;
    }
}
