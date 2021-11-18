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
        Debug.Log("SceneLoader: " + sceneName);

        SoundManager.Instance.PlayClickedSFX();

        if (!isGoingBack && StaticData.Instance != null)
            StaticData.Instance
                .SceneIndexHistory
                .Push(SceneManager.GetActiveScene().buildIndex);

        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            if (op.progress >= 0.9f)
            {
                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    IEnumerator LoadScene(int buildIndex, bool isGoingBack = false)
    {
        SoundManager.Instance.PlayClickedSFX();

        if (!isGoingBack && StaticData.Instance != null)
            StaticData.Instance
                .SceneIndexHistory
                .Push(SceneManager.GetActiveScene().buildIndex);

        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);

        AsyncOperation op = SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Single);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            if (op.progress >= 0.9f)
            {
                op.allowSceneActivation = true;
            }

            yield return null;
        }
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
