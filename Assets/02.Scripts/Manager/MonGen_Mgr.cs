using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonGen_Mgr : MonoBehaviour
{
    public GameObject[] m_Monster; // 소환할 몬스터들
    public Transform[] m_SpawnPos; // 소환할 위치들

    private List<GameObject> spawnedMonsters = new List<GameObject>(); // 소환된 몬스터들 리스트

    #region Singleton
    public static MonGen_Mgr Inst;
    private void Awake()
    {
        if (Inst == null)
            Inst = this;
    }
    #endregion

    private void Start()
    {
        StartCoroutine(SpawnMonsters());
    }

    private IEnumerator SpawnMonsters()
    {
        while (true)
        {
            for (int i = 0; i < m_SpawnPos.Length; i++)
            {
                if (spawnedMonsters.Count >= m_SpawnPos.Length)
                {
                    yield break; // 모든 위치가 가득 차면 스폰 중지
                }

                if (spawnedMonsters.Count < m_SpawnPos.Length)
                {
                    GameObject monster = Instantiate(m_Monster[Random.Range(0, m_Monster.Length)], m_SpawnPos[i].position, Quaternion.identity);
                    spawnedMonsters.Add(monster);
                }
            }
            yield return new WaitForSeconds(1f); // 1초마다 스폰 시도
        }
    }
}
