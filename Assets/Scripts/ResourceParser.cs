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

    public CardData[] ParseCSVToCards(TOPIC topic)
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
            case TOPIC.Computer:
                rawData =
                    Resources
                        .Load
                        <TextAsset
                        >("Cards/Cards - Computer");
                break;
            case TOPIC.Networking:
                rawData =
                    Resources
                        .Load
                        <TextAsset
                        >("Cards/Cards - Networking");
                break;
            case TOPIC.Software:
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

}
