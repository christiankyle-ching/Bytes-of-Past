using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private decimal attemptCount;
    private decimal correctAttempts;

    private string debugTextObjectName = "Debug_PlayerStats";
    private TextMeshProUGUI debugText;

    public decimal Accuracy
    {
        get => this.correctAttempts / this.attemptCount;
    }

    public string AccuracyText
    {
        get
        {
            decimal percentageForm = System.Math.Round(this.Accuracy * 100, 2);
            return "Accuracy: " + percentageForm + "%";
        }
    }

    private void Awake()
    {
        this.correctAttempts = 0;
        this.attemptCount = 0;
        
        this.debugText = GameObject.Find(debugTextObjectName).GetComponent<TextMeshProUGUI>();
    }

    public void CorrectDrop()
    {
        attemptCount++;
        correctAttempts++;

        debugText.text = this.AccuracyText;
    }

    public void IncorrectDrop()
    {
        attemptCount++;

        debugText.text = this.AccuracyText;
    }
}
