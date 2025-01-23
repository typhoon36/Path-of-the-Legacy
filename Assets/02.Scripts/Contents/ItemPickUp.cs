using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ItemPickUp : MonoBehaviour
{
    public ItemData m_Item;
    public int m_ItemCount = 1;      // ������ ���� ����

    private float m_ScanRange = 5f;     // �÷��̾� ��ĵ �Ÿ�

    public Text m_NameTxt = null;
    Transform m_PlayerDist;

    void Start()
    {
        m_PlayerDist = GameObject.FindObjectOfType<Player_Ctrl>().transform;
    }

    void FixedUpdate()
    {
        // �̸� Null Check
        if (m_NameTxt == null)
            m_NameTxt = GetComponentInChildren<Text>();
        

        float a_Dist = Vector3.Distance(transform.position, m_PlayerDist.position);

        // scanRange��ŭ ������ Ȱ��ȭ
        if (a_Dist <= m_ScanRange)
            m_NameTxt.gameObject.SetActive(true);
        else
            m_NameTxt.gameObject.SetActive(false);
    }
}
