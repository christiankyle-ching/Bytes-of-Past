using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameLoader : MonoBehaviour
{
    [Header("UI References")]
    public Slider progressBar;
    public TextMeshProUGUI progressText;
    public SceneLoader sceneLoader;

    private void Start()
    {
        StartCoroutine(LoadCardsFromResource());
    }

    IEnumerator LoadCardsFromResource()
    {
        ResourceRequest req0 = Resources.LoadAsync<TextAsset>("Cards/Cards - Computer");
        ResourceRequest req1 = Resources.LoadAsync<TextAsset>("Cards/Cards - Networking");
        ResourceRequest req2 = Resources.LoadAsync<TextAsset>("Cards/Cards - Software");

        while (!req0.isDone || !req1.isDone || !req2.isDone)
        {
            yield return null;
        }

        ResourceParser.Instance.SetCards(req0.asset as TextAsset, HistoryTopic.COMPUTER);
        ResourceParser.Instance.SetCards(req1.asset as TextAsset, HistoryTopic.NETWORKING);
        ResourceParser.Instance.SetCards(req2.asset as TextAsset, HistoryTopic.SOFTWARE);

#if !UNITY_EDITOR
        // Artificial Loading for Release
        float j = 0;
        while (j < 1f)
        {
            j += 0.01f;
            UpdateProgress(Mathf.Clamp01(j));
            yield return new WaitForSecondsRealtime(.01f);
        }
#endif

        //UpdateProgress(req0.progress, req1.progress, req2.progress);
        //Debug.Log($"Finished Loading Resources... {req0.isDone}, {req1.isDone}, {req2.isDone}");
        //Debug.Log($"Finished Loading Resources... {req0.progress}, {req1.progress}, {req2.progress}");

        // If GameHasRun, go to main menu, else, show tutorial first
        if (PlayerPrefs.GetInt("GameHasRun", 0) == 1)
        {
            sceneLoader.GoToMainMenu();
        }
        else
        {
            sceneLoader.GoToTutorial();
            PlayerPrefs.SetInt("GameHasRun", 1);
        }

    }

    // Method can still be used when we want to use real progress
    void UpdateProgress(params float[] progresses)
    {
        float overallProgress = progresses.Sum() / progresses.Length;

        progressBar.value = overallProgress;
        progressText.text = $"{Math.Round(overallProgress * 100)} %";
    }

    void UpdateProgress(float progress)
    {
        progressBar.value = progress;
        progressText.text = $"{Math.Round(progress * 100)} %";
    }

}
