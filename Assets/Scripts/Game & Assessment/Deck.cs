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

    private AudioSource audioSource;
    public AudioClip singleDrawSFX, multipleDrawSFX;


    [SerializeField]
    private TextMeshProUGUI txtRemainingCount;

    // Prefabs
    [SerializeField]
    private Card cardPrefab;

    // Cards
    private List<CardData> cards;
    public int CardsCount { get => this.cards.Count; }

    // Game
    public StaticData staticData;

    void Awake()
    {
        staticData = StaticData.Instance;

        this.cards = new List<CardData>();
        this.gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SinglePlayerGameController>();
        this.audioSource = GetComponent<AudioSource>();

        LoadCards(staticData.SelectedTopic);
        SetRemainingCards();
    }


    void SetRemainingCards()
    {
        txtRemainingCount.text =
        $@"{CardsCount}
        CARDS
        LEFT";
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
        CardData[] cardAssets = ResourceParser.Instance.ParseCSVToCards(topic);

        foreach (CardData cardData in cardAssets)
        {
            this.cards.Add(cardData);
        }

        ShuffleCards();
    }

    void ShuffleCards()
    {
        List<CardData> shuffledCards = new List<CardData>();

        foreach (CardData cardData in this.cards.OrderBy(x => UnityEngine.Random.Range(0f, 1f)))
        {
            shuffledCards.Add(cardData);
        }

        this.cards = shuffledCards;

        // throw new System.NotImplementedException();
    }

    public CardData[] PopCards(int count)
    {
        List<CardData> _cards = new List<CardData>();
        try
        {
            for (int i = 0; i < count; i++)
            {
                // Pop first in Stack before anything else, to catch errors immediately
                // Prevents instantiation of blank cards
                //CardData cardData = cards.Pop();
                CardData cardData = cards.ElementAt(0);
                _cards.Add(cardData);

                cards.RemoveAt(0);

                // if cards is empty after giving a card,
                // only break the loop, deck should still be in game
                if (cards.Count <= 0) break;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        SetRemainingCards();

        return _cards.OrderBy(card => card.Year).ToArray();
    }

    public void GiveCard(Transform playerDropZone, int count)
    {
        if (cards.Count <= 0) return;

        Transform receivingCardContainer = playerDropZone.GetChild(0);

        try
        {
            CardData[] _cards = PopCards(count);

            foreach (CardData cardData in _cards)
            {
                Card card = Instantiate(cardPrefab, receivingCardContainer);
                card.CardData = cardData;
                card.initCardData();
            }

            //for (int i = 0; i < count; i++)
            //{
            //    // Pop first in Stack before anything else, to catch errors immediately
            //    // Prevents instantiation of blank cards
            //    CardData cardData = cards.Pop();

            //    Card card = Instantiate(cardPrefab, receivingCardContainer);
            //    card.CardData = cardData;
            //    card.initCardData();

            //    // if cards is empty after giving a card,
            //    // only break the loop, deck should still be in game
            //    if (cards.Count <= 0) break;
            //}
        }
        catch (Exception ex)
        {
            throw ex;
        }

        SetRemainingCards();

        if (count > 0)
        {
            if (count == 1)
            {
                PlaySingleDrawSFX();
            }
            else
            {
                PlayMultipleDrawSFX();
            }
        }
    }

    private void PlaySingleDrawSFX()
    {
        audioSource.clip = singleDrawSFX;
        audioSource.Play();
    }

    private void PlayMultipleDrawSFX()
    {
        audioSource.clip = multipleDrawSFX;
        audioSource.Play();
    }

    public void AddCard(CardData cardData)
    {
        cards.Add(cardData);
    }

}
