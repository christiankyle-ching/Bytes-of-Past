using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceParser : MonoBehaviour
{
    private static ResourceParser _instance;
    public static ResourceParser Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public CardData[] ParseCSVToCards(HistoryTopic topic)
    {
        /* 
        IMPORTANT: Download from Google Sheets in .tsv. Then RENAME format to .csv.
        Then drag it to Unity (to be recognized as a TextAsset with tabs as delimiters).
        */
        List<CardData> cards = new List<CardData>();

        // Parse a CSV file containing the questions and answers
        TextAsset rawData = null;

        switch (topic)
        {
            case HistoryTopic.COMPUTER:
                rawData =
                    Resources
                        .Load
                        <TextAsset
                        >("Cards/Cards - Computer");
                break;
            case HistoryTopic.NETWORKING:
                rawData =
                    Resources
                        .Load
                        <TextAsset
                        >("Cards/Cards - Networking");
                break;
            case HistoryTopic.SOFTWARE:
                rawData =
                    Resources
                        .Load
                        <TextAsset
                        >("Cards/Cards - Software");
                break;
        }

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

        return cards.ToArray();
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

}
