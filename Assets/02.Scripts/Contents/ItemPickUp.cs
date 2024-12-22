using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickUp : MonoBehaviour
{
    public ItemData item;
    public int itemCount = 1;      // 아이템 전용 개수

    private float scanRange = 5f;     // 플레이어 스캔 거리

    public Text nameBarUI = null;
    private Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.FindObjectOfType<Player_Ctrl>().transform;
    }

    void FixedUpdate()
    {
        // 이름바 Null Check
        if (nameBarUI == null)
        {
            nameBarUI = GetComponentInChildren<Text>();
        }

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // scanRange만큼 가까우면 활성화
        if (distance <= scanRange)
            nameBarUI.gameObject.SetActive(true);
        else
            nameBarUI.gameObject.SetActive(false);
    }
}
