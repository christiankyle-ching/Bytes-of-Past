using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MPCardInfo : MonoBehaviour
{
    public bool isRevealed = false;

    public CardData cardData;
    public int infoIndex;

    private TextMeshProUGUI Year;
    private TextMeshProUGUI Title;
    private TextMeshProUGUI Description;
    private Image Image;
    private Image CardBGImage;

    private TextMeshProUGUI SpecialActionLabel;
    private Image SpecialActionImage;

    public void InitCardData(CardData data, SPECIALACTION special = SPECIALACTION.None)
    {
        cardData = data;
        cardData.SetSpecialAction(special);

        // Get GO References
        Year = transform.Find("Year").GetComponent<TextMeshProUGUI>();
        Title = transform.Find("Title").GetComponent<TextMeshProUGUI>();
        Description = transform.Find("Container").Find("Description").GetComponent<TextMeshProUGUI>();
        Image = transform.Find("Container").Find("Image").GetComponent<Image>();
        CardBGImage = GetComponent<Image>();

        SpecialActionLabel = transform.Find("SpecialAction").GetComponentInChildren<TextMeshProUGUI>();
        SpecialActionImage = transform.Find("SpecialAction").GetComponentInChildren<Image>();

        // Set GO values
        Year.text = cardData.Year.ToString();
        Title.text = cardData.Title.ToString();
        Description.text = cardData.Description.ToString();
        Image.sprite = Resources.Load<Sprite>($"Cards/Icons/{cardData.ID}");
        CardBGImage.sprite = GetColoredSprite(cardData.Color);

        SpecialActionLabel.text = GetSpecialActionLabel(cardData.SpecialAction);
        SpecialActionImage.sprite = GetSpecialActionSprite(cardData.SpecialAction);

        // Name in Scene
        name = cardData.Year.ToString();
    }

    public void RevealCard()
    {
        isRevealed = true;
        GetComponent<Animator>().SetTrigger("Correct");
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

    public static Sprite GetSpecialActionSprite(SPECIALACTION special)
    {
        switch (special)
        {
            case SPECIALACTION.Peek:
                return Resources.Load<Sprite>($"SpecialActions/Peek");
            case SPECIALACTION.SkipTurn:
                return Resources.Load<Sprite>($"SpecialActions/SkipTurn");
            case SPECIALACTION.DoubleDraw:
                return Resources.Load<Sprite>($"SpecialActions/DoubleDraw");
            default:
                return null;
        }
    }

    private static string GetSpecialActionLabel(SPECIALACTION special)
    {
        switch (special)
        {
            case SPECIALACTION.Peek:
                return "Peek";
            case SPECIALACTION.SkipTurn:
                return "Skip Turn";
            case SPECIALACTION.DoubleDraw:
                return "Double Draw";
            default:
                return null;
        }
    }
}