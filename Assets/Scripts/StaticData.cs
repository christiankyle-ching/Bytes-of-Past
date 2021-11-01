using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GAMEMODE
{
    SinglePlayer = 0,
    Multiplayer = 1,
    PreAssessment = 3,
    PostAssessment = 4
}

public enum TOPIC
{
    Computer = 0,
    Networking = 1,
    Software = 2
}

public enum DIFFICULTY
{
    Easy = 0,
    Medium = 1,
    Hard = 2
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
            _instance = this;
        }
    }

    public GameObject sceneLoader;

    // These variables are exposed to all Scenes to pass data between them by DontDestroyOnLoad
    public GAMEMODE SelectedGameMode;

    public TOPIC SelectedTopic;

    public DIFFICULTY SelectedDifficulty;

    public bool IsPostAssessment;

    public Stack<int> SceneIndexHistory = new Stack<int>();

    public ProfileStatisticsData profileStatisticsData;

    public bool showTutorial = false;

    public void LoadProfileData()
    {
        //SaveLoadSystem.ResetProfileData(); Use this when error occurs due to old version of stats
        profileStatisticsData = SaveLoadSystem.LoadProfileStatisticsData();
    }

    public void SetTopic(TOPIC topic)
    {
        SelectedTopic = topic;
        Debug.Log("Changed Topic: " + TopicUtils.GetName(SelectedTopic));
    }

    public void SetDifficulty(DIFFICULTY diff)
    {
        SelectedDifficulty = diff;
        Debug.Log("Changed Difficulty: " + diff);
    }

    public void SetGameMode(GAMEMODE gm)
    {
        SelectedGameMode = gm;
        Debug.Log("Changed GameMode: " + gm);
    }
}
