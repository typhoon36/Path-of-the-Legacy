using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 플레이어 애니메이션 이벤트
public class PlayerAnimEvent : MonoBehaviour
{
    [SerializeField] CapsuleCollider capsuleCollider;

    int nextSkillIndex = 0;     // 스킬 콤보 체크용

    const int X_Axis = 0, Y_Axis = 1, Z_Axis = 2;

    #region 공격 사거리

    // 공격 사이즈 클래스
    class AttackSize
    {
        public float x;
        public float y;
        public float z;
        public float redius;
        public float height;
        public int direction;   // x: 0, y: 1, z: 2
    }

    // Id 101 공격 범위 
    AttackSize skill101 = new AttackSize()
    {
        x = 0,
        y = 0,
        z = -0.35f,
        redius = 2.35f,
        height = 4.5f,
        direction = Y_Axis,
    };

    // Id 102 공격 범위
    AttackSize[] skill102 = new AttackSize[]
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

    // Id 103 공격 범위 
    AttackSize skill103 = new AttackSize()
    {
        x = -0.4f,
        y = 0,
        z = -0.6f,
        redius = 4.4f,
        height = 8.7f,
        direction = Y_Axis,
    };

    // Id 104 공격 범위 
    AttackSize skill104 = new AttackSize()
    {
        x = 0,
        y = 0,
        z = -0.4f,
        redius = 2.8f,
        height = 5.5f,
        direction = Y_Axis,
    };

    // Id 105 공격 범위 
    AttackSize skill105 = new AttackSize()
    {
        x = 0,
        y = 0,
        z = 0,
        redius = 1.2f,
        height = 12f,
        direction = Z_Axis,
    };

    // Id 106 공격 범위 
    AttackSize[] skill106 = new AttackSize[]
    {
        new AttackSize()
        {
            x = 0f, y = 0, z = 0.13f, redius = 1.23f, height = 3.4f, direction = Y_Axis,
        },
        new AttackSize()
        {
            x = 0f, y = 0f, z = 3.5f, redius = 3f, height = 9.4f, direction = Y_Axis,
        },
        new AttackSize()
        {
            x = 0f, y = 0f, z = 3.5f, redius = 3f, height = 9.4f, direction = Y_Axis,
        },
        new AttackSize()
        {
            x = 0f, y = 0f, z = 3.5f, redius = 3f, height = 9.4f, direction = Y_Axis,
        },
    };


    #endregion

    // 기본 검 공격
    public void OnBasicAttack()
    {
        capsuleCollider.gameObject.SetActive(true);
    }

    // skill 101 
    public void OnTripleSlash()
    {
        OnSize(skill101);
    }

    // skill 102 
    public void OnRisingSlash()
    {
        OnSize(skill102[nextSkillIndex]);

        ++nextSkillIndex;
        if (nextSkillIndex == skill102.Length)
            nextSkillIndex = 0;
    }

    // skill 103 
    public void OnRotationBlade()
    {
        OnSize(skill103);
    }

    // skill 104 
    public void OnDarkBlade()
    {
        OnSize(skill104);
    }

    // skill 105 
    public void OnBigSwordSlash()
    {
        OnSize(skill105);
    }

    // skill 106 
    public void OnBladeAnnihilation()
    {
        OnSize(skill106[nextSkillIndex]);

        //++nextSkillIndex;
        //if (nextSkillIndex == skill106.Length)
        //    nextSkillIndex = 0;
    }


    void OnSize(AttackSize size)
    {
        capsuleCollider.gameObject.SetActive(true);
        SetSize(size);
    }

    void SetSize(AttackSize size)
    {
        capsuleCollider.direction = size.direction;
        capsuleCollider.center = new Vector3(size.x, size.y, size.z);
        capsuleCollider.radius = size.redius;
        capsuleCollider.height = size.height;
    }
}