using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerGameController : MonoBehaviour
{
    int playerLives = 0;

    int startingCardsCount = 8;

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

    private SinglePlayerMenuManager menuManager;

    StaticData staticData;

    DIFFICULTY _difficulty = DIFFICULTY.Easy;

    TOPIC _topic = TOPIC.Computer;

    // GAME: Start
    private void Awake()
    {
        try
        {
            staticData =
                GameObject
                    .FindWithTag("Static Data")
                    .GetComponent<StaticData>();

            _difficulty = staticData.SelectedDifficulty;
            _topic = staticData.SelectedTopic;
        }
        catch (System.NullReferenceException)
        {
            Debug.LogError("Static Data Not Found: Play from the Main Menu");
            staticData = new StaticData();
        }

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

        InitLives();
        DrawCards();
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

    // Game Actions
    public void DecreaseLife()
    {
        // If Easy Mode, do not check for decreasing life.
        if (_difficulty == DIFFICULTY.Easy) return;

        playerLives--;

        int lastIndex = livesContainer.transform.childCount - 1;

        //Destroy(livesContainer.transform.GetChild(lastIndex).gameObject); // destroy last life
        livesContainer.transform.GetChild(lastIndex).GetComponent<Life>().Discard(); // destroy last life

        if (lastIndex <= 0)
        {
            menuManager.EndGame(false, playerStats);
        }
    }

    // Game Flow Functions
    public void HandleDropInTimeline(Card droppedCard, int dropPos)
    {
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
    }

    private bool IsHandEmpty()
    {
        // GetChild is necessary since cards are placed in a container inside a DropZone
        return player.GetChild(0).childCount <= 0;
    }

    private void HandleInvalidDrop(Card droppedCard)
    {
        DecreaseLife();

        // Add card first in graveyard, so that
        // if there's no card left in deck, something in graveyard
        // can be pulled by the deck
        // then give it back again
        graveyard.AddCard(droppedCard.CardData);
        droppedCard.Discard();

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
}
