using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mirror;

public class MPTimer : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip tickSFX;

    // TODO: Set in Prod
#if UNITY_EDITOR
    private int seconds = 5;
    private int quizSeconds = 5;
#else
    private int seconds = 30;
    private int quizSeconds = 30;
#endif

    private bool isRunningQuiz = false;
    public Color runningColor = Color.yellow;
    public Color dangerColor = Color.red;
    private Color defaultColor;

    private int dangerSeconds;
    private int dangerQuizSeconds;

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
        dangerQuizSeconds = quizSeconds / 3;
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
                if (isRunningQuiz)
                {
                    SkipQuiz();
                }
                else
                {
                    DrawCard();
                }

                StopTimer();
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
            bool inDanger = isRunningQuiz ?
                (secondsLeft <= dangerQuizSeconds) :
                (secondsLeft <= dangerSeconds);

            if (inDanger)
            {
                textObj.color = dangerColor;
                SoundManager.Instance.PlayTimerTickSFX();
            }
            else
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
        NetworkClient.localPlayer.GetComponent<PlayerManager>().PlayCard(null, -1, -1, false);
    }

    private void SkipQuiz()
    {
        NetworkClient.localPlayer.GetComponent<PlayerManager>().AnswerQuiz("");
    }

    public void StartTimer()
    {
        secondsLeft = seconds;
        interval = 1f;
        isRunning = true;
        isRunningQuiz = false;
        UpdateText();
    }

    public void StartQuizTimer()
    {
        secondsLeft = quizSeconds;
        interval = 1f;
        isRunning = true;
        isRunningQuiz = true;
        UpdateText();
    }

    public void StopTimer()
    {
        isRunning = false;
        isRunningQuiz = false;
        secondsLeft = 0;
        UpdateText();
    }
}
