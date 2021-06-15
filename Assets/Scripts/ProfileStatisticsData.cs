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
        unless it is 0, which MIGHT mean it's their first game,
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
        gameWinLossCount[gameModeIndex,
        topicIndex,
        difficultyIndex,
        (gameWon ? 0 : 1)] += 1;

        SaveLoadSystem.SaveProfileStatisticsData(this);
    }

    public void UpdateAssessmentScore(GAMEMODE gameMode, TOPIC topic, int score)
    {
        int preOrPostIndex = gameMode == GAMEMODE.PreAssessment ? 0 : 1;
        int topicIndex = (int) topic;

        // Update Score in Array
        assessmentScores[preOrPostIndex, topicIndex] = score;

        SaveLoadSystem.SaveProfileStatisticsData(this);
    }
}
