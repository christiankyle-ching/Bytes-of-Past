using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private int attemptCount;
    private int correctAttempts;


    public float Accuracy
    {
        get => this.correctAttempts / this.attemptCount;
    }

    public string AccuracyText
    {
        get
        {
            double percentageForm = System.Math.Round((this.Accuracy * 100), 2);
            return "Accuracy: " + percentageForm + "%";
        }
    }

    private void Awake()
    {
        this.correctAttempts = 0;
        this.attemptCount = 0;
    }

    public void CorrectDrop()
    {
        attemptCount++;
        correctAttempts++;
    }

    public void IncorrectDrop()
    {
        attemptCount++;
    }
}
