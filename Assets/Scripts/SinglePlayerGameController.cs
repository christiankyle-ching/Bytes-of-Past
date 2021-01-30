using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerGameController : MonoBehaviour
{
    private int startingCardsCount = 4;
    private int playerLives = 5;

    // References
    private Transform player;
    private DropZone timeline;
    private Transform timelineCardContainer;
    private Deck deck;
    private Graveyard graveyard;

    private void Awake()
    {
        this.player = GameObject.FindGameObjectWithTag("Player").transform;

        GameObject _timelineObj = GameObject.FindGameObjectWithTag("Timeline");
        this.timeline = _timelineObj.GetComponent<DropZone>();
        this.timelineCardContainer = _timelineObj.transform.GetChild(0);

        this.deck = GameObject.FindGameObjectWithTag("Deck").GetComponent<Deck>();
        this.graveyard = GameObject.FindGameObjectWithTag("Graveyard").GetComponent<Graveyard>();
    }
    // Start is called before the first frame update
    void Start()
    {
        deck.GiveCard(player, startingCardsCount);
    }

    public void HandleDropInTimeline(Card droppedCard, int dropPos)
    {
        if (IsDropValid(droppedCard, dropPos))
        {
            timeline.AcceptDrop(droppedCard);
        } else
        {
            HandleInvalidDrop(droppedCard);
        }
    }

    private void HandleInvalidDrop(Card droppedCard)
    {
        graveyard.AddCard(droppedCard.CardData);
        Destroy(droppedCard.gameObject);

        try
        {
            deck.GiveCard(player, 1);
        } catch (InvalidOperationException ex)
        {
            // if deck is empty
            graveyard.PushAllToDeck();
            deck.GiveCard(player, 1);
        }

    }

    private bool IsDropValid(Card droppedCard, int dropPos)
    {
        Card[] timelineCards = timelineCardContainer.GetComponentsInChildren<Card>();

        // Shorten code
        int yearBefore, cardYear, yearAfter;
        cardYear = droppedCard.randomYear; // DEBUG: Use Card.CardData.Year on implementation, replace all randomYear
        try
        {
            yearBefore = timelineCards[dropPos - 1].randomYear;
        }
        catch
        {
            yearBefore = int.MinValue;
        }

        try
        {
            yearAfter = timelineCards[dropPos].randomYear;
        }
        catch
        {
            yearAfter = int.MaxValue;
        }

        //Debug.Log("Before: " + yearBefore);
        //Debug.Log("Dropped: " + cardYear);
        //Debug.Log("After: " + yearAfter);

        return (yearBefore <= cardYear && cardYear <= yearAfter);
    }
}
