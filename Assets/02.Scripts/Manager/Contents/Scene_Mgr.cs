using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 씬 전환을 위한 매니저
public class Scene_Mgr : MonoBehaviour
{
    #region Singleton
    public static Scene_Mgr Inst;

    private void Awake()
    {
        if (Inst == null)
            Inst = this;
        else
            Destroy(gameObject);
    }
    #endregion

    public void ChangeScene(Define_S.Scene scene)
    {
        if (scene == Define_S.Scene.Game)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
            
        }
        else if (scene == Define_S.Scene.Dungeon)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("DungeonScene");
        }
    }
}
