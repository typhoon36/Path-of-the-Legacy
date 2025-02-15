using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor_Ctrl : MonoBehaviour
{
    public enum CursorType
    {
        None,
        Attack,
        Hand,
        Interact,
    }

    CursorType m_CursorType = CursorType.None;

    RaycastHit m_Hit;
    Texture2D m_HandIcon;    // 기본 icon
    Texture2D m_InteractIcon;    // 상호작용 icon

    int m_Mask = (1 << (int)Define_S.Layer.Ground)| (1 << (int)Define_S.Layer.Monster)| (1 << (int)Define_S.Layer.Npc);

    void Start()
    {
        // 모바일 환경 감지
        if (Application.isMobilePlatform)
        {
            this.enabled = false;//모바일 환경에서는 조이스틱 이동이기에 false
            return;
        }

        m_HandIcon = Resources.Load<Texture2D>("Cursor/Hand");
        m_InteractIcon = Resources.Load<Texture2D>("Cursor/Interact");

        Cursor.SetCursor(m_HandIcon, new Vector2(m_HandIcon.width / 3.1f, 0), CursorMode.Auto);
        m_CursorType = CursorType.Hand;
    }

    void Update()
    {
        CursorUpdate();
    }

    void CursorUpdate()
    {
        Ray a_Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(a_Ray, 600f, m_Mask);

        Debug.DrawRay(a_Ray.origin, a_Ray.direction * 600f, Color.green);

        if (hits.Length > 0)
        {
            m_Hit = hits[0];
            float closestDistance = m_Hit.distance;

            foreach (var hit in hits)
            {
                if (hit.distance < closestDistance)
                {
                    m_Hit = hit;
                    closestDistance = hit.distance;
                }
            }

            if (m_Hit.collider.gameObject.layer == (int)Define_S.Layer.Npc)
            {
                if (m_CursorType != CursorType.Interact)
                {
                    Cursor.SetCursor(m_InteractIcon, new Vector2(m_InteractIcon.width / 4.5f, m_InteractIcon.height / 2), CursorMode.Auto);
                    m_CursorType = CursorType.Interact;
                }
            }
            else
            {
                if (m_CursorType != CursorType.Hand)
                {
                    Cursor.SetCursor(m_HandIcon, new Vector2(m_HandIcon.width / 3.1f, 0), CursorMode.Auto);
                    m_CursorType = CursorType.Hand;
                }
            }
        }
    }
}
