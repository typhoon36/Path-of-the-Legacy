using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonGen_Mgr : MonoBehaviour
{
    public GameObject[] m_Monster; // 소환할 몬스터들
    public Transform[] m_SpawnPos; // 소환할 위치들

    [HideInInspector] public List<GameObject> m_Monsters; // 소환된 몬스터들 리스트

    float m_ReSpawnTime = 10f; // 몬스터 리스폰 시간

    #region Singleton
    public static MonGen_Mgr Inst;
    private void Awake()
    {
        if (Inst == null)
            Inst = this;

        m_Monsters = new List<GameObject>(); // 리스트 초기화
    }
    #endregion

    private void Start()
    {
        StartCoroutine(SpawnMonsters());
    }

    public IEnumerator SpawnMonsters()
    {
        while (true)
        {
            for (int i = 0; i < m_SpawnPos.Length; i++)
            {
                bool isPositionOccupied = false;
                foreach (var monster in m_Monsters)
                {
                    if (Vector3.Distance(monster.transform.position, m_SpawnPos[i].position) < 0.1f)
                    {
                        isPositionOccupied = true;
                        break;
                    }
                }

                if (!isPositionOccupied)
                {
                    GameObject monster = Instantiate(m_Monster[Random.Range(0, m_Monster.Length)], m_SpawnPos[i].position, Quaternion.identity);
                    m_Monsters.Add(monster);
                }
            }
            yield return new WaitForSeconds(1f); // 1초마다 스폰 시도
        }
    }

    public IEnumerator ReSpawnMonsters(GameObject monster, int spawnIndex)
    {
        yield return new WaitForSeconds(2f); // 2초 후에 제거
        m_Monsters.Remove(monster);
        Destroy(monster);

        yield return new WaitForSeconds(m_ReSpawnTime - 2f); // 나머지 시간 대기 후 다시 스폰
        GameObject newMonster = Instantiate(m_Monster[Random.Range(0, m_Monster.Length)], m_SpawnPos[spawnIndex].position, Quaternion.identity);
        m_Monsters.Add(newMonster);
    }

    public void OnMonsterDeath(GameObject monster)
    {
        int index = m_Monsters.IndexOf(monster);
        if (index != -1)
        {
            StartCoroutine(ReSpawnMonsters(monster, index));
        }
    }
}
