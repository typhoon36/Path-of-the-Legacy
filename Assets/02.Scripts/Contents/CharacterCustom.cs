using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public class CharacterCustom : MonoBehaviour
{
    public bool IsStopRot = false;    // 회전 제어

    [SerializeField] float m_RotSpeed = 3.5f;     // 회전 속도
    float m_CurRotY = 0.01f;    // 캐릭터 Y 회전값 

    //파트별 리스트
    [SerializeField] List<GameObject> HairList = new List<GameObject>();
    [SerializeField] List<GameObject> EyeBrowList = new List<GameObject>();
    [SerializeField] List<GameObject> EarsList = new List<GameObject>();
    [SerializeField] List<GameObject> FacialHairList = new List<GameObject>();
    [SerializeField] List<GameObject> NoseList = new List<GameObject>();

    // 현재 선택된 파츠 인덱스
    int m_CurHairIdx = 0;
    int m_CurEyebrowsIdx = 0;
    int m_CurEarIdx = 0;
    int m_CurFacialHairIdx = 0;
    int m_CurNoseIdx = 0;


    void Update() { CharaterRotate(); }

    
    public void NextPart(Define.DefaultPart a_PartType, bool IsNext)
    {
        // 부위 타입에 맞게 변경
        switch (a_PartType)
        {
            case Define.DefaultPart.Hair:
                ChangePart(HairList, ref m_CurHairIdx, IsNext);
                break;
            case Define.DefaultPart.Eyebrows:
                ChangePart(EyeBrowList, ref m_CurEyebrowsIdx, IsNext);
                break;
            case Define.DefaultPart.Ears:
                ChangePart(EarsList, ref m_CurEarIdx, IsNext);
                break;

            case Define.DefaultPart.FacialHair:
                ChangePart(FacialHairList, ref m_CurFacialHairIdx, IsNext);
                break;
            case Define.DefaultPart.Nose:
                ChangePart(NoseList, ref m_CurNoseIdx, IsNext);
                break;

        }
    }

    // 커스텀 저장
    public void SaveCustom()
    {

        Managers.Game.DefaultPart = new Dictionary<Define.DefaultPart, SkinnedData>();

        Managers.Game.DefaultPart.Add(Define.DefaultPart.Hair, SetSkinned(HairList[m_CurHairIdx]));
        Managers.Game.DefaultPart.Add(Define.DefaultPart.Eyebrows, SetSkinned(EyeBrowList[m_CurEyebrowsIdx]));
        Managers.Game.DefaultPart.Add(Define.DefaultPart.Ears, SetSkinned(EarsList[m_CurEarIdx]));
        Managers.Game.DefaultPart.Add(Define.DefaultPart.FacialHair, SetSkinned(FacialHairList[m_CurFacialHairIdx]));
        Managers.Game.DefaultPart.Add(Define.DefaultPart.Nose, SetSkinned(NoseList[m_CurNoseIdx]));
    }

    //캐릭터 회전
    void CharaterRotate()
    {
        if (IsStopRot == true) return;


        if (Input.GetMouseButtonDown(0) == true || Input.GetMouseButtonDown(1) == true)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
        }

        //키보드 입력에 따라 회전
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.LeftArrow) ||
            Input.GetKey(KeyCode.RightArrow))
        {
            SetRotate(-Input.GetAxis("Horizontal"));
        }
        // 마우스 입력에 따라 회전
        else if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            SetRotate(-Input.GetAxis("Mouse X"));
        }
    }

    // 회전 설정
    void SetRotate(float a_Horizontal)
    {
        m_CurRotY += a_Horizontal * m_RotSpeed;

        transform.localRotation = Quaternion.Euler(0f, m_CurRotY, 0f);
    }

    // 파츠 부위 변경
    void ChangePart(List<GameObject> a_PartList, ref int a_CurIdx, bool IsNext)
    {
        // 현재 부위 비활성화
        a_PartList[a_CurIdx].SetActive(false);

        if (IsNext == true)
        {
            // 다음 인덱스 설정
            a_CurIdx++;

            // 인덱스가 리스트 크기 이상일 경우 처음 인덱스로 설정
            if (a_CurIdx >= a_PartList.Count) a_CurIdx = 0;
        }

        else
        {
            // 이전 인덱스 설정
            a_CurIdx--;
            // 인덱스가 음수일 경우 마지막 인덱스로 설정
            if (a_CurIdx < 0) a_CurIdx = a_PartList.Count - 1;
        }

        // 현재 부위 활성화
        a_PartList[a_CurIdx].SetActive(true);
    }


    SkinnedData SetSkinned(GameObject a_SkinnedObj)
    {

        SkinnedMeshRenderer a_SkinnedMesh = a_SkinnedObj.GetComponent<SkinnedMeshRenderer>();


        SkinnedData a_Skinned = new SkinnedData()
        {
            SharedMeshName = a_SkinnedMesh.name,
            Bounds = a_SkinnedMesh.localBounds,
            RootBoneName = a_SkinnedMesh.rootBone.name,
        };


        a_Skinned.Bones = new List<string>();
        foreach (Transform child in a_SkinnedMesh.bones)
            a_Skinned.Bones.Add(child.name);

        return a_Skinned;
    }
}
