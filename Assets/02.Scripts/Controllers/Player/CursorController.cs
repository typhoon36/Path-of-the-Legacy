using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 마우스 커서 제어
public class CursorController : MonoBehaviour
{
    // 마우스 커서 상태
    public enum CursorType
    {
        None,
        Attack,
        Hand,
        Loot,
    }

    CursorType m_CursorType = CursorType.None;

    RaycastHit hit;
    Texture2D m_AttackIcon;  // 공격 icon
    Texture2D m_HandIcon;    // 기본 icon
    Texture2D m_LootIcon;    // npc icon

    int m_Mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster) | (1 << (int)Define.Layer.Npc);

    void Start()
    {
        // 커서 텍스쳐 가져오기
        m_AttackIcon = Managers.Resource.Load<Texture2D>("Textures/Cursor/Attack");
        m_HandIcon = Managers.Resource.Load<Texture2D>("Textures/Cursor/Hand");
        m_LootIcon = Managers.Resource.Load<Texture2D>("Textures/Cursor/Loot");

        // Hand icon 커서에 적용 
        Cursor.SetCursor(m_HandIcon, new Vector2(m_HandIcon.width / 3.1f, 0), CursorMode.Auto);
        m_CursorType = CursorType.Hand;
    }

    void Update() { CursorUpdate(); }

    void CursorUpdate()
    {
        // 꾹 누르면 아이콘 유지
        if (Input.GetMouseButton(0)) return;

        // 마우스 포인트 가져오기
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100f, m_Mask))
        {
            // NPC
            if (hit.collider.gameObject.layer == (int)Define.Layer.Npc)
            {
                if (m_CursorType != CursorType.Loot)
                {
                    Cursor.SetCursor(m_LootIcon, new Vector2(m_LootIcon.width / 4.5f, m_LootIcon.height / 2), CursorMode.Auto);
                    m_CursorType = CursorType.Loot;
                }
                return;
            }
            // Monster
            else if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
            {
                if (m_CursorType != CursorType.Attack)
                {
                    Cursor.SetCursor(m_AttackIcon, new Vector2(m_AttackIcon.width / 3.9f, 0), CursorMode.Auto);
                    m_CursorType = CursorType.Attack;
                }
                return;
            }
            // Default
            else if (m_CursorType != CursorType.Hand)
            {
                Cursor.SetCursor(m_HandIcon, new Vector2(m_HandIcon.width / 3.1f, 0), CursorMode.Auto);
                m_CursorType = CursorType.Hand;
            }
        }
    }
}