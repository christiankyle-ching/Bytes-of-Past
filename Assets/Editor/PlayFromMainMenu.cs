using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class PlayFromMainMenu
{
    /* 
    Auto-play on Main Menu if Play Button was pressed on EDITOR ONLY
    */
    static PlayFromMainMenu()
    {
        // Comment out to debug a specific scene easily
        //string path = "Assets/Scenes/Main Menu.unity";
        // EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
    }
}
