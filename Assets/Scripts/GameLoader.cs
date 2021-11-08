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

        // COMPUTER
        while (!req0.isDone)
        {
            UpdateProgress(req0.progress, req1.progress, req2.progress);
            yield return null;
        }
        ResourceParser.Instance.SetCards(req0.asset as TextAsset, HistoryTopic.COMPUTER);

        // NETWORKING
        while (!req1.isDone)
        {
            UpdateProgress(req0.progress, req1.progress, req2.progress);
            yield return null;
        }
        ResourceParser.Instance.SetCards(req1.asset as TextAsset, HistoryTopic.NETWORKING);

        // TODO: Better artificial loading? Because loading is instant
        yield return new WaitForSecondsRealtime(1f); 

        // SOFTWARE
        while (!req2.isDone)
        {
            UpdateProgress(req0.progress, req1.progress, req2.progress);
            yield return null;
        }
        ResourceParser.Instance.SetCards(req2.asset as TextAsset, HistoryTopic.SOFTWARE);

        UpdateProgress(req0.progress, req1.progress, req2.progress);
        Debug.Log($"Finished Loading Resources... {req0.isDone}, {req1.isDone}, {req2.isDone}");
        Debug.Log($"Finished Loading Resources... {req0.progress}, {req1.progress}, {req2.progress}");

        sceneLoader.GoToTutorial();
    }

    void UpdateProgress(params float[] progresses)
    {
        float overallProgress = progresses.Sum() / progresses.Length;

        progressBar.value = overallProgress;
        progressText.text = $"{Math.Round(overallProgress * 100)} %";
    }

}
