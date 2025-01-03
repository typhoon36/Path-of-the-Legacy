using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAnimation : MonoBehaviour
{

    public GameObject[] frames;
    public GameObject circle;
    public float pacing;

    private int currentFrame;
    private int previousFrame;
    [Range(0, 50)]
    public float frameOffset = 0f;
    public float frameSpeed = 10f;

    private float startFrame;
    [HideInInspector] public float timeLoop = 0f;

    public bool loop;
    float m_CacTime = 0.0f;
    int m_CurIdx = 0;
    int m_Idx = 0;

    void Start()
    {

        for (int i = 0; i < frames.Length; i++)
        { 
            frames[i].SetActive(false);
        }

        if (circle != null) 
            circle.SetActive(false);

    }


    void Update()
    {

        m_CacTime = m_CacTime + Time.deltaTime;

        if (0.10f < m_CacTime)
        {
            if (0 <= m_CacTime && m_CacTime <= (frames.Length - 1))
            {
                for (m_Idx = 0; m_Idx < frames.Length; m_Idx++)
                {
                    if (m_CurIdx == m_Idx)
                        frames[m_Idx].SetActive(true);
                    else
                        frames[m_Idx].SetActive(false);
                }
            }

            m_CurIdx = m_CurIdx + 1;
            if (frames.Length + 1 < m_CurIdx)
                m_CurIdx = 0;

            m_CacTime = 0.0f;
        }




    }
}