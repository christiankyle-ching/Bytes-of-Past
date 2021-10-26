using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private int attemptCount;
    private int correctAttempts;

    public int remainingLife;
    public int initialLife;

    public float Accuracy
    {
        get => (float)this.correctAttempts / (float)this.attemptCount;
    }

    public string AccuracyText
    {
        get
        {
            Debug.Log(this.correctAttempts + "/" + this.attemptCount);

            double percentageForm = System.Math.Round((this.Accuracy * 100), 2);
            return "Accuracy: " + percentageForm + "%";
        }
    }

    private void Awake()
    {
        this.correctAttempts = 0;
        this.attemptCount = 0;
        this.remainingLife = 0;
        this.initialLife = 0;
    }

    public void CorrectDrop()
    {

        attemptCount++;
        correctAttempts++;
    }

    public void IncorrectDrop()
    {
        attemptCount++;
        remainingLife--;
    }
}
