using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MPCardData
{
    public string id;
    public string title;
    public string inventor;
    public string description;
    public int year;
    public CARDCOLOR color;
    
    public MPCardData(string id, int year, string title, string inventor, string description)
    {
        this.id = id;
        this.year = year;
        this.title = title;
        this.inventor = inventor;
        this.description = description;
        
        this.color = (CARDCOLOR)Random.Range(0, 5); // random color scheme
    }

}
