using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerGameController : MonoBehaviour
{
    int playerLives = 0;
    int startingCardsCount = 8;
    public int turnSeconds = 30;

    // Scene References
    private Transform player;
    private PlayerStats playerStats;

    private DropZone timeline;
    private Transform timelineCardContainer;

    private Deck deck;
    private Graveyard graveyard;

    private Transform livesContainer;
    [SerializeField]
    private GameObject lifePrefab;
    [SerializeField]
    private GameObject cardPrefab;

    public SPTimer timer;

    private SinglePlayerMenuManager menuManager;
    StaticData staticData;

    DIFFICULTY _difficulty = DIFFICULTY.Easy;
    TOPIC _topic = TOPIC.Computer;

    // GAME: Start
    private void Awake()
    {
        staticData = StaticData.Instance;

        _difficulty = StaticData.Instance.SelectedDifficulty;
        _topic = StaticData.Instance.SelectedTopic;

        Debug.Log($"Difficulty: {_difficulty}");
        Debug.Log($"Topic: {_topic}");

        this.player = GameObject.FindGameObjectWithTag("Player").transform;
        this.playerStats = player.gameObject.GetComponent<PlayerStats>();

        GameObject _timelineObj = GameObject.FindGameObjectWithTag("Timeline");
        this.timeline = _timelineObj.GetComponent<DropZone>();
        this.timelineCardContainer = _timelineObj.transform.GetChild(0);

        this.deck =
            GameObject.FindGameObjectWithTag("Deck").GetComponent<Deck>();
        this.graveyard =
            GameObject
                .FindGameObjectWithTag("Graveyard")
                .GetComponent<Graveyard>();

        this.livesContainer =
            GameObject.FindGameObjectWithTag("PlayerLives").transform;

        this.menuManager =
            GameObject
                .FindGameObjectWithTag("Menu")
                .GetComponent<SinglePlayerMenuManager>();
    }

    void Start()
    {
        switch (_difficulty)
        {
            case DIFFICULTY.Easy:
                playerLives = 0;
                startingCardsCount = 8;
                break;
            case DIFFICULTY.Medium:
                playerLives = 5;
                startingCardsCount = 8;
                break;
            case DIFFICULTY.Hard:
                playerLives = 3;
                startingCardsCount = 8;
                break;
        }

        this.playerStats.initialLife = playerLives;

        InitLives();
        PlaceCardsInTimeline();
        DrawCards();

        InitTimer();
    }

    void InitTimer()
    {
        if (_difficulty == DIFFICULTY.Hard)
        {
            timer.seconds = turnSeconds;
            timer.StartTimer();
        }
        else
        {
            timer.gameObject.SetActive(false);
        }
    }

    void InitLives()
    {
        for (int i = 0; i < playerLives; i++)
        {
            Instantiate(lifePrefab, livesContainer);
        }
    }

    void DrawCards()
    {
        deck.GiveCard(player, startingCardsCount);
    }

    void PlaceCardsInTimeline()
    {
        CardData[] cards = null;

        switch (_difficulty)
        {
            case DIFFICULTY.Easy:
                cards = deck.PopCards(1);
                break;
            case DIFFICULTY.Medium:
                cards = deck.PopCards(1);
                break;
            case DIFFICULTY.Hard:
                cards = deck.PopCards(3);
                break;
        }

        if (cards != null && cards?.Length > 0)
        {
            foreach (CardData data in cards)
            {
                GameObject cardGO = Instantiate(cardPrefab, timelineCardContainer);
                Card card = cardGO.GetComponent<Card>();
                card.CardData = data;
                card.initCardData();
                card.OnAcceptDrop();
            }
        }
    }

    // Game Actions
    public void DecreaseLife()
    {
        // If Easy Mode, do not check for decreasing life.
        if (_difficulty == DIFFICULTY.Easy) return;

        int lastIndex = livesContainer.transform.childCount - 1;
        livesContainer.transform.GetChild(lastIndex).GetComponent<Life>().Discard(); // destroy last life

        playerStats.remainingLife = lastIndex;

        // If the index is 0 (which means that the destroyed life is the last one)
        // End the Game
        if (lastIndex <= 0)
        {
            timer.StopTimer();
            menuManager.EndGame(false, playerStats);
        }
    }

    // Game Flow Functions
    public void HandleDropInTimeline(Card droppedCard, int dropPos)
    {
        DisableCardDragTemp();

        if (IsDropValid(droppedCard, dropPos))
        {
            timeline.AcceptDrop(droppedCard);
            playerStats.CorrectDrop();

            if (IsHandEmpty()) menuManager.EndGame(true, playerStats);
        }
        else
        {
            HandleInvalidDrop(droppedCard);
            playerStats.IncorrectDrop();
        }

        timer.StartTimer();
    }

    private bool IsHandEmpty()
    {
        // GetChild is necessary since cards are placed in a container inside a DropZone
        return player.GetChild(0).childCount <= 0;
    }

    private void HandleInvalidDrop(Card droppedCard)
    {
        DecreaseLife();

        if (droppedCard != null)
        {
            graveyard.AddCard(droppedCard.CardData);
            droppedCard.Discard();
        }

        try
        {
            deck.GiveCard(player, 1);
        }
        catch (InvalidOperationException)
        {
            // if deck is empty
            // graveyard.PushAllToDeck();
            // deck.GiveCard(player, 1);
            menuManager.EndGame(false, playerStats);
        }
    }

    private bool IsDropValid(Card droppedCard, int dropPos)
    {
        if (droppedCard == null) return false;

        Card[] timelineCards =
            timelineCardContainer.GetComponentsInChildren<Card>();

        // Shorten code
        int
            yearBefore,
            cardYear,
            yearAfter;

        cardYear = droppedCard.CardData.Year;
        try
        {
            yearBefore = timelineCards[dropPos - 1].CardData.Year;
        }
        catch
        {
            yearBefore = int.MinValue;
        }

        try
        {
            yearAfter = timelineCards[dropPos].CardData.Year;
        }
        catch
        {
            yearAfter = int.MaxValue;
        }

        Debug.Log(yearBefore + ", " + cardYear + ", " + yearAfter);
        return (yearBefore <= cardYear && cardYear <= yearAfter);
    }

    // Disable card drag for 1s to prevent spam
    private void DisableCardDragTemp()
    {
        Card[] cards = player.GetChild(0).GetComponentsInChildren<Card>();
        foreach (Card card in cards)
        {
            card.TempDisable();
        }
    }
}
