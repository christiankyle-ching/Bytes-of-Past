using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Animator transition;

    private float transitionTime = 0.5f;

    public void GoToMainMenu()
    {
        StartCoroutine(LoadScene("Main Menu"));
    }

    public void GoToSinglePlayerLobby()
    {
        StartCoroutine(LoadScene("Single Player Lobby"));
    }

    public void GoToSinglePlayerGame()
    {
        StartCoroutine(LoadScene("Single Player Game"));
    }

    public void GoToMultiplayerLobby()
    {
        StartCoroutine(LoadScene("Multiplayer Lobby"));
        throw new NotImplementedException();
    }

    public void GoToMultiplayerGame()
    {
        // StartCoroutine(LoadScene("Multiplayer Game"));
        throw new NotImplementedException();
    }

    public void GoToAssessmentLobby()
    {
        StartCoroutine(LoadScene("Assessment Lobby"));
    }

    public void GoToAssessmentTest()
    {
        StartCoroutine(LoadScene("Assessment Test"));
    }

    public void GoToProfile()
    {
        StartCoroutine(LoadScene("Profile"));
    }

    IEnumerator LoadScene(string sceneName)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
