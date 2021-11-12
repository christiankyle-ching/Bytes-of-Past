using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResourceParser : MonoBehaviour
{
    private static ResourceParser _instance;
    public static ResourceParser Instance { get { return _instance; } }

    public CardData[] _computerCards;
    public CardData[] _networkingCards;
    public CardData[] _softwareCards;

    public bool resourcesLoaded = false;

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
        SceneManager.activeSceneChanged += ChangedActiveScene;
    }

    private void ChangedActiveScene(Scene current, Scene next)
    {
        if (_computerCards == null || _networkingCards == null || _softwareCards == null)
        {
            Debug.Log("CARDS NOT LOADED. PLAY FROM SPLASH SCREEN.");

            TextAsset r0 = Resources.Load<TextAsset>("Cards/Cards - Computer");
            SetCards(r0, HistoryTopic.COMPUTER);

            TextAsset r1 = Resources.Load<TextAsset>("Cards/Cards - Networking");
            SetCards(r1, HistoryTopic.NETWORKING);

            TextAsset r2 = Resources.Load<TextAsset>("Cards/Cards - Software");
            SetCards(r2, HistoryTopic.SOFTWARE);

        }
    }

    public void SetCards(TextAsset rawData, HistoryTopic topic)
    {
        List<CardData> cards = new List<CardData>();

        if (rawData != null)
        {
            List<String> lines = rawData.text.Split('\n').ToList(); // split into lines

            // ignore header
            lines = lines.Skip(3).ToList();

            for (int i = 0; i < lines.Count; i++)
            {
                string[] cells = lines[i].Split('\t');

                /*
                Source: https://docs.google.com/spreadsheets/d/17vhpg6dLhe91SQQIARxOhVp6K-YyyFKwv4303HgV5dk/
                Cell 0 : ID
                Cell 1 : Status
                Cell 2 : Date
                Cell 3 : Name
                Cell 4 : Inventor
                Cell 5 : Description
                Cell 6 : Image Link
                */

                cards
                    .Add(new CardData(cells[0], Int32.Parse(cells[2]), cells[3], cells[4], cells[5]));
            }
        }

        switch (topic)
        {
            case HistoryTopic.COMPUTER:
                _computerCards = cards.ToArray();
                Debug.Log($"SetCards: {topic}[{_computerCards.Length}]");
                break;
            case HistoryTopic.NETWORKING:
                _networkingCards = cards.ToArray();
                Debug.Log($"SetCards: {topic}[{_networkingCards.Length}]");
                break;
            case HistoryTopic.SOFTWARE:
                _softwareCards = cards.ToArray();
                Debug.Log($"SetCards: {topic}[{_softwareCards.Length}]");
                break;
            default:
                break;
        }
    }

    public QuestionData[] ParseCSVToQuestions(HistoryTopic topic)
    {
        /* 
        IMPORTANT: Download from Google Sheets in .tsv. Then RENAME format to .csv.
        Then drag it to Unity (to be recognized as a TextAsset with tabs as delimiters).
        */
        List<QuestionData> questions = new List<QuestionData>();

        // Parse a CSV file containing the questions and answers
        TextAsset rawData = null;

        switch (topic)
        {
            case HistoryTopic.COMPUTER:
                rawData =
                    Resources
                        .Load
                        <TextAsset
                        >("AssessmentTests/Assessment Questions - Computers");
                break;
            case HistoryTopic.NETWORKING:
                rawData =
                    Resources
                        .Load
                        <TextAsset
                        >("AssessmentTests/Assessment Questions - Networking");
                break;
            case HistoryTopic.SOFTWARE:
                rawData =
                    Resources
                        .Load
                        <TextAsset
                        >("AssessmentTests/Assessment Questions - Software");
                break;
        }

        if (rawData != null)
        {
            string[] lines = rawData.text.Split('\n'); // split into lines

            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 0) continue; // ignore header

                string[] cells = lines[i].Split('\t');

                // Cell 0 : ID
                // Cell 1 : Question
                // Cell 2 to 4 : Wrong Answers (3)
                // Cell 5 : Correct Answers
                questions
                    .Add(new QuestionData(cells[1],
                        cells.Skip(2).Take(3).ToArray(),
                        cells[5]));
            }
        }

        return questions.ToArray();
    }

    #region Cards

    public CardData GetCard(int index, HistoryTopic _topic)
    {
        switch (_topic)
        {
            case HistoryTopic.COMPUTER:
                return _computerCards[index];
            case HistoryTopic.NETWORKING:
                return _networkingCards[index];
            case HistoryTopic.SOFTWARE:
                return _softwareCards[index];
        }

        return null;
    }

    public CardData[] GetCards(HistoryTopic _topic)
    {
        switch (_topic)
        {
            case HistoryTopic.COMPUTER:
                return _computerCards;
            case HistoryTopic.NETWORKING:
                return _networkingCards;
            case HistoryTopic.SOFTWARE:
                return _softwareCards;
        }

        return null;
    }

    #endregion
}
