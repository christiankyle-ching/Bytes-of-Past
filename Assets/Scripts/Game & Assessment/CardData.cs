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
    Peek, SkipTurn, DoubleDraw, None
}

// [CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class CardData
{
    public string Title;
    public string Inventor;
    public string Description;
    public int Year;
    public Sprite Artwork;
    public CARDCOLOR Color;
    public SPECIALACTION SpecialAction;
    public string ID;

    public CardData(string id, int year, string title, string inventor, string description, SPECIALACTION special = SPECIALACTION.None)
    {
        this.ID = id;
        this.Year = year;
        this.Title = title;
        this.Inventor = inventor;
        this.Description = description;
        this.Artwork = Resources.Load<Sprite>($"Cards/Icons/{id}");
        SetSpecialAction(special);

        this.Color = (CARDCOLOR)UnityEngine.Random.Range(0, 5); // random color scheme
    }

    public void SetSpecialAction(SPECIALACTION special)
    {
        this.SpecialAction = special;
    }
}
