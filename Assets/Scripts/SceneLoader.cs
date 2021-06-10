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

    public void backToLoadingScreen()
    {
        SceneManager.LoadScene("Loading Screen");
    }
    public void LoadMultiPlay()
    {
        SceneManager.LoadScene("Multiplayer Lobby");
    }

    public void LoadAssessmentLobby()
    {
        SceneManager.LoadScene("Assessment Lobby");
    }
    public void LoadProfile()
    {
        SceneManager.LoadScene("Profile");
    }

    public void LoadDifficulty()
    {
        SceneManager.LoadScene("Difficulty Scene");
    }
    /* 
    isPostAssessment = 0 is false, 1 is true
    */
    public void LoadAssessment(TOPIC topic, int isPostAssessment = 0)
    {
        // Store the selected topic temporarily for the other scene
        PlayerPrefs.SetInt("Assessment_SelectedTopic", (int) topic);
        PlayerPrefs.SetInt("Assessment_IsPostAssessment", isPostAssessment);

        SceneManager.LoadScene("Assessment Test");
    }

    public void ExitApp()
    {
        Application.Quit();
    }
}
