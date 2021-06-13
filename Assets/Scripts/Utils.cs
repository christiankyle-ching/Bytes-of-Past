public static class TopicUtils
{
    public static string GetName(TOPIC topic)
    {
        switch (topic)
        {
            case TOPIC.Computer:
                return "Computer";
            case TOPIC.Networking:
                return "Networking and Web";
            case TOPIC.Software:
                return "Software and Languages";
            default:
                return "NO TOPIC";
        }
    }
}

public static class DifficultyUtils
{
    public static string GetName(DIFFICULTY difficulty)
    {
        switch (difficulty)
        {
            case DIFFICULTY.Easy:
                return "Easy";
            case DIFFICULTY.Medium:
                return "Medium";
            case DIFFICULTY.Hard:
                return "Hard";
            default:
                return "NO DIFFICULTY";
        }
    }
}
