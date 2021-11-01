using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstRunSceneLoader : MonoBehaviour
{
    public SceneLoader sceneLoader;

    void Start()
    {
        Debug.Log("FirstRun: " + StaticData.Instance.showTutorial);

        if (!StaticData.Instance.showTutorial)
        {
            Debug.Log("Not from Main Menu");

            if (PlayerPrefs.GetInt("GameHasRun", 0) == 1)
            {
                sceneLoader.GoToMainMenu(true);
            }
            else
            {
                PlayerPrefs.SetInt("GameHasRun", 1);
            }
        }
    }
}
