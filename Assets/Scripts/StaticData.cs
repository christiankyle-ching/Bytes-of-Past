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

    public void Start()
    {
        // Prevents screen from sleeping to avoid MP problems
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

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

    #region ------------------------------ Student Profile ------------------------------

    static readonly string PREFKEY_PROFILENAME = "Profile_Name";
    static readonly string PREFKEY_PROFILESECTION = "Profile_Section";
    static readonly string PREFKEY_PLAYERAVATAR = "Profile_Avatar";

    // ------------------------------ STUDENT NAME ------------------------------

    public string GetPlayerName()
    {
        return PlayerPrefs.GetString(PREFKEY_PROFILENAME, "");
    }

    public void SetPlayerName(string playerName)
    {
        PlayerPrefs.SetString(PREFKEY_PROFILENAME, playerName);
    }

    // ------------------------------ SECTION ------------------------------

    public int GetPlayerSection()
    {
        return PlayerPrefs.GetInt(PREFKEY_PROFILESECTION, 0);
    }

    public void SetPlayerSection(int index)
    {
        PlayerPrefs.SetInt(PREFKEY_PROFILESECTION, index);
    }

    public string GetPlayerSectionString()
    {
        int index = GetPlayerSection();

        switch (index)
        {
            case 0:
                return "Grade 9-1";
            case 1:
                return "Grade 9-2";
            case 2:
                return "Grade 9-3";
            default:
                return "";
        }
    }

    // ------------------------------ AVATAR ------------------------------
    public Avatar GetPlayerAvatar()
    {
        return (Avatar)PlayerPrefs.GetInt(PREFKEY_PLAYERAVATAR, (int)Avatar.MALE_0);
    }

    public void SetPlayerAvatar(Avatar _av)
    {
        PlayerPrefs.SetInt(PREFKEY_PLAYERAVATAR, (int)_av);
    }

    #endregion
}
