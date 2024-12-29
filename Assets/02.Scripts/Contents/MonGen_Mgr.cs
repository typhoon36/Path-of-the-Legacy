using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonGen_Mgr : MonoBehaviour
{
    public GameObject[] m_Monster; // ��ȯ�� ���͵�
    public Transform[] m_SpawnPos; // ��ȯ�� ��ġ��

    [HideInInspector] public List<GameObject> m_Monsters; // ��ȯ�� ���͵� ����Ʈ

    float m_ReSpawnTime = 10f; // ���� ������ �ð�

    #region Singleton
    public static MonGen_Mgr Inst;
    private void Awake()
    {
        if (Inst == null)
            Inst = this;

        m_Monsters = new List<GameObject>(); // ����Ʈ �ʱ�ȭ
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
            yield return new WaitForSeconds(1f); // 1�ʸ��� ���� �õ�
        }
    }

    public IEnumerator ReSpawnMonsters(GameObject monster, int spawnIndex)
    {
        yield return new WaitForSeconds(2f); // 2�� �Ŀ� ����
        m_Monsters.Remove(monster);
        Destroy(monster);

        yield return new WaitForSeconds(m_ReSpawnTime - 2f); // ������ �ð� ��� �� �ٽ� ����
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
