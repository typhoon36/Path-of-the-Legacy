using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonGen_Mgr : MonoBehaviour
{
    public GameObject[] m_Monster; // ��ȯ�� ���͵�
    [SerializeField] Transform[] m_SpawnPos; // ��ȯ�� ��ġ��

    [SerializeField] float m_SpawnRaidus = 10.0f; // ��ȯ ����
    [SerializeField] float m_SpawnTime = 2.0f; // ��ȯ �ֱ�
    [SerializeField] int m_SpawnCnt = 0; // ��ȯ�� ���� ��
    int m_OldCnt = 0;
    [SerializeField] int m_MaxSpawnCnt = 15; // �ִ� ��ȯ�� ���� ��

    bool IsSpawn = false;
    Coroutine m_SpawnCo;
    int m_CurSpawnIdx = 0; // ���� ��ȯ ��ġ �ε���

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

    //���� ����
    IEnumerator SpawnMob()
    {
        IsSpawn = true; // �ڷ�ƾ ���� ������ ����
        m_SpawnCnt++; // ��ȯ�� ���� �� ����

        yield return new WaitForSeconds(m_SpawnTime);

        if (m_Monster.Length == 0 || m_SpawnPos.Length == 0)
        {
            m_SpawnCnt--; // ��ȯ�� ���� �� ����
            IsSpawn = false;
            yield break;
        }

        // ���������� ��ȯ ��ġ ����
        Transform a_Pos = m_SpawnPos[m_CurSpawnIdx];
        m_CurSpawnIdx = (m_CurSpawnIdx + 1) % m_SpawnPos.Length;

        Vector3 spawnPosition = a_Pos.position + new Vector3(Random.Range(-m_SpawnRaidus, m_SpawnRaidus), 0, Random.Range(-m_SpawnRaidus, m_SpawnRaidus));
        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPosition, out hit, m_SpawnRaidus, NavMesh.AllAreas))
        {
            GameObject a_Obj = Instantiate(m_Monster[Random.Range(0, m_Monster.Length)], hit.position, Quaternion.identity);
            NavMeshAgent a_Nav = a_Obj.GetComponent<NavMeshAgent>();

            Vector3 a_RandPos = hit.position; // �⺻������ �ʱ�ȭ
            int a_Maxtemp = 10; // �ִ� �õ� Ƚ��
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

            // �÷��̾ �ٶ󺸵��� ȸ��
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Vector3 directionToPlayer = player.transform.position - a_Obj.transform.position;
                directionToPlayer.y = 0; // y�� ȸ�� ����
                a_Obj.transform.rotation = Quaternion.LookRotation(directionToPlayer);
            }

            m_OldCnt++; // ��ȯ�� ���� �� ����
        }
        else
        {
            Debug.LogError("NavMesh ���� ���͸� ��ġ�� �� �����ϴ�.");
        }

        m_SpawnCnt--; // ��ȯ�� ���� �� ����
        IsSpawn = false; // �ڷ�ƾ ���� �Ϸ�� ����
    }
}
