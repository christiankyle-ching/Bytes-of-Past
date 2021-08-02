using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    // Game Handler
    private SinglePlayerGameController gameController;

    // Reference to the dropzone's card container
    private Transform cardContainer;

    
    private bool IsTimeline
    {
        get => tag == "Timeline";
    }

    void Awake()
    {
        cardContainer = this.transform.GetChild(0);
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SinglePlayerGameController>();

        // if the dropzone is the timeline,
        // disable dragging of cards inside
        if (IsTimeline) DisableAllCardsDrag(); 
    }

    void DisableAllCardsDrag()
    {
        Card[] cards = cardContainer.GetComponentsInChildren<Card>();
        foreach (var card in cards)
        {
            card.canDrag = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return; // if nothing is dragged

        Card card = eventData.pointerDrag.GetComponent<Card>();

        // upon enter, set the placeholderContainer to this dropzone's container
        if (card != null && card.canDrag) card.placeholderContainer = cardContainer;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return; // if nothing is dragged

        Card card = eventData.pointerDrag.GetComponent<Card>();

        if (card == null) return; // if drag is not a card

        // upon exit on any dropzone,
        // set placeholderContainer back to the card's original container,
        // so that if the card is dropped outside any dropzone,
        // card will snap back to it's original place [see Card.OnEndDrag]
        if (card.canDrag && card.placeholderContainer == this.transform)
            card.placeholderContainer = card.initialContainer;

    }

    public void OnDrop(PointerEventData eventData)
    {
        Card card = eventData.pointerDrag.GetComponent<Card>();

        // if OnDrop object is not a card, return
        if (card == null) return;

        // drop only if dropped to timeline
        if (IsTimeline && card.canDrag)
        {
            gameController.HandleDropInTimeline(card, card.PlaceholderPos);
        }
    }

    public void AcceptDrop(Card card)
    {
        card.initialContainer = this.cardContainer;
        card.Disable();
    }
}
