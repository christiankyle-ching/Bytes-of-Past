using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SPCardInfo : MonoBehaviour
{
    public bool isRevealed = false;

    public CardData cardData;
    public int infoIndex;

    private TextMeshProUGUI Year;
    private TextMeshProUGUI Title;
    private TextMeshProUGUI Description;
    private Image Image;
    private Image CardBGImage;

    public void InitCardData(CardData data)
    {
        cardData = data;

        // Get GO References
        Year = transform.Find("Year").GetComponent<TextMeshProUGUI>();
        Title = transform.Find("Title").GetComponent<TextMeshProUGUI>();
        Description = transform.Find("Container").Find("Description").GetComponent<TextMeshProUGUI>();
        Image = transform.Find("Container").Find("Image").GetComponent<Image>();
        CardBGImage = GetComponent<Image>();

        transform.Find("SpecialAction").GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        transform.Find("SpecialAction").GetComponentInChildren<Image>().enabled = false;

        // Set GO values
        Year.text = cardData.Year.ToString();
        Title.text = cardData.Title.ToString();
        Description.text = cardData.Description.ToString();
        Image.sprite = Resources.Load<Sprite>($"Cards/Icons/{cardData.ID}");
        CardBGImage.sprite = GetColoredSprite(cardData.Color);

        // Name in Scene
        name = cardData.Year.ToString();
    }

    public static Sprite GetColoredSprite(CARDCOLOR color)
    {
        switch (color)
        {
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
}
