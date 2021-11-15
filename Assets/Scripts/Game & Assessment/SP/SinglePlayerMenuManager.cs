using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SinglePlayerMenuManager : MonoBehaviour
{
    public SceneLoader sceneLoader;
    public EndGameAchievementChecker achievementSystem;

    StaticData staticData;

    [Header("Menus")]
    public GameObject pauseGameMenu;
    public GameObject endGameMenu;

    [Header("Texts")]
    public GameObject
            winOrLossText,
            accuracyText;
    public GameObject[] topicTexts;
    public GameObject[] difficultyTexts;

    [Header("Buttons")]
    public Button btnPause;
    public Button btnResume;
    public Button[] restartButtons;
    public Button[] mainMenuButtons;

    private void Start()
    {
        staticData = StaticData.Instance;

        SetTopicTexts($"Topic: {TopicUtils.GetName(staticData.SelectedTopic)}");
        SetDifficultyTexts($"{DifficultyUtils.GetName(staticData.SelectedDifficulty)} Difficulty");

        pauseGameMenu.SetActive(false);
        endGameMenu.SetActive(false);
        btnPause.gameObject.SetActive(true);

        btnPause.onClick.AddListener(PauseGame);
        btnResume.onClick.AddListener(ResumeGame);
        foreach (Button btn in restartButtons) btn.onClick.AddListener(RestartGame);
        foreach (Button btn in mainMenuButtons) btn.onClick.AddListener(GoToMainMenu);
    }

    public void ResumeGame()
    {
        SoundManager.Instance.PlayClickedSFX();
        pauseGameMenu.SetActive(false);
        btnPause.gameObject.SetActive(true);
    }

    public void PauseGame()
    {
        SoundManager.Instance.PlayClickedSFX();
        pauseGameMenu.SetActive(true);
        btnPause.gameObject.SetActive(false);
    }

    public void RestartGame()
    {
        sceneLoader.GoToSinglePlayerGame();
    }

    public void GoToMainMenu()
    {
        sceneLoader.GoToMainMenu();
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

        // Setup Canvas
        endGameMenu.SetActive(true);
        btnPause.gameObject.SetActive(false);

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
            playerStats.remainingLives,
            playerStats.totalLives);

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
