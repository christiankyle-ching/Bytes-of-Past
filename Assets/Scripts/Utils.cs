public static class TopicUtils
{
    public static string GetName(HistoryTopic topic)
    {
        switch (topic)
        {
            case HistoryTopic.COMPUTER:
                return "Computer";
            case HistoryTopic.NETWORKING:
                return "Networking and Web";
            case HistoryTopic.SOFTWARE:
                return "Software and Languages";
            default:
                return "NO TOPIC";
        }
    }

    public static string GetPrefKey_IsPlayed(HistoryTopic topic)
    {
        return $"TopicPlayed_{(int)topic}";
    }

    public static string GetPrefKey_IsPreAssessmentDone(HistoryTopic topic)
    {
        return $"TopicPreAssessmentDone_{(int)topic}";
    }

    public static string GetPrefKey_IsPostAssessmentDone(HistoryTopic topic)
    {
        return $"TopicPostAssessmentDone_{(int)topic}";
    }
}

public static class DifficultyUtils
{
    public static string GetName(GameDifficulty difficulty)
    {
        switch (difficulty)
        {
            case GameDifficulty.EASY:
                return "Easy";
            case GameDifficulty.MEDIUM:
                return "Medium";
            case GameDifficulty.HARD:
                return "Hard";
            default:
                return "NO DIFFICULTY";
        }
    }
}

public static class PrefsConverter
{
    public static bool IntToBoolean(int n)
    {
        return n == 1;
    }
}
