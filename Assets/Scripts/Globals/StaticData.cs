using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

public enum StudentSection
{
    Generosity, Faith, Endurance, Diligence, Courage
}

public class StaticData : MonoBehaviour
{
    private static StaticData _instance;
    public static StaticData Instance { get { return _instance; } }

    public static Regex nameValidator =
        new Regex(@"\b\s*(?<lname>[a-zA-Z]+)\s*,\s*(?<fname>[a-zA-Z]+)\b");

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Start()
    {
        Invoke(nameof(InitGame), 1f); // Call this at start instead of Awake to finish ResourceParser
    }

    void InitGame()
    {
        Input.multiTouchEnabled = false;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        LoadProfileData();
    }

    // These variables are exposed to all Scenes to pass data between them by DontDestroyOnLoad
    public GameMode SelectedGameMode;
    public HistoryTopic SelectedTopic;
    public GameDifficulty SelectedDifficulty;
    public bool IsPostAssessment;
    public bool checkedUpdates = false;

    public Stack<int> SceneIndexHistory = new Stack<int>();

    public ProfileStatisticsData profileStatisticsData;

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
    static int minNameLength = 4;

    public static bool IsPlayerNameValid(string strName)
    {
        bool isLongEnough = strName.Length >= minNameLength;
        bool isRightFormat = nameValidator.IsMatch(strName);

        return isLongEnough && isRightFormat;
    }

    public string GetDBPlayerName()
    {
        return PlayerPrefs.GetString(PREFKEY_PROFILENAME, "");
    }


    public string GetPlayerFirstName()
    {
        return StudentName_ExtractFirstName(GetDBPlayerName());
    }

    public string GetPlayerLastName()
    {
        return StudentName_ExtractLastName(GetDBPlayerName());
    }

    public string GetPlayerShortName()
    {
        string fName = GetPlayerFirstName();
        string lName = GetPlayerLastName();

        Debug.Log("FIRSTNAME: " + fName);

        return $"{fName.Substring(0, 1)}. {lName}";
    }

    public string GetPlayerFullName()
    {
        string fName = GetPlayerFirstName();
        string lName = GetPlayerLastName();

        return $"{fName} {lName}";
    }

    public string GetPlayerInversedFullName()
    {
        string fName = GetPlayerFirstName();
        string lName = GetPlayerLastName();

        return $"{lName}, {fName}";
    }

    public string FormatPlayerName(string strName)
    {
        string fName = StudentName_ExtractFirstName(strName);
        string lName = StudentName_ExtractLastName(strName);

        return $"{lName}, {fName}";
    }

    public void SetPlayerName(string strName)
    {
        string playerName = FormatPlayerName(strName.Trim());

        Debug.Log("CHANGE_NAME: " + playerName);

        PlayerPrefs.SetString(PREFKEY_PROFILENAME, playerName);
    }

    public string StudentName_ExtractFirstName(string strName)
    {
        try
        {
            return nameValidator.Match(strName).Groups["fname"].Value;
        }
        catch (Exception ex)
        {
            Debug.Log("ERROR: Extracting First Name - " + ex);
            return "";
        }
    }

    public string StudentName_ExtractLastName(string strName)
    {
        try
        {
            return nameValidator.Match(strName).Groups["lname"].Value;
        }
        catch (Exception ex)
        {
            Debug.Log("ERROR: Extracting Last Name - " + ex);
            return "";
        }
    }

    // ------------------------------ SECTION ------------------------------


    public int GetPlayerSection()
    {
        return PlayerPrefs.GetInt(PREFKEY_PROFILESECTION, 0);
    }

    public void SetPlayerSection(StudentSection s)
    {
        PlayerPrefs.SetInt(PREFKEY_PROFILESECTION, (int)s);
    }

    public string GetPlayerSectionString()
    {
        return Enum.GetName(typeof(StudentSection), GetPlayerSection());
    }

    public List<string> GetPlayerSections()
    {
        return Enum.GetNames(typeof(StudentSection)).ToList();
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
