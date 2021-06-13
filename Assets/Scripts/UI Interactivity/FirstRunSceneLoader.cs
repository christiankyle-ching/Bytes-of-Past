﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstRunSceneLoader : MonoBehaviour
{
    public SceneLoader sceneLoader;

    void Awake()
    {
        if (PlayerPrefs.GetInt("IsFirstRun", 1) == 0)
        {
            sceneLoader.GoToMainMenu(true);
        }
        else
        {
            PlayerPrefs.SetInt("IsFirstRun", 0);
        }
    }
}