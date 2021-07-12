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
        EditorSceneManager.playModeStartScene =
            AssetDatabase
                .LoadAssetAtPath<SceneAsset>("Assets/Scenes/Main Menu.unity");
    }
}
