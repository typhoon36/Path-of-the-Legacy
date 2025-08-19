using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#pragma warning disable 618, 649
namespace UnityStandardAssets.Utilityity
{
#if UNITY_EDITOR

    [ExecuteInEditMode]
#endif
    public class PlatformSpecificContent : MonoBehaviour
#if UNITY_EDITOR
        , UnityEditor.Build.IActiveBuildTargetChanged
#endif
    {
         enum BuildTargetGroup
        {
            Standalone,
            Mobile
        }

        [SerializeField]
         BuildTargetGroup m_BuildTargetGroup;
        [SerializeField]
         GameObject[] m_Content = new GameObject[0];
        [SerializeField]
         MonoBehaviour[] m_MonoBehaviours = new MonoBehaviour[0];
        [SerializeField]
         bool m_ChildrenOfThisObject;

#if !UNITY_EDITOR
	void OnEnable()
	{
		CheckEnableContent();
	}
#else
        public int callbackOrder
        {
            get
            {
                return 1;
            }
        }
#endif

#if UNITY_EDITOR

         void OnEnable()
        {
            EditorApplication.update += Update;
        }


         void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            CheckEnableContent();
        }

         void Update()
        {
            CheckEnableContent();
        }
#endif


         void CheckEnableContent()
        {
#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_TIZEN)
		if (m_BuildTargetGroup == BuildTargetGroup.Mobile)
		{
			EnableContent(true);
		} else {
			EnableContent(false);
		}
#endif

#if !(UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_TIZEN)
            if (m_BuildTargetGroup == BuildTargetGroup.Mobile)
            {
                EnableContent(false);
            }
            else
            {
                EnableContent(true);
            }
#endif
        }


         void EnableContent(bool enabled)
        {
            if (m_Content.Length > 0)
            {
                foreach (var g in m_Content)
                {
                    if (g != null)
                    {
                        g.SetActive(enabled);
                    }
                }
            }
            if (m_ChildrenOfThisObject)
            {
                foreach (Transform t in transform)
                {
                    t.gameObject.SetActive(enabled);
                }
            }
            if (m_MonoBehaviours.Length > 0)
            {
                foreach (var monoBehaviour in m_MonoBehaviours)
                {
                    monoBehaviour.enabled = enabled;
                }
            }
        }
    }
}