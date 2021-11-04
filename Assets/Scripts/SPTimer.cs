using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SPTimer : MonoBehaviour
{
    public SinglePlayerGameController gameController;
    private AudioSource audioSource;
    public AudioClip tickSFX;

    int seconds = 15;
    int quizSeconds = 15;
    private bool isRunningQuiz = false;
    public Color runningColor = Color.yellow;
    public Color dangerColor = Color.red;
    private Color defaultColor;

    private int dangerSeconds;
    private int dangerQuizSeconds;

    private int secondsLeft = 0;
    private float interval = 1f;
    private bool isRunning = false;

    public TextMeshProUGUI textObj;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = tickSFX;

        defaultColor = textObj.color;
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
                    SkipTurn();
                }

                //StopTimer();
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

    private void SkipTurn()
    {
        gameController.HandleDropInTimeline(null, -1);
    }

    private void SkipQuiz()
    {
        gameController.AnswerQuiz("");
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

    public void InitTimer(int turnSeconds, int _quizSeconds)
    {
        seconds = turnSeconds;
        dangerSeconds = turnSeconds / 3;

        quizSeconds = _quizSeconds;
        dangerQuizSeconds = _quizSeconds / 3;
    }
}
