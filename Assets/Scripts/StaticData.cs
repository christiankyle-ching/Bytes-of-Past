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

[System.Serializable]
public class ProfileStatisticsData
{
    /*
    With size of [2,3,3]
    2 or GAMEMODE (Single Player & Multiplayer)
    3 for each of the TOPIC
    3 for each DIFFICULTY
    */
    public float[,,] gameAccuracy = new float[2, 3, 3];

    /*
    With size of [2,3,3,2]
    2 or GAMEMODE (Single Player & Multiplayer)
    3 for each of the TOPIC
    3 for each DIFFICULTY
    2 for Win/Loss counts
    */
    public int[,,,] gameWinLossCount = new int[2, 3, 3, 2];

    /*
    With size of [2,3]
    2 or GAMEMODE (Pre & Post Assessment)
    3 for each of the TOPIC
    */
    public int[,] assessmentScores = new int[2, 3];

    public void UpdateGameAccuracy(
        GAMEMODE gameMode,
        TOPIC topic,
        DIFFICULTY difficulty,
        float newAccuracy,
        bool gameWon
    )
    {
        int gameModeIndex = gameMode == GAMEMODE.SinglePlayer ? 0 : 1;
        int topicIndex = (int) topic;
        int difficultyIndex = (int) difficulty;

        /* 
        Update Accuracy in Array with their average
        unless it is 0, which means it's their first game,
        and the accuracy should be set without averaging
         */
        float existingAccuracy =
            gameAccuracy[gameModeIndex, topicIndex, difficultyIndex];
        gameAccuracy[gameModeIndex, topicIndex, difficultyIndex] =
            (existingAccuracy == 0f)
                ? newAccuracy
                : ((newAccuracy + existingAccuracy) / 2);

        /* 
        Update Win/Loss count
         */
        int currentCount =
            gameWinLossCount[gameModeIndex,
            topicIndex,
            difficultyIndex,
            (gameWon ? 0 : 1)];

        gameWinLossCount[gameModeIndex,
        topicIndex,
        difficultyIndex,
        (gameWon ? 0 : 1)] = currentCount++;
    }

    public void UpdateAssessmentScore(GAMEMODE gameMode, TOPIC topic, int score)
    {
        int preOrPostIndex = gameMode == GAMEMODE.PreAssessment ? 0 : 1;
        int topicIndex = (int) topic;

        // Update Score in Array
        assessmentScores[preOrPostIndex, topicIndex] = score;
    }
}

public class StaticData : MonoBehaviour
{
    public GameObject sceneLoader;

    // These variables are exposed to all Scenes to pass data between them by DontDestroyOnLoad
    public GAMEMODE SelectedGameMode;

    public TOPIC SelectedTopic;

    public DIFFICULTY SelectedDifficulty;

    public bool IsPostAssessment;

    public Stack<int> SceneIndexHistory = new Stack<int>();

    public ProfileStatisticsData
        profileStatisticsData = new ProfileStatisticsData();

    void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("Static Data").Length > 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);

            // Load necessary data
            LoadProfileData();
        }
    }

    void LoadProfileData()
    {
        profileStatisticsData = SaveLoadSystem.LoadProfileStatisticsData();
    }
}
