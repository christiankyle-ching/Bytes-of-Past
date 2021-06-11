using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultySelect : MonoBehaviour
{
    public GameObject

            sceneLoader,
            btnEasy,
            btnMedium,
            btnHard;

    void Awake()
    {
        btnEasy
            .GetComponent<Button>()
            .onClick
            .AddListener(() => OnTopicSelect(DIFFICULTY.Easy));

        btnMedium
            .GetComponent<Button>()
            .onClick
            .AddListener(() => OnTopicSelect(DIFFICULTY.Medium));

        btnHard
            .GetComponent<Button>()
            .onClick
            .AddListener(() => OnTopicSelect(DIFFICULTY.Hard));
    }

    void OnTopicSelect(DIFFICULTY difficulty)
    {
        StaticData staticData =
            GameObject.FindWithTag("UI Manager").GetComponent<StaticData>();

        staticData.SelectedDifficulty = difficulty;

        GAMEMODE gameMode = staticData.SelectedGameMode;

        switch (gameMode)
        {
            case GAMEMODE.SinglePlayer:
                sceneLoader.GetComponent<SceneLoader>().GoToSinglePlayerGame();
                break;
            case GAMEMODE.Multiplayer:
                sceneLoader.GetComponent<SceneLoader>().GoToMultiplayerLobby();
                break;
        }
    }
}
