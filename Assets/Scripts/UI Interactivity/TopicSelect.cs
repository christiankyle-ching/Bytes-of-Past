using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopicSelect : MonoBehaviour
{
    public GameObject

            sceneLoader,
            btnComputer,
            btnNetworking,
            btnSoftware;

    void Awake()
    {
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
    }

    void OnTopicSelect(TOPIC topic)
    {
        StaticData staticData =
            GameObject.FindWithTag("UI Manager").GetComponent<StaticData>();

        staticData.SelectedTopic = topic;

        GAMEMODE gameMode = staticData.SelectedGameMode;

        switch (gameMode)
        {
            case GAMEMODE.SinglePlayer:
                sceneLoader.GetComponent<SceneLoader>().GoToDifficultySelect();
                break;
            case GAMEMODE.Multiplayer:
                sceneLoader.GetComponent<SceneLoader>().GoToDifficultySelect();
                break;
            case GAMEMODE.PreAssessment:
                staticData.IsPostAssessment = false;
                sceneLoader.GetComponent<SceneLoader>().GoToAssessmentTest();
                break;
            case GAMEMODE.PostAssessment:
                staticData.IsPostAssessment = true;
                sceneLoader.GetComponent<SceneLoader>().GoToAssessmentTest();
                break;
        }
    }
}
