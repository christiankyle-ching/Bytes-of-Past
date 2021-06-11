using System;
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

    // UI Child Elements
    private TextMeshProUGUI title;
    private TextMeshProUGUI description;
    private TextMeshProUGUI count;
    private Image image;

    void Awake()
    {
        this.cards = new Stack<CardData>();

        this.gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SinglePlayerGameController>();

        this.title = transform.Find("Title").GetComponent<TextMeshProUGUI>();
        this.description = transform.Find("Description").GetComponent<TextMeshProUGUI>();
        this.count = transform.Find("Count").GetComponent<TextMeshProUGUI>();
        this.image = transform.Find("Image").GetComponent<Image>();

        LoadCards();
        SetTopCardPreview();
    }


    void SetTopCardPreview()
    {
        try
        {
            CardData topCard = cards.Peek();

            this.title.text = topCard.Title;
            this.description.text = topCard.Description;
            this.image.sprite = topCard.Artwork;
            this.count.text = cards.Count.ToString();

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

    void LoadCards()
    {
        CardData[] cardAssets = Resources.LoadAll<CardData>("Cards/Data");

        foreach(CardData cardData in cardAssets)
        {
            cards.Push(cardData);
        }

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

        SetTopCardPreview();
    }

    public void AddCard(CardData cardData)
    {
        cards.Push(cardData);
    }
}
