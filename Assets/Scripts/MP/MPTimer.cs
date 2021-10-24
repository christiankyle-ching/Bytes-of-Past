using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mirror;

public class MPTimer : MonoBehaviour
{
    public int seconds = 10;
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
                StopTimer();
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
            textObj.color = (secondsLeft <= dangerSeconds) ? dangerColor : runningColor;
        }
        else
        {
            textObj.color = defaultColor;
        }
    }

    private void DrawCard()
    {
        NetworkClient.connection.identity.GetComponent<PlayerManager>().PlayCard(null, -1, -1, false);
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
