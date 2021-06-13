using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Animator transition;

    private float transitionTime = 0.5f;

    private StaticData staticData;

    void Awake()
    {
        try
        {
            staticData =
                GameObject.FindWithTag("Static Data").GetComponent<StaticData>();
        }
        catch (System.Exception)
        {
            Debug.Log("No Static Data Found: 'Main Menu' scene not loaded once");
        }
    }

    public void GoBack()
    {
        StartCoroutine(LoadScene(staticData.SceneIndexHistory.Pop(), true));
    }

    public void GoToMainMenu(bool loadImmediately = false)
    {
        if (loadImmediately)
            SceneManager.LoadScene("Main Menu");
        else
            StartCoroutine(LoadScene("Main Menu"));
    }

    public void GoToTopicSelect()
    {
        StartCoroutine(LoadScene("Topic Select"));
    }

    public void GoToDifficultySelect()
    {
        StartCoroutine(LoadScene("Difficulty Select"));
    }

    public void GoToSinglePlayerGame()
    {
        StartCoroutine(LoadScene("Single Player Game"));
    }

    public void GoToMultiplayerLobby()
    {
        // StartCoroutine(LoadScene("Multiplayer Lobby"));
        throw new NotImplementedException();
    }

    public void GoToMultiplayerGame()
    {
        // StartCoroutine(LoadScene("Multiplayer Game"));
        throw new NotImplementedException();
    }

    public void GoToAssessmentTest()
    {
        StartCoroutine(LoadScene("Assessment Test"));
    }

    public void GoToProfile()
    {
        StartCoroutine(LoadScene("Profile"));
    }

    IEnumerator LoadScene(string sceneName, bool isGoingBack = false)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        if (!isGoingBack && staticData != null)
            staticData
                .SceneIndexHistory
                .Push(SceneManager.GetActiveScene().buildIndex);

        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    IEnumerator LoadScene(int buildIndex, bool isGoingBack = false)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        if (!isGoingBack && staticData != null)
            staticData
                .SceneIndexHistory
                .Push(SceneManager.GetActiveScene().buildIndex);

        SceneManager.LoadScene(buildIndex, LoadSceneMode.Single);
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}
