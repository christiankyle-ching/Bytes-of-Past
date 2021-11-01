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
        staticData = StaticData.Instance;
    }

    public void GoBack()
    {
        try
        {
            StartCoroutine(LoadScene(staticData.SceneIndexHistory.Pop(), true));
        }
        catch (InvalidOperationException)
        {
            StartCoroutine(LoadScene(1, true)); // Load Main Menu
        }

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
        StartCoroutine(LoadScene("MPOffline"));
    }

    public void GoToAssessmentTest()
    {
        StartCoroutine(LoadScene("Assessment Test"));
    }

    public void GoToProfile()
    {
        StartCoroutine(LoadScene("Profile"));
    }

    public void GoToAchievements()
    {
        StartCoroutine(LoadScene("Achievements"));
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

    public void ResetProfile()
    {
        Debug.Log("DEBUG_RESET");
        SaveLoadSystem.ResetProfileData();
        Application.Quit();
    }

    public void QuitApp()
    {
        Application.Quit();
    }

    public void GoToTutorial()
    {
        staticData.showTutorial = true;
        SceneManager.LoadScene("Tutorial");
        //staticData.SceneIndexHistory.Clear(); TODO: Do i need this?
    }

    public void GoToMPTutorial()
    {
        StartCoroutine(LoadScene("MPTutorial"));
    }

    public void GoToCredits()
    {
        StartCoroutine(LoadScene("Credits"));
    }

}
