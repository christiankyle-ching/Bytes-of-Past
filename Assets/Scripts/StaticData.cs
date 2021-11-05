using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameMode
{
    SP = 0,
    MP = 1,
    PRE_TEST = 3,
    POST_TEST = 4,
    TUTORIAL = 5,
}

public enum HistoryTopic
{
    COMPUTER = 0,
    NETWORKING = 1,
    SOFTWARE = 2
}

public enum GameDifficulty
{
    EASY = 0,
    MEDIUM = 1,
    HARD = 2
}

public class StaticData : MonoBehaviour
{
    private static StaticData _instance;
    public static StaticData Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            LoadProfileData();
            Input.multiTouchEnabled = false; // Disable multi-touch
            _instance = this;
        }
    }

    // These variables are exposed to all Scenes to pass data between them by DontDestroyOnLoad
    public GameMode SelectedGameMode;
    public HistoryTopic SelectedTopic;
    public GameDifficulty SelectedDifficulty;
    public bool IsPostAssessment;

    public Stack<int> SceneIndexHistory = new Stack<int>();

    public ProfileStatisticsData profileStatisticsData;
    public bool showTutorial = false;

    public void LoadProfileData()
    {
        if (PlayerPrefs.GetInt("GameHasData", 0) == 0)
        {
            // If this is first run, then reset old data, then load a new one
            SaveLoadSystem.ResetProfileData();
            PlayerPrefs.SetInt("GameHasData", 1);
        }
        else
        {
            profileStatisticsData = SaveLoadSystem.LoadProfileStatisticsData();
        }
    }

    public void SetTopic(HistoryTopic topic)
    {
        SelectedTopic = topic;
        Debug.Log("Changed Topic: " + TopicUtils.GetName(SelectedTopic));
    }

    public void SetDifficulty(GameDifficulty diff)
    {
        SelectedDifficulty = diff;
        Debug.Log("Changed Difficulty: " + diff);
    }

    public void SetGameMode(GameMode gm)
    {
        SelectedGameMode = gm;
        Debug.Log("Changed GameMode: " + gm);
    }
}
