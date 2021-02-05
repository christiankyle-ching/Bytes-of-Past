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

    // UI Child Elements
    private TextMeshProUGUI title;
    private TextMeshProUGUI description;
    private Image image;
    
    void Awake()
    {
        this.cards = new Stack<CardData>();

        this.gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SinglePlayerGameController>();

        this.title = transform.Find("Title").GetComponent<TextMeshProUGUI>();
        this.description = transform.Find("Description").GetComponent<TextMeshProUGUI>();
        this.image = transform.Find("Image").GetComponent<Image>();

        LoadCards();
        SetTopCardPreview();
    }


    void SetTopCardPreview()
    {
        try
        {
            CardData topCard = cards.Peek();

            Debug.Log(topCard);

            this.title.text = topCard.Title;
            this.description.text = topCard.Description;
            this.image.sprite = topCard.Artwork;
        } catch (InvalidOperationException)
        {
            // TODO: if no cards in deck, make invisible
        }
        
    }

    void LoadCards()
    {
        string[] cardAssets = AssetDatabase.FindAssets("t:CardData");
        foreach (string guid in cardAssets)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            CardData loadedCard = (CardData) AssetDatabase.LoadAssetAtPath(assetPath, typeof(CardData));

            cards.Push(loadedCard);
        }
    }

    void ShuffleCards()
    {
        throw new System.NotImplementedException();
    }

    public void GiveCard(Transform playerDropZone, int count) 
    {
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
        } catch (Exception ex)
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
