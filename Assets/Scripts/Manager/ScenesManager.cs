using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : SingletonMono<ScenesManager>
{
    
    void Awake()
    {
        MessageManager.AddListener(DefineManager.OpenLaunchScene, () =>
         {
             SceneManager.LoadScene("Launch");
         });

        MessageManager.AddListener(DefineManager.OpenLevelScene, () =>
        {
            SceneManager.LoadScene("Level");
        });

        MessageManager.AddListener(DefineManager.OpenGameScene, () =>
        {
            SceneManager.LoadScene("Game");
        });

        MessageManager.AddListener(DefineManager.OpenOverScene, () =>
        {
            SceneManager.LoadScene("Over");
        });
    }
    
    void OnDestroy()
    {
        MessageManager.RemoveListener(DefineManager.OpenLaunchScene, () =>
        {
            SceneManager.LoadScene("Launch");
        });

        MessageManager.RemoveListener(DefineManager.OpenLevelScene, () =>
        {
            SceneManager.LoadScene("Level");
        });

        MessageManager.RemoveListener(DefineManager.OpenGameScene, () =>
        {
            SceneManager.LoadScene("Game");
        });

        MessageManager.RemoveListener(DefineManager.OpenOverScene, () =>
        {
            SceneManager.LoadScene("Over");
        });
    }
}
