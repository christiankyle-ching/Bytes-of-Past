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
            .AddListener(() => OnDifficultySelect(GameDifficulty.EASY));

        btnMedium
            .GetComponent<Button>()
            .onClick
            .AddListener(() => OnDifficultySelect(GameDifficulty.MEDIUM));

        btnHard
            .GetComponent<Button>()
            .onClick
            .AddListener(() => OnDifficultySelect(GameDifficulty.HARD));
    }

    void OnDifficultySelect(GameDifficulty difficulty)
    {
        SoundManager.Instance.PlayClickedSFX();

        StaticData staticData = StaticData.Instance;

        // Set Difficulty
        staticData.SetDifficulty(difficulty);

        GameMode gameMode = staticData.SelectedGameMode;

        switch (gameMode)
        {
            case GameMode.SP:
                sceneLoader.GetComponent<SceneLoader>().GoToSinglePlayerGame();
                break;
        }
    }
}
