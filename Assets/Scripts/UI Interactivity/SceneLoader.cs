using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public Animator transition;
    private float transitionTime = 0.5f;

    public void GoBack()
    {
        try
        {
            int lastSceneIndex = StaticData.Instance.SceneIndexHistory.Pop();
            StartCoroutine(LoadScene(lastSceneIndex, true));
        }
        catch (InvalidOperationException)
        {
            GoToMainMenu(); // Load Main Menu
        }

    }

    public void GoToMainMenu(bool loadImmediately = false)
    {
        StaticData.Instance.reachedMainMenuScene = true;

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
        StartCoroutine(LoadScene("SPGame"));
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
    public void GoToCredits()
    {
        StartCoroutine(LoadScene("Credits"));
    }

    IEnumerator LoadScene(string sceneName, bool isGoingBack = false)
    {
        SoundManager.Instance.PlayClickedSFX();
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        if (!isGoingBack && StaticData.Instance != null)
            StaticData.Instance
                .SceneIndexHistory
                .Push(SceneManager.GetActiveScene().buildIndex);

        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    IEnumerator LoadScene(int buildIndex, bool isGoingBack = false)
    {
        SoundManager.Instance.PlayClickedSFX();

        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        if (!isGoingBack && StaticData.Instance != null)
            StaticData.Instance
                .SceneIndexHistory
                .Push(SceneManager.GetActiveScene().buildIndex);

        SceneManager.LoadScene(buildIndex, LoadSceneMode.Single);
    }

    public void ResetProfile()
    {
        Debug.Log("DEBUG_COMPLETE_RESET");
        SaveLoadSystem.ResetProfileData(true);
        Application.Quit();
    }

    public void QuitApp()
    {
        Application.Quit();
    }

    public void GoToInteractiveTutorial()
    {
        StaticData.Instance.SetGameMode(GameMode.TUTORIAL);
        StartCoroutine(LoadScene("SPGame"));
    }

    public void GoToTutorial()
    {
        StartCoroutine(LoadScene("Tutorial"));
    }

    public void GoToMPTutorial()
    {
        StartCoroutine(LoadScene("MPTutorial"));
    }

    public void GoToDevelopers()
    {
        StartCoroutine(LoadScene("Developers"));
    }
}
