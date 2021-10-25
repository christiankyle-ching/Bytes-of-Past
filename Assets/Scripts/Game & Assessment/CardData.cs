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

    public CardData(string id, int year, string title, string inventor, string description)
    {
        this.id = id;
        this.year = year;
        this.title = title;
        this.inventor = inventor;
        this.description = description;
        this.artwork = Resources.Load<Sprite>($"Cards/Icons/{id}");

        this.color = (CARDCOLOR)UnityEngine.Random.Range(0, 5); // random color scheme
    }
}
