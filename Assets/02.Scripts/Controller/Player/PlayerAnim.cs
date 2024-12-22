using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    [SerializeField]
    private CapsuleCollider Collider;

    private int m_NextSkillIdx = 0;     // 스킬 콤보 체크용

    private const int X_Axis = 0, Y_Axis = 1, Z_Axis = 2;

    #region 사거리
    // 공격 사이즈 클래스
    private class AttackSize
    {
        public float x;
        public float y;
        public float z;
        public float redius;
        public float height;
        public int direction;   // x: 0, y: 1, z: 2
    }

    // Id 101 
    private AttackSize skill101 = new AttackSize()
    {
        x = 0,
        y = 0,
        z = -0.35f,
        redius = 2.35f,
        height = 4.5f,
        direction = Y_Axis,
    };

    // Id 102 
    private AttackSize[] skill102 = new AttackSize[]
    {
        new AttackSize()
        {
            x = 0.3f, y = 0, z = 1.23f, redius = 0.5f, height = 3.5f, direction = Z_Axis,
        },
        new AttackSize()
        {
            x = 0.43f, y = 0.56f, z = 1f, redius = 0.93f, height = 3.6f, direction = Y_Axis,
        },
        new AttackSize()
        {
            x = 0, y = 0, z = 0f, redius = 2.35f, height = 4.5f, direction = Y_Axis,
        },
    };
    #endregion

    // 기본 검 공격
    private void OnBasicAttack()
    {
        Collider.gameObject.SetActive(true);
    }

    // skill 101 : 
    private void OnTripleSlash()
    {
        OnSize(skill101);
    }

    // skill 102 : 
    private void OnRisingSlash()
    {
        OnSize(skill102[m_NextSkillIdx]);

        ++m_NextSkillIdx;
        if (m_NextSkillIdx == skill102.Length)
            m_NextSkillIdx = 0;
    }

    private void OnSize(AttackSize size)
    {
        Collider.gameObject.SetActive(true);
        SetSize(size);
    }

    private void SetSize(AttackSize size)
    {
        Collider.direction = size.direction;
        Collider.center = new Vector3(size.x, size.y, size.z);
        Collider.radius = size.redius;
        Collider.height = size.height;
    }
}
