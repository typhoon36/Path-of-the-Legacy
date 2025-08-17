using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public class CharacterCustom : MonoBehaviour
{
    public bool stopRotation = false;    // 회전 제어

    [SerializeField]
    private float rotationSpeed = 3.5f;     // 회전 속도
    private float currentRotation_Y = 0.01f;    // 캐릭터 Y 회전값 

    // 부위별 파츠 리스트
    [SerializeField] List<GameObject> HairList = new List<GameObject>();
    [SerializeField] List<GameObject> EyeBrowList = new List<GameObject>();
    [SerializeField] List<GameObject> Ears = new List<GameObject>();
    [SerializeField] List<GameObject> FacialHairList = new List<GameObject>();
    [SerializeField] List<GameObject> NoseList = new List<GameObject>();

    // 부위별 현재 List index
    private int currentHairIndex = 0;
    private int currentEyebrowsIndex = 0;
    private int currentEarIndex = 0;
    private int currentFacialHairIndex = 0;
    private int currentNoseIndex = 0;


    private void Update()
    {
        CharaterRotation();
    }

    // ~ UI_CustomButton.cs 에서 파츠 변경 버튼을 누를 때 호출
    public void NextPart(Define.DefaultPart partType, bool isNext)
    {
        // 부위 타입에 맞게 변경
        switch (partType)
        {
            case Define.DefaultPart.Hair:
                ChangePart(HairList, ref currentHairIndex, isNext);
                break;
            case Define.DefaultPart.Eyebrows:
                ChangePart(EyeBrowList, ref currentEyebrowsIndex, isNext);
                break;
            case Define.DefaultPart.Ears:
                ChangePart(Ears, ref currentEarIndex, isNext);
                break;

            case Define.DefaultPart.FacialHair:
                ChangePart(FacialHairList, ref currentFacialHairIndex, isNext);
                break;
            case Define.DefaultPart.Nose:
                ChangePart(NoseList, ref currentNoseIndex, isNext);
                break;

        }
    }

    // ~ UI_CustomButton.cs 에서 확인 버튼을 누를 때 호출
    public void SaveCustom()
    {
        // 딕셔너리 생성
        Managers.Game.DefaultPart = new Dictionary<Define.DefaultPart, SkinnedData>();

        // GameManager의 데이터에 저장
        Managers.Game.DefaultPart.Add(Define.DefaultPart.Hair, SetSkinned(HairList[currentHairIndex]));
        Managers.Game.DefaultPart.Add(Define.DefaultPart.Eyebrows, SetSkinned(EyeBrowList[currentEyebrowsIndex]));
        Managers.Game.DefaultPart.Add(Define.DefaultPart.Ears, SetSkinned(Ears[currentEarIndex]));
        Managers.Game.DefaultPart.Add(Define.DefaultPart.FacialHair, SetSkinned(FacialHairList[currentFacialHairIndex]));
        Managers.Game.DefaultPart.Add(Define.DefaultPart.Nose, SetSkinned(NoseList[currentNoseIndex]));

    }

    // 캐릭터 회전 (Update)
    private void CharaterRotation()
    {
        // 회전 제어
        if (stopRotation == true)
            return;

        // UI를 클릭하면 회전 X
        if (Input.GetMouseButtonDown(0) == true || Input.GetMouseButtonDown(1) == true)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
        }

        // A Key, ◀ Key : 왼쪽으로 회전
        // D Key, ▶ Key : 오른쪽으로 회전
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
    private void SetRotation(float horizontal)
    {
        currentRotation_Y += horizontal * rotationSpeed;

        transform.localRotation = Quaternion.Euler(0f, currentRotation_Y, 0f);
    }

    // 파츠 부위 변경
    private void ChangePart(List<GameObject> partList, ref int currentIndex, bool isNext)
    {
        // 현재 부위 비활성화
        partList[currentIndex].SetActive(false);

        // ( ▶ ) Button
        if (isNext == true)
        {
            currentIndex++;

            // 다음버튼 눌렀을 때 현재 인덱스가 마지막이라면 처음으로 이동
            if (currentIndex >= partList.Count)
                currentIndex = 0;
        }
        // ( ◀ ) Button
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
    private SkinnedData SetSkinned(GameObject skinnedObject)
    {
        // SkinnedMeshRenderer 컴포넌트 받기
        SkinnedMeshRenderer skinnedMesh = skinnedObject.GetComponent<SkinnedMeshRenderer>();

        // 이름, localBounds, rootBone을 저장
        SkinnedData skinned = new SkinnedData()
        {
            SharedMeshName = skinnedMesh.name,
            Bounds = skinnedMesh.localBounds,
            RootBoneName = skinnedMesh.rootBone.name,
        };

        // bones 저장
        skinned.Bones = new List<string>();
        foreach (Transform child in skinnedMesh.bones)
            skinned.Bones.Add(child.name);

        return skinned;
    }
}
