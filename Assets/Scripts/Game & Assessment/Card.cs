using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool canDrag;

    [SerializeField]
    private CardData cardData;
    public CardData CardData
    {
        get => this.cardData;
        set => this.cardData = value;
    }

    // UI Child Elements
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public Image image;
    public Image cardBGImage;

    public TextMeshProUGUI yearObj;

    // Container
    public Transform initialContainer;

    // Placeholder
    private GameObject placeholder;
    public Transform placeholderContainer;
    public int PlaceholderPos { get => placeholder.transform.GetSiblingIndex(); }


    // Start is called before the first frame update
    void Awake()
    {
        canDrag = true;
        initialContainer = transform.parent;
        placeholderContainer = initialContainer;
    }

    private void Start()
    {
        // this.initCardData();
    }

    public void initCardData()
    {
        this.title.text = this.cardData.Title;
        this.description.text = this.cardData.Description;
        this.image.sprite = this.cardData.Artwork;
        this.cardBGImage.sprite = GetColoredSprite(this.cardData.Color); 

        this.yearObj.text = this.cardData.Year.ToString();

        // Name in Scene
        name = this.cardData.Year.ToString();        
    }

    public static Sprite GetColoredSprite(CARDCOLOR color) {
        switch(color) {
            case CARDCOLOR.Orange:
                return Resources.Load<Sprite>("Cards/Templates/Orange");
            case CARDCOLOR.Blue:
                return Resources.Load<Sprite>("Cards/Templates/Blue");
            case CARDCOLOR.Green:
                return Resources.Load<Sprite>("Cards/Templates/Green");
            case CARDCOLOR.Red:
                return Resources.Load<Sprite>("Cards/Templates/Red");
            case CARDCOLOR.Violet:
                return Resources.Load<Sprite>("Cards/Templates/Violet");
            default: 
                return Resources.Load<Sprite>("Cards/Templates/Orange");
        }
    }

    void CreatePlaceholder()
    {
        // TODO: This can be converted as a Prefab
        placeholder = new GameObject();
        placeholder.name = "Placeholder Card while Dragging";

        RectTransform rect = placeholder.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(100, 140);

        placeholder.transform.SetParent(placeholderContainer);
        placeholder.transform.SetSiblingIndex(transform.GetSiblingIndex());
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag) return;

        CreatePlaceholder();

        // placing the card up in the Game Canvas
        // to remove effect of LayoutElement, while preserving
        // cards count in the container
        transform.SetParent(initialContainer.parent);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag) return;

        // TODO: currently anchored to the center, use offset instead
        transform.position = eventData.pointerCurrentRaycast.worldPosition;

        AdjustPlaceholderPos();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // on drag end, set the card's parent
        // to wherever the placeholder is currently at, with its index
        // NOTE: placeholderContainer's value is manipulated by
        // DropZone class [see DropZone.OnPointer events]
        transform.SetParent(placeholderContainer);
        transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
        Destroy(placeholder);
    }

    void AdjustPlaceholderPos()
    {
        // Set new placeholderContainer when value is changed by DropZone
        if (placeholderContainer != placeholder.transform.parent)
            placeholder.transform.SetParent(placeholderContainer);

        // Set default index of placeholder to last,
        // so if loop does not break (all cards are to the left of dragged
        // card, the default index will take place.
        int newIndex = placeholderContainer.childCount;

        for (int i = 0; i < placeholderContainer.childCount; i++)
        {
            if (transform.position.x < placeholderContainer.GetChild(i).position.x)
            {
                newIndex = i;

                // TODO: attempt to fix immediate switch of cards when going left to right
                if (placeholder.transform.GetSiblingIndex() < placeholderContainer.GetChild(i).position.x)
                    newIndex--;

                break;
            }
        }

        placeholder.transform.SetSiblingIndex(newIndex);
    }

    void OnDestroy()
    {
        // makes sure to destroy the placeholder whenever this object
        // is destroyed
        Destroy(placeholder);
    }

    public void Disable()
    {
        canDrag = false;
    }

}
