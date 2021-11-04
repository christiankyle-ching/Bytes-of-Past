using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graveyard : MonoBehaviour
{
    public AudioSource audioSource;

    private Deck deck;

    private void Awake()
    {
        this.deck =
            GameObject.FindGameObjectWithTag("Deck").GetComponent<Deck>();
    }

    public void AddCard(CardData cardData)
    {
        deck.AddCard(cardData);
        PlayTrashSound();
    }

    public void PlayTrashSound()
    {
        SoundManager.Instance.PlayDiscardSFX();
    }
}
