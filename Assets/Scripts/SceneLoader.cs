using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public void onClick()
    {
        LoadMainMenu();
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void LoadSoloPlay()
    {
        SceneManager.LoadScene("Single Player Lobby");
    }

    public void LoadMultiPlay()
    {
        SceneManager.LoadScene("Multiplayer Lobby");
    }

    public void LoadPostAssessment()
    {
        SceneManager.LoadScene("Post Assessment Test");
    }

    public void ExitApp()
    {
        Application.Quit();
    }
}
