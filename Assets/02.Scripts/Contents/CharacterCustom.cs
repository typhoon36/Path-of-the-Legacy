using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


// 캐릭터 커스텀 제어 ( 커스텀 부위 : 머리카락, 눈썹, 수염, 귀,코 )
public class CharacterCustom : MonoBehaviour
{
    public bool StopRotation = false;    // 회전 제어

    [SerializeField] float m_RotSpeed = 3.5f;     // 회전 속도
    float CurRot_Y = 0.01f;    // 캐릭터 Y 회전값 

    // 부위별 파츠 리스트
    [SerializeField] List<GameObject> HairList = new List<GameObject>();
    [SerializeField] List<GameObject> EyebrowsList = new List<GameObject>();
    [SerializeField] List<GameObject> FacialHairList = new List<GameObject>();
    [SerializeField] List<GameObject> EarsList = new List<GameObject>();
    [SerializeField] List<GameObject> NoseList = new List<GameObject>();

    // 부위별 현재 List index
    int m_CurHairIdx = 0;
    int m_CurEyebrowsIdx = 0;
    int m_CurFacialHairIdx = 0;
    int m_CurEarsIdx = 0;
    int m_CurNoseIdx = 0;


    void Update() { CharaterRotation(); }

    // 파츠 변경 버튼을 누를 때 호출
    public void NextPart(Define.DefaultPart a_PartType, bool IsNext)
    {
        // 부위 타입에 맞게 적용
        switch (a_PartType)
        {
            case Define.DefaultPart.Hair:
                ChangePart(HairList, ref m_CurHairIdx, IsNext);
                break;
            case Define.DefaultPart.Eyebrows:
                ChangePart(EyebrowsList, ref m_CurEyebrowsIdx, IsNext);
                break;
            case Define.DefaultPart.FacialHair:
                ChangePart(FacialHairList, ref m_CurFacialHairIdx, IsNext);
                break;
            case Define.DefaultPart.Ears:
                ChangePart(EarsList, ref m_CurEarsIdx, IsNext);
                break;
            case Define.DefaultPart.Nose:
                ChangePart(NoseList, ref m_CurNoseIdx, IsNext);
                break;
        }
    }

    // 커스텀 저장 ( 저장 버튼을 누를 때 호출 )
    public void SaveCustom()
    {
        // 딕셔너리 생성
        Managers.Game.DefaultPart = new Dictionary<Define.DefaultPart, SkinnedData>();

        // GameManager의 데이터에 저장
        Managers.Game.DefaultPart.Add(Define.DefaultPart.Hair, SetSkinned(HairList[m_CurHairIdx]));
        Managers.Game.DefaultPart.Add(Define.DefaultPart.Eyebrows, SetSkinned(EyebrowsList[m_CurEyebrowsIdx]));
        Managers.Game.DefaultPart.Add(Define.DefaultPart.FacialHair, SetSkinned(FacialHairList[m_CurFacialHairIdx]));
        Managers.Game.DefaultPart.Add(Define.DefaultPart.Ears, SetSkinned(EarsList[m_CurEarsIdx]));
        Managers.Game.DefaultPart.Add(Define.DefaultPart.Nose, SetSkinned(NoseList[m_CurNoseIdx]));
    }

    // 캐릭터 회전 (Update)
    void CharaterRotation()
    {
        // 회전 제어
        if (StopRotation == true) return;

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
    void SetRotation(float Horizontal)
    {
        CurRot_Y += Horizontal * m_RotSpeed;

        transform.localRotation = Quaternion.Euler(0f, CurRot_Y, 0f);
    }

    // 파츠 부위 변경
    void ChangePart(List<GameObject> a_PartList, ref int a_CurIdx, bool IsNext)
    {
        // 현재 부위 비활성화
        a_PartList[a_CurIdx].SetActive(false);

        // ( 다음 ) Button
        if (IsNext == true)
        {
            a_CurIdx++;

            // 다음버튼 눌렀을 때 현재 인덱스가 마지막이라면 처음으로 이동
            if (a_CurIdx >= a_PartList.Count)
                a_CurIdx = 0;
        }
        // ( 이전 ) Button
        else
        {
            a_CurIdx--;

            // 뒤로버튼 눌렀을 때 현재 인덱스가 처음이라면 마지막으로 이동
            if (a_CurIdx < 0)
                a_CurIdx = a_PartList.Count - 1;
        }

        // 변경된 부위 활성화
        a_PartList[a_CurIdx].SetActive(true);
    }

    // SkinnedMeshRenderer 필요 정보 저장
    SkinnedData SetSkinned(GameObject a_SkinnedObj)
    {
        // SkinnedMeshRenderer 컴포넌트 받기
        SkinnedMeshRenderer a_SkinnedMesh = a_SkinnedObj.GetComponent<SkinnedMeshRenderer>();

        // 이름, localBounds, rootBone을 저장
        SkinnedData a_Skinned = new SkinnedData()
        {
            sharedMeshName = a_SkinnedMesh.name,
            bounds = a_SkinnedMesh.localBounds,
            rootBoneName = a_SkinnedMesh.rootBone.name,
        };

        // bones 저장
        a_Skinned.bones = new List<string>();
        foreach (Transform child in a_SkinnedMesh.bones)
            a_Skinned.bones.Add(child.name);

        return a_Skinned;
    }
}
