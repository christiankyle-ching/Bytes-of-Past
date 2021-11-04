﻿using TMPro;
using UnityEngine;

public class SinglePlayerMenuManager : MonoBehaviour
{
    public SceneLoader sceneLoader;
    public EndGameAchievementChecker achievementSystem;

    StaticData staticData;

    // END GAME
    public GameObject endGameMenu;

    public GameObject

            winOrLossText,
            accuracyText;

    public GameObject[] topicTexts;

    public GameObject[] difficultyTexts;

    // PAUSE GAME
    public GameObject pauseGameMenu;

    void Awake()
    {
        staticData = StaticData.Instance;

        SetTopicTexts($"Topic: {TopicUtils.GetName(staticData.SelectedTopic)}");
        SetDifficultyTexts($"{DifficultyUtils.GetName(staticData.SelectedDifficulty)} Difficulty");
    }

    public void ResumeGame()
    {
        SoundManager.Instance.PlayClickedSFX();
        pauseGameMenu.SetActive(false);
    }

    public void PauseGame()
    {
        SoundManager.Instance.PlayClickedSFX();
        pauseGameMenu.SetActive(true);
    }

    public void EndGame(bool isGameWon, PlayerStats playerStats)
    {
        Debug.Log(playerStats.AccuracyText);
        Debug.Log(playerStats.Accuracy);

        // Set Texts
        winOrLossText.GetComponent<TextMeshProUGUI>().text =
            isGameWon ? "You Won!" : "You Lost";

        accuracyText.GetComponent<TextMeshProUGUI>().text =
            playerStats.AccuracyText;

        endGameMenu.SetActive(true);

        // Save new accuracy
        staticData
            .profileStatisticsData
            .UpdateSPGameAccuracy(
                staticData.SelectedTopic,
                staticData.SelectedDifficulty,
                playerStats.Accuracy,
                isGameWon);

        // Set the topic as already played
        PlayerPrefs
            .SetInt(TopicUtils.GetPrefKey_IsPlayed(staticData.SelectedTopic),
            1);

        // Check Achievements
        GameData gameData = new GameData(
            staticData.SelectedGameMode,
            staticData.SelectedDifficulty,
            isGameWon,
            playerStats.Accuracy,
            playerStats.remainingLife,
            playerStats.initialLife);

        achievementSystem.ViewAcquiredAchievements(gameData);
    }

    public void SetTopicTexts(string text)
    {
        foreach (GameObject obj in topicTexts)
        {
            obj.GetComponent<TextMeshProUGUI>().text = text;
        }
    }

    public void SetDifficultyTexts(string text)
    {
        foreach (GameObject obj in difficultyTexts)
        {
            obj.GetComponent<TextMeshProUGUI>().text = text;
        }
    }
}
