using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SPTimer : MonoBehaviour
{
    public SinglePlayerGameController gameController;
    private AudioSource audioSource;
    public AudioClip tickSFX;

    public int seconds = 15;
    public Color runningColor = Color.yellow;
    public Color dangerColor = Color.red;
    private Color defaultColor;

    private int dangerSeconds;

    private int secondsLeft = 0;
    private float interval = 1f;
    private bool isRunning = false;

    TextMeshProUGUI textObj;

    private void Start()
    {
        textObj = GetComponentInChildren<TextMeshProUGUI>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = tickSFX;

        defaultColor = textObj.color;
        dangerSeconds = seconds / 3;
    }

    private void Update()
    {
        interval -= Time.deltaTime;

        // Loops each 1 second
        if (interval < 0 && isRunning)
        {
            if (secondsLeft > 0)
            {
                secondsLeft--;
            }
            else
            {
                DrawCard();
            }

            UpdateText();

            interval = 1f;
        }
    }

    private void UpdateText()
    {
        textObj.text = $"{secondsLeft} S";

        if (isRunning)
        {
            if (secondsLeft <= dangerSeconds)
            {
                textObj.color = dangerColor;
                audioSource.Play();
            } else
            {
                textObj.color = runningColor;
            }
        }
        else
        {
            textObj.color = defaultColor;
        }
    }

    private void DrawCard()
    {
        gameController.HandleDropInTimeline(null, -1);
    }

    public void StartTimer()
    {
        secondsLeft = seconds;
        interval = 1f;
        isRunning = true;
        UpdateText();
    }

    public void StopTimer()
    {
        isRunning = false;
        secondsLeft = 0;
        UpdateText();
    }
}
