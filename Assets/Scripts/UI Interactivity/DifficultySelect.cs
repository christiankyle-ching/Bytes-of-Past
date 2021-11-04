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
            .AddListener(() => OnDifficultySelect(DIFFICULTY.Easy));

        btnMedium
            .GetComponent<Button>()
            .onClick
            .AddListener(() => OnDifficultySelect(DIFFICULTY.Medium));

        btnHard
            .GetComponent<Button>()
            .onClick
            .AddListener(() => OnDifficultySelect(DIFFICULTY.Hard));
    }

    void OnDifficultySelect(DIFFICULTY difficulty)
    {
        SoundManager.Instance.PlayClickedSFX();

        StaticData staticData = StaticData.Instance;

        // Set Difficulty
        staticData.SetDifficulty(difficulty);

        GAMEMODE gameMode = staticData.SelectedGameMode;

        switch (gameMode)
        {
            case GAMEMODE.SinglePlayer:
                sceneLoader.GetComponent<SceneLoader>().GoToSinglePlayerGame();
                break;
        }
    }
}
