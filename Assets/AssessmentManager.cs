using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum TOPIC
{
    Computer = 0,
    Networking = 1,
    Software = 2
}

public class AssessmentManager : MonoBehaviour
{
    public GameObject questionText;

    public GameObject btnAnswer1;

    public GameObject btnAnswer2;

    public GameObject btnAnswer3;

    public GameObject endGameMenu;

    public GameObject txtScore;

    public GameObject txtTopic;

    // QUESTIONS DATA
    private Stack<QuestionData> questions = new Stack<QuestionData>();

    private QuestionData currentQuestion;

    private string[] currentChoices;

    private int currentScore = 0;

    private int totalQuestionCount;

    void Awake()
    {
        btnAnswer1
            .GetComponent<Button>()
            .onClick
            .AddListener(() => SelectAnswer1());

        btnAnswer2
            .GetComponent<Button>()
            .onClick
            .AddListener(() => SelectAnswer2());

        btnAnswer3
            .GetComponent<Button>()
            .onClick
            .AddListener(() => SelectAnswer3());

        LoadQuestions();
        ShowNextQuestion();
    }

    void LoadQuestions()
    {
        // TODO: Apply Different Topics
        QuestionData[] resourcesQuestions =
            Resources.LoadAll<QuestionData>("AssessmentTests/Computer");

        foreach (QuestionData question in resourcesQuestions)
        {
            questions.Push (question);
        }

        totalQuestionCount = resourcesQuestions.Length;
    }

    void ShowNextQuestion()
    {
        Debug.Log($"SCORE: {currentScore}/{totalQuestionCount}");

        // TODO: If no questions left, show score and save and exit.
        if (questions.Count <= 0)
        {
            EndTest();
            return;
        }

        currentQuestion = questions.Pop();
        currentChoices = currentQuestion.Choices;

        questionText.GetComponent<TextMeshProUGUI>().text =
            $@"Question: {totalQuestionCount - questions.Count}/{
                totalQuestionCount}

            {currentQuestion.Question}";
        btnAnswer1.GetComponentInChildren<TextMeshProUGUI>().text =
            currentChoices[0] ?? "NO DATA";
        btnAnswer2.GetComponentInChildren<TextMeshProUGUI>().text =
            currentChoices[1] ?? "NO DATA";
        btnAnswer3.GetComponentInChildren<TextMeshProUGUI>().text =
            currentChoices[2] ?? "NO DATA";
    }

    void EndTest()
    {
        txtScore.GetComponent<TextMeshProUGUI>().text =
            $"Score: {currentScore}/{totalQuestionCount}";

        // TODO: Set Topic
        txtTopic.GetComponent<TextMeshProUGUI>().text = "Computer";

        endGameMenu.SetActive(true);
    }

    void SaveAssessmentTestScore(TOPIC topic)
    {
        float accuracy = currentScore / totalQuestionCount;
        PlayerPrefs.SetFloat($"topic{topic}", accuracy);
    }

    // Answer Methods
    void SelectAnswer1()
    {
        if (currentQuestion.isAnswerCorrect(currentChoices[0]))
        {
            currentScore++;
        }
        ShowNextQuestion();
    }

    void SelectAnswer2()
    {
        if (currentQuestion.isAnswerCorrect(currentChoices[1]))
        {
            currentScore++;
        }
        ShowNextQuestion();
    }

    void SelectAnswer3()
    {
        if (currentQuestion.isAnswerCorrect(currentChoices[2]))
        {
            currentScore++;
        }
        ShowNextQuestion();
    }
}
