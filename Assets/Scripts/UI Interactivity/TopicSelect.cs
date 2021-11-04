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

    void Awake()
    {
        staticData = StaticData.Instance;

        btnComputer
            .GetComponent<Button>()
            .onClick
            .AddListener(() => OnTopicSelect(TOPIC.Computer));

        btnNetworking
            .GetComponent<Button>()
            .onClick
            .AddListener(() => OnTopicSelect(TOPIC.Networking));

        btnSoftware
            .GetComponent<Button>()
            .onClick
            .AddListener(() => OnTopicSelect(TOPIC.Software));

        SetTopicDisabled(btnComputer.GetComponent<Button>(), TOPIC.Computer);
        SetTopicDisabled(btnNetworking.GetComponent<Button>(),
        TOPIC.Networking);
        SetTopicDisabled(btnSoftware.GetComponent<Button>(), TOPIC.Software);
    }

    void SetTopicDisabled(Button button, TOPIC topic)
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

        if (staticData.SelectedGameMode == GAMEMODE.PostAssessment)
        {
            bool postAssessmentAllowed =
                isPreAssessmentDone && !isPostAssessmentDone && isPlayed;

            button.interactable = postAssessmentAllowed;

            if (!postAssessmentAllowed)
                buttonText.text =
                    isPostAssessmentDone ? "Already Taken" : "Locked";
        }
        else if (staticData.SelectedGameMode == GAMEMODE.SinglePlayer)
        {
            if (!isPreAssessmentDone)
            {
                buttonText.text = "Take Pre-Test";
            }
        }

        Debug
            .Log(topic +
            $"\nPRE-Assessment Done? {isPreAssessmentDone}" +
            $"\nPLAYED? {isPlayed}" +
            $"\nPOST-Assessment Done? {isPostAssessmentDone}");
    }

    void OnTopicSelect(TOPIC topic)
    {
        SoundManager.Instance.PlayClickedSFX();

        staticData.SetTopic(topic);

        GAMEMODE gameMode = staticData.SelectedGameMode;

        bool isPreAssessmentDone =
            PlayerPrefs
                .GetInt(TopicUtils.GetPrefKey_IsPreAssessmentDone(topic), 0) ==
            1;

        switch (gameMode)
        {
            case GAMEMODE.SinglePlayer:
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
            case GAMEMODE.PostAssessment:
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

        staticData.SetGameMode(GAMEMODE.PreAssessment);
        staticData.IsPostAssessment = false;
        sceneLoader.GetComponent<SceneLoader>().GoToAssessmentTest();
    }
}
