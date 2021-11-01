[System.Serializable]
public class ProfileStatisticsData
{
    /*
    With size of [3,3]
    3 for each of the TOPIC
    3 for each DIFFICULTY
    */
    public float[,] SPGameAccuracy = new float[3, 3];

    /*
    With size of [3,3,2]
    3 for each of the TOPIC
    3 for each DIFFICULTY
    2 for Win/Loss counts
    */
    public int[,,] SPWinLossCount = new int[3, 3, 2];

    /*
    With size of [2,3]
    2 or GAMEMODE (Pre & Post Assessment)
    3 for each of the TOPIC
    */
    public int[,] assessmentScores = new int[2, 3];

    public void UpdateSPGameAccuracy(
        TOPIC topic,
        DIFFICULTY difficulty,
        float newAccuracy,
        bool gameWon
    )
    {
        int topicIndex = (int)topic;
        int difficultyIndex = (int)difficulty;

        /* 
        Update Accuracy in Array with their average
        unless it is 0, which MIGHT mean it's their first game,
        then the accuracy should be set without averaging
         */
        float existingAccuracy =
            SPGameAccuracy[topicIndex, difficultyIndex];

        SPGameAccuracy[topicIndex, difficultyIndex] =
            (existingAccuracy == 0f)
                ? newAccuracy
                : ((newAccuracy + existingAccuracy) / 2);

        /* 
        Update Win/Loss count
        */
        SPWinLossCount[topicIndex, difficultyIndex, (gameWon ? 0 : 1)] += 1;

        SaveLoadSystem.SaveProfileStatisticsData(this);
    }


    public void UpdateAssessmentScore(GAMEMODE gameMode, TOPIC topic, int score)
    {
        int preOrPostIndex = gameMode == GAMEMODE.PreAssessment ? 0 : 1;
        int topicIndex = (int)topic;

        // Update Score in Array
        assessmentScores[preOrPostIndex, topicIndex] = score;

        SaveLoadSystem.SaveProfileStatisticsData(this);
    }

    // MULTPLAYER

    /*
    With size of [3]
    3 for each of the TOPIC    
    */
    public float[] MPGameAccuracy = new float[3];

    /*
    With size of [3,3,2]
    3 for each of the TOPIC
    2 for Win/Loss counts
    */
    public int[,] MPWinLossCount = new int[3, 2];


    public void UpdateMPGameAccuracy(
        TOPIC topic,
        float newAccuracy,
        bool gameWon
    )
    {
        int topicIndex = (int)topic;

        /* 
        Update Accuracy in Array with their average
        unless it is 0, which MIGHT mean it's their first game,
        then the accuracy should be set without averaging
         */
        float existingAccuracy =
            MPGameAccuracy[topicIndex];

        MPGameAccuracy[topicIndex] =
            (existingAccuracy == 0f)
                ? newAccuracy
                : ((newAccuracy + existingAccuracy) / 2);

        /* 
        Update Win/Loss count
        */
        MPWinLossCount[topicIndex, (gameWon ? 0 : 1)] += 1;

        SaveLoadSystem.SaveProfileStatisticsData(this);
    }

}
