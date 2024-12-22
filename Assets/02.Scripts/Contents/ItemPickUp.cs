using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickUp : MonoBehaviour
{
    public ItemData item;
    public int itemCount = 1;      // ������ ���� ����

    private float scanRange = 5f;     // �÷��̾� ��ĵ �Ÿ�

    public Text nameBarUI = null;
    private Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.FindObjectOfType<Player_Ctrl>().transform;
    }

    void FixedUpdate()
    {
        // �̸��� Null Check
        if (nameBarUI == null)
        {
            nameBarUI = GetComponentInChildren<Text>();
        }

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // scanRange��ŭ ������ Ȱ��ȭ
        if (distance <= scanRange)
            nameBarUI.gameObject.SetActive(true);
        else
            nameBarUI.gameObject.SetActive(false);
    }
}
