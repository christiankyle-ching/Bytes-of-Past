using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graveyard : MonoBehaviour
{
    public AudioSource audioSource;

    private Stack<CardData> cards;

    private Deck deck;

    private void Awake()
    {
        this.cards = new Stack<CardData>();
        this.deck =
            GameObject.FindGameObjectWithTag("Deck").GetComponent<Deck>();
    }

    public void AddCard(CardData cardData)
    {
        cards.Push (cardData);
        PlayTrashSound();

        Debug.Log("Graveyard: " + cards.Count);
    }

    public void PushAllToDeck()
    {
        Debug.Log("Graveyard to Deck: " + cards.Count);
        foreach (CardData cardData in cards)
        {
            deck.AddCard (cardData);
        }
    }

    public void PlayTrashSound()
    {
        audioSource.Play();
    }
}
