using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Card", menuName ="Card" )]
public class CardData : ScriptableObject
{
    // public -> private to prevent access, used getter methods instead

    // Changed name to title, to avoid hiding GameObject.name
    [SerializeField]
    private string title;
    [SerializeField]
    private string description;
    [SerializeField]
    private int year;
    [SerializeField]
    private Sprite artwork;

    public string Title
    {
        get => this.title;
    }

    public string Description
    {
        get => this.description;
    }

    public int Year
    {
        get => this.year;
    }

    public Sprite Artwork
    {
        get => this.artwork;
    }

}
