using System;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    // Game Controller
    private SinglePlayerGameController gameController;

    // Prefabs
    [SerializeField]
    private Card cardPrefab;

    // Cards
    private Stack<CardData> cards;
    public int CardsCount { get => this.cards.Count; }

    // Game
    public StaticData staticData;

    void Awake()
    {
        try
        {
            staticData =
                GameObject
                    .FindWithTag("Static Data")
                    .GetComponent<StaticData>();
        }
        catch (System.NullReferenceException)
        {
            Debug.LogError("Static Data Not Found: Play from the Main Menu");
            staticData = new StaticData();
        }

        this.cards = new Stack<CardData>();

        this.gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SinglePlayerGameController>();

        LoadCards(TOPIC.Computer);
        // SetTopCardPreview();
    }


    void SetTopCardPreview()
    {
        try
        {
            CardData topCard = cards.Peek();
            transform.GetChild(0).GetComponent<Card>().CardData = topCard;
            transform.GetChild(0).GetComponent<Card>().initCardData();
            SetVisible(true);
        }
        catch (InvalidOperationException)
        {
            // TODO: if no cards in deck, make invisible
            SetVisible(false);
        }

    }

    void SetVisible(bool isVisible)
    {
        GetComponent<Image>().color = isVisible ? Color.white : Color.clear;

        foreach (Image img in GetComponentsInChildren<Image>())
        {
            img.color = isVisible ? Color.white : Color.clear;
        }

        foreach (TextMeshProUGUI txt in GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (!isVisible) txt.text = "";
        }
    }

    void LoadCards(TOPIC topic)
    {
        CardData[] cardAssets = ParseCSVToCards(topic);

        foreach(CardData cardData in cardAssets)
        {
            this.cards.Push(cardData);
        }

    }

    CardData[] ParseCSVToCards(TOPIC topic)
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
                        >("Cards/Cards - Networking");
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

    void ShuffleCards()
    {
        throw new System.NotImplementedException();
    }

    public void GiveCard(Transform playerDropZone, int count)
    {
        if (cards.Count <= 0) return;
        
        Transform receivingCardContainer = playerDropZone.GetChild(0);

        try
        {
            for (int i = 0; i < count; i++)
            {
                // Pop first in Stack before anything else, to catch errors immediately
                // Prevents instantiation of blank cards
                CardData cardData = cards.Pop();

                Card card = Instantiate(cardPrefab, receivingCardContainer);
                card.CardData = cardData;
                card.initCardData();

                // if cards is empty after giving a card,
                // only break the loop, deck should still be in game
                if (cards.Count <= 0) break;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        // SetTopCardPreview();
    }

    public void AddCard(CardData cardData)
    {
        cards.Push(cardData);
    }
}
