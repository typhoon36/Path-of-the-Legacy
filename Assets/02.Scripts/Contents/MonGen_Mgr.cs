using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonGen_Mgr : MonoBehaviour
{
    public GameObject[] m_Monster; // 소환할 몬스터들
    [SerializeField] Transform[] m_SpawnPos; // 소환할 위치들

    [SerializeField] float m_SpawnRaidus = 10.0f; // 소환 범위
    [SerializeField] float m_SpawnTime = 2.0f; // 소환 주기
    [SerializeField] int m_SpawnCnt = 0; // 소환할 몬스터 수
    int m_OldCnt = 0;
    [SerializeField] int m_MaxSpawnCnt = 15; // 최대 소환할 몬스터 수

    bool IsSpawn = false;
    Coroutine m_SpawnCo;
    int m_CurSpawnIdx = 0; // 현재 소환 위치 인덱스

    #region Singleton
    public static MonGen_Mgr Inst;
    void Awake()
    {
        Inst = this;
    }
    #endregion

    void Start()
    {
        m_OldCnt = m_SpawnCnt;

    }

    void Update()
    {
        if ((m_OldCnt + m_SpawnCnt) < m_MaxSpawnCnt && !IsSpawn)
        {
            m_SpawnCo = StartCoroutine(SpawnMob());
        }
        else if ((m_OldCnt + m_SpawnCnt) >= m_MaxSpawnCnt && IsSpawn)
        {
            if (m_SpawnCo != null)
            {
                StopCoroutine(m_SpawnCo);
                m_SpawnCo = null;
            }
        }
    }

    //몬스터 스폰
    IEnumerator SpawnMob()
    {
        IsSpawn = true; // 코루틴 실행 중으로 설정
        m_SpawnCnt++; // 소환할 몬스터 수 증가

        yield return new WaitForSeconds(m_SpawnTime);

        if (m_Monster.Length == 0 || m_SpawnPos.Length == 0)
        {
            m_SpawnCnt--; // 소환할 몬스터 수 감소
            IsSpawn = false;
            yield break;
        }

        // 순차적으로 소환 위치 선택
        Transform a_Pos = m_SpawnPos[m_CurSpawnIdx];
        m_CurSpawnIdx = (m_CurSpawnIdx + 1) % m_SpawnPos.Length;

        Vector3 spawnPosition = a_Pos.position + new Vector3(Random.Range(-m_SpawnRaidus, m_SpawnRaidus), 0, Random.Range(-m_SpawnRaidus, m_SpawnRaidus));
        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPosition, out hit, m_SpawnRaidus, NavMesh.AllAreas))
        {
            GameObject a_Obj = Instantiate(m_Monster[Random.Range(0, m_Monster.Length)], hit.position, Quaternion.identity);
            NavMeshAgent a_Nav = a_Obj.GetComponent<NavMeshAgent>();

            Vector3 a_RandPos = hit.position; // 기본값으로 초기화
            int a_Maxtemp = 10; // 최대 시도 횟수
            int a_Temp = 0;

            while (a_Temp < a_Maxtemp)
            {
                Vector3 a_RandDir = Random.insideUnitSphere * m_SpawnRaidus;
                a_RandDir.y = 0;
                a_RandPos = a_Pos.position + a_RandDir;

                NavMeshPath a_Path = new NavMeshPath();

                if (a_Nav.CalculatePath(a_RandPos, a_Path))
                {
                    a_Obj.transform.position = a_RandPos;
                    break;
                }

                a_Temp++;
            }

            a_Nav.nextPosition = a_Obj.transform.position;
            a_Obj.GetComponent<Monster_Ctrl>().m_SpawnPos = a_RandPos;

            // 플레이어를 바라보도록 회전
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Vector3 directionToPlayer = player.transform.position - a_Obj.transform.position;
                directionToPlayer.y = 0; // y축 회전 방지
                a_Obj.transform.rotation = Quaternion.LookRotation(directionToPlayer);
            }

            m_OldCnt++; // 소환된 몬스터 수 증가
        }
        else
        {
            Debug.LogError("NavMesh 위에 몬스터를 배치할 수 없습니다.");
        }

        m_SpawnCnt--; // 소환할 몬스터 수 감소
        IsSpawn = false; // 코루틴 실행 완료로 설정
    }
}
