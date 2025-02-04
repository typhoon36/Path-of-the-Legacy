using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//보스 몬스터 
public class Necro_Ctrl : Monster_Ctrl
{
    [SerializeField] float m_ArrowRange = 5f;

    Portal m_ExitPortal;


    public override void Init()
    {
        //부모인 Monster_Ctrl의 Init() 호출
        base.Init();

    }

}
