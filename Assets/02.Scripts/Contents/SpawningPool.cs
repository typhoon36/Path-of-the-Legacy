using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


//몬스터 스폰 관리
public class SpawningPool : MonoBehaviour
{
    public GameObject m_MonsterObj;    // 몬스터 Prefab

    [SerializeField] Vector3 m_SpawnPos;              // 스폰 위치

    [SerializeField] float m_SpawnRadius = 5f;      // 스폰 최대 거리

    [SerializeField] float m_SpawnTime = 5f;        // 스폰 최대 시간

    [SerializeField] int m_MonsterCount = 0;      // 현재 몬스터 수
    int m_OldCount = 0;     

    [SerializeField] int m_MaxCount = 0;  // 최대 몬스터 수

    // 몬스터 수 증가
    public void AddMonsterCount(Transform a_Parent, int a_Value)
    {
        // 스포너 부모 체크
        if (transform == a_Parent) this.m_MonsterCount += a_Value;
    }

    // 최대 몬스터 지정
    public void SetKeepMonsterCount(int a_Count) { this.m_MaxCount = a_Count; }

    void Start()
    {
        Managers.Game.OnSpawnEvent -= AddMonsterCount;
        Managers.Game.OnSpawnEvent += AddMonsterCount;
    }

    void Update() { while ((m_OldCount + m_MonsterCount) < m_MaxCount) StartCoroutine("ReserveSpawn"); }

    // 몬스터 스폰 설정
    IEnumerator ReserveSpawn()
    {
   
        m_OldCount++;

        yield return new WaitForSeconds(Random.Range(1, m_SpawnTime));

        // 몬스터 생성
        GameObject a_Obj = Managers.Game.Spawn(Define.WorldObject.Monster, m_MonsterObj, transform);
        NavMeshAgent a_Nav = a_Obj.GetOrAddComponent<NavMeshAgent>();

        Vector3 a_RandPos;

        // 소환 가능한 위치를 찾을 때까지 루프
        while (true)
        {
            // 원 형태 랜덤 벡터 지정
            Vector3 a_RandDir = Random.insideUnitSphere * m_SpawnRadius;
            a_RandDir.y = 0;
            a_RandPos = m_SpawnPos + a_RandDir;

            NavMeshPath a_Path = new NavMeshPath();
            if (a_Nav.CalculatePath(a_RandPos, a_Path))   // randPos 위치에 소환 가능 여부 확인
            {
                a_Obj.transform.position = a_RandPos;
                break;
            }
        }

        // 위치 설정
        a_Nav.nextPosition = a_RandPos;
        a_Obj.GetComponent<MonsterController>().spawnPos = a_RandPos;

        m_OldCount--;
    }
}
