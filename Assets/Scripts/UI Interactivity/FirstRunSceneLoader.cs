using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstRunSceneLoader : MonoBehaviour
{
    public SceneLoader sceneLoader;
    public StaticData staticData;

    void Awake()
    {
        staticData = StaticData.Instance;

        if (!staticData.showTutorial)
        {
            if (PlayerPrefs.GetInt("IsFirstRun", 1) == 0)
            {
                // TODO: Uncomment on production to show tutorial
                sceneLoader.GoToMainMenu(true);
            }
            else
            {
                PlayerPrefs.SetInt("IsFirstRun", 0);
            }
        }
    }
}
