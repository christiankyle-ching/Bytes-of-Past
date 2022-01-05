using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopicSelect : MonoBehaviour
{
    StaticData staticData;

    public GameObject
            sceneLoader,
            btnComputer,
            btnNetworking,
            btnSoftware;

    bool postHasLocked = false;
    bool preHasUntaken = false;

    public GameObject preAssessmentTooltip;
    public GameObject postAssessmentTooltip;

    void Awake()
    {
        staticData = StaticData.Instance;

        btnComputer
            .GetComponent<Button>()
            .onClick
            .AddListener(() => OnTopicSelect(HistoryTopic.COMPUTER));

        btnNetworking
            .GetComponent<Button>()
            .onClick
            .AddListener(() => OnTopicSelect(HistoryTopic.NETWORKING));

        btnSoftware
            .GetComponent<Button>()
            .onClick
            .AddListener(() => OnTopicSelect(HistoryTopic.SOFTWARE));

        SetTopicDisabled(btnComputer.GetComponent<Button>(), HistoryTopic.COMPUTER);
        SetTopicDisabled(btnNetworking.GetComponent<Button>(),
        HistoryTopic.NETWORKING);
        SetTopicDisabled(btnSoftware.GetComponent<Button>(), HistoryTopic.SOFTWARE);

        ShowTooltips();
    }

    void SetTopicDisabled(Button button, HistoryTopic topic)
    {
        bool isPreAssessmentDone = PrefsConverter.IntToBoolean(PlayerPrefs
                .GetInt(TopicUtils.GetPrefKey_IsPreAssessmentDone(topic), 0));

        bool isPlayed =
            PrefsConverter.IntToBoolean(PlayerPrefs.GetInt(TopicUtils.GetPrefKey_IsPlayed(topic), 0));

        bool isPostAssessmentDone =
            PrefsConverter.IntToBoolean(PlayerPrefs
                .GetInt(TopicUtils.GetPrefKey_IsPostAssessmentDone(topic), 0));

        TextMeshProUGUI buttonText =
            button
                .gameObject
                .transform
                .Find("Button")
                .GetComponentInChildren<TextMeshProUGUI>();

        if (staticData.SelectedGameMode == GameMode.POST_TEST)
        {
            bool postAssessmentAllowed =
                isPreAssessmentDone && !isPostAssessmentDone && isPlayed;

            button.interactable = postAssessmentAllowed;

            if (!postAssessmentAllowed)
                buttonText.text =
                    isPostAssessmentDone ? "Already Taken" : "Locked";

            if (!postAssessmentAllowed) postHasLocked = true;
        }
        else if (staticData.SelectedGameMode == GameMode.SP)
        {
            if (!isPreAssessmentDone)
            {
                buttonText.text = "Take Pre-Test";

                preHasUntaken = true;
            }
        }

        Debug
            .Log(topic +
            $"\nPRE-Assessment Done? {isPreAssessmentDone}" +
            $"\nPLAYED? {isPlayed}" +
            $"\nPOST-Assessment Done? {isPostAssessmentDone}");
    }

    void OnTopicSelect(HistoryTopic topic)
    {
        staticData.SetTopic(topic);

        GameMode gameMode = staticData.SelectedGameMode;

        bool isPreAssessmentDone =
            PlayerPrefs
                .GetInt(TopicUtils.GetPrefKey_IsPreAssessmentDone(topic), 0) ==
            1;

        switch (gameMode)
        {
            case GameMode.SP:
                if (isPreAssessmentDone)
                {
                    sceneLoader
                        .GetComponent<SceneLoader>()
                        .GoToDifficultySelect();
                }
                else
                {
                    // if the player hasn't played the topic yet, do a pre-assessment first
                    LoadPreAssessment();
                }
                break;
            case GameMode.POST_TEST:
                staticData.IsPostAssessment = true;
                sceneLoader.GetComponent<SceneLoader>().GoToAssessmentTest();
                break;
            default:
                Debug.LogError("No Game Mode Selected");
                sceneLoader.GetComponent<SceneLoader>().GoToDifficultySelect();
                break;
        }
    }

    void LoadPreAssessment()
    {
        Debug.Log("LoadPreAssessment");

        staticData.SetGameMode(GameMode.PRE_TEST);
        staticData.IsPostAssessment = false;
        sceneLoader.GetComponent<SceneLoader>().GoToAssessmentTest();
    }

    void ShowTooltips()
    {
        postAssessmentTooltip.SetActive(postHasLocked);
        preAssessmentTooltip.SetActive(preHasUntaken);
    }
}
