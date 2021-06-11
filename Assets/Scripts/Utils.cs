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
