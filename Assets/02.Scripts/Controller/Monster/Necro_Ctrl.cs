using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//���� ���� 
public class Necro_Ctrl : Monster_Ctrl
{
    [SerializeField] float m_ArrowRange = 5f;

    Portal m_ExitPortal;


    public override void Init()
    {
        //�θ��� Monster_Ctrl�� Init() ȣ��
        base.Init();

    }

}
