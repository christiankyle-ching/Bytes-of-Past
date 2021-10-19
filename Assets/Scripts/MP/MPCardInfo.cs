using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MPCardInfo : MonoBehaviour
{
    public MPCardData cardData;
    public int infoIndex;

    private TextMeshProUGUI Year;
    private TextMeshProUGUI Title;
    private TextMeshProUGUI Description;
    private Image Image;
    private Image CardBGImage;

    public void InitCardData(MPCardData data)
    {
        cardData = data;

        Year = transform.Find("Year").GetComponent<TextMeshProUGUI>();
        Title = transform.Find("Title").GetComponent<TextMeshProUGUI>();
        Description = transform.Find("Container").Find("Description").GetComponent<TextMeshProUGUI>();
        Image = transform.Find("Container").Find("Image").GetComponent<Image>();
        CardBGImage = GetComponent<Image>();

        Year.text = cardData.year.ToString();
        Title.text = cardData.title.ToString();
        Description.text = cardData.description.ToString();
        Image.sprite = Resources.Load<Sprite>($"Cards/Icons/{cardData.id}");
        CardBGImage.sprite = GetColoredSprite(cardData.color);

        // Name in Scene
        name = cardData.year.ToString();
    }

    private static Sprite GetColoredSprite(CARDCOLOR color)
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