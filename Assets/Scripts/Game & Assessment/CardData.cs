using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CARDCOLOR
{
    Blue, Green, Orange, Red, Violet
}

public enum SPECIALACTION
{
    Peek, DisableNext, DoubleDraw, None
}

// [CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class CardData
{
    // public -> private to prevent access, used getter methods instead

    [SerializeField]
    private string title;
    [SerializeField]
    private string inventor;
    [SerializeField]
    private string description;
    [SerializeField]
    private int year;
    [SerializeField]
    private Sprite artwork;
    [SerializeField]
    private CARDCOLOR color;
    [SerializeField]
    private float specialActionRate = 0.3f; // 0.0 to 1.0 = Percentage of special action
    [SerializeField]
    private SPECIALACTION specialAction;

    private string id;

    public string ID
    {
        get => this.id;
    }

    public string Title
    {
        get => this.title;
    }

    public string Inventor
    {
        get => this.Inventor;
    }


    public string Description
    {
        get => this.description;
    }

    public int Year
    {
        get
        {
            return this.year;
        }
    }

    public Sprite Artwork
    {
        get => this.artwork;
    }

    public CARDCOLOR Color
    {
        get => this.color;
    }

    public CardData(string id, int year, string title, string inventor, string description, bool trySpecial = false)
    {
        this.id = id;
        this.year = year;
        this.title = title;
        this.inventor = inventor;
        this.description = description;
        this.artwork = Resources.Load<Sprite>($"Cards/Icons/{id}");

        this.color = (CARDCOLOR)UnityEngine.Random.Range(0, 5); // random color scheme
        if (trySpecial)
        {
            TryGenerateSpecial();
        }
        else
        {
            this.specialAction = SPECIALACTION.None;
        }
    }

    public void TryGenerateSpecial()
    {
        float rand = UnityEngine.Random.Range(0f, 1f);

        if (rand <= specialActionRate)
        {
            Array values = Enum.GetValues(typeof(SPECIALACTION));
            System.Random random = new System.Random();
            this.specialAction = (SPECIALACTION)values.GetValue(random.Next(values.Length));
        }
        else
        {
            this.specialAction = SPECIALACTION.None;
        }
    }
}
