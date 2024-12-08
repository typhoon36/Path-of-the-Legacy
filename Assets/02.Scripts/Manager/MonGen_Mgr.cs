using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonGen_Mgr : MonoBehaviour
{
    public GameObject[] m_Monster; // ��ȯ�� ���͵�
    public Transform[] m_SpawnPos; // ��ȯ�� ��ġ��

    private List<GameObject> spawnedMonsters = new List<GameObject>(); // ��ȯ�� ���͵� ����Ʈ

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
                    yield break; // ��� ��ġ�� ���� ���� ���� ����
                }

                if (spawnedMonsters.Count < m_SpawnPos.Length)
                {
                    GameObject monster = Instantiate(m_Monster[Random.Range(0, m_Monster.Length)], m_SpawnPos[i].position, Quaternion.identity);
                    spawnedMonsters.Add(monster);
                }
            }
            yield return new WaitForSeconds(1f); // 1�ʸ��� ���� �õ�
        }
    }
}
